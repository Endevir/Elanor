using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using shared;

namespace Elanor.DatFile
{
    public class DatPreloader
    {
        public DatPreloader(DatController datController, List<uint> patchFiles)
        {
            _patchFiles = patchFiles;
            _datController = datController;
        }

        private readonly DatController _datController;
        private readonly List<uint> _patchFiles;

        private int _currentSize;       
        private int _currentIndex;

        public Dictionary<uint, Subfile> Subfiles;

        public int PreloadSize { get; } = 100 * 1024 * 1024;

        public async Task<int> Preload()
        {
            try
            {
                _currentSize = 0;
                Subfiles = new Dictionary<uint, Subfile>();

                if (_currentIndex == _patchFiles.Count)
                {
                    return 0;
                }

                if (!await _datController.ReOpen(true))
                {
                    return 0;
                }

                var preloaded = 0;
                for (var i = _currentIndex; i < _patchFiles.Count; i++)
                {
                    var id = _patchFiles[i];
                    var subfile = _datController.Dat[id];

                    if (subfile == null)
                    {
                        Logger.Write($"не удалось прочитать файл #{id}, пропускаю.");
                        continue;
                    }

                    Subfiles.Add(id, subfile);
                    _currentSize += subfile.Data.Length;

                    preloaded++;

                    if (_currentSize >= PreloadSize || i == _patchFiles.Count - 1)
                    {
                        _currentIndex = i + 1;
                        break;
                    }
                }

                _datController.Close();

                return preloaded;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                Subfiles.Clear();
                return 0;
            }
        }
    }
}
