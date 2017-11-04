using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Elanor.Properties;
using Elanor.Misc;
using shared;

namespace Elanor.DatFile
{
    public class DatController
    {
        public DatController(string path)
        {
            Path = path;

            if (IsValid)
            {
                CreateEmptySet();
            }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;

                if (File.Exists(value))
                {
                    IsValid = true;
                }
            }
        }

        public Dictionary<PatchContentType, DatPatchInfo> DatPatches { get; } = new Dictionary<PatchContentType, DatPatchInfo>();

        public bool IsValid { get; private set; }

        public shared.DatFile Dat { get; private set; }

        public async Task<bool> ReOpen(bool readOnly)
        {
            Close();
            Dat = new shared.DatFile();

            return await OpenAsync(readOnly);
        }

        public async Task<bool> OpenAsync(bool readOnly)
        {
            return readOnly ? await Task.Run(OpenRead) : await Task.Run(OpenWrite);
        }

        private async Task<bool> OpenRead()
        {
            try
            {
                if (Dat == null)
                {
                    Dat = new shared.DatFile();
                }

                await Task.Run(() => Dat.Open(Path, true));

                return true;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                return false;
            }           
        }

        private async Task<bool> OpenWrite()
        {
            try
            {
                if (Dat == null)
                {
                    Dat = new shared.DatFile();
                }

                await Task.Run(() => Dat.Open(Path, false));

                return true;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                return false;
            }
        }

        public void Close()
        {
            try
            {
                if (Dat != null)
                {
                    Dat.Flush();
                    Dat.Dispose();
                    Dat = null;
                }
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
            }
        }

        private void CreateEmptySet()
        {
            DatPatches.Clear();

            foreach (var contentType in EnumHelper<PatchContentType>.GetValues(PatchContentType.Video))
            {
                DatPatches.Add(contentType, new DatPatchInfo(contentType));
            }
        }

        public bool IsSubscriptionNotEmpty
        {
            get { return DatPatches.Any(x => x.Value.IsApplySubscribed); }
        }

        public async Task GetMarkAsync()
        {
            IsValid = await ReadNinjaMarkAsync();
        }

        private async Task<bool> ReadNinjaMarkAsync()
        {
            try
            {
                if (!await ReOpen(true))
                {
                    return false;
                }

                // try open ninja id
                var id = Settings.Default.MarkId;
                if (Dat.SubfileInfo.ContainsKey(id))
                {
                    var tsf = new TextSubfile(Dat[id].Data);
                    var vToken = Settings.Default.TurbineLauncherVersionToken;

                    // if turbine launcher version changed since last patch, force patch all
                    // otherwise check regular per-patch update availability
                    if (tsf.Body.ContainsKey(vToken) && tsf.Body[vToken].Pieces.Length == 1)
                    {
                        if (string.CompareOrdinal(tsf.Body[vToken].Pieces[0], Setup.TurbineLauncherVersion) == 0)
                        {
                            foreach (var patch in DatPatches.Values)
                            {
                                // get applied patches data
                                if (tsf.Body.ContainsKey(patch.Token) && tsf.Body[patch.Token].Pieces.Length == 1)
                                {
                                    patch.ReceiveMark(tsf.Body[patch.Token].Pieces[0]);
                                }
                            }
                        }
                    }
                }

                Close();

                return true;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                return false;
            }
        }

        public async Task<bool> WriteNinjaMark()
        {
            try
            {
                if (!await ReOpen(false))
                {
                    return false;
                }

                var fragments = DatPatches.Values.ToDictionary(patch => patch.Token, NinjaMark.ComposeMark);
                fragments.Add(Settings.Default.TurbineLauncherVersionToken, Setup.TurbineLauncherVersion);

                var id = Settings.Default.MarkId;
                var data = TextSubfileBuilder.CustomData(id, fragments);

                IntPtr buffer = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, buffer, data.Length);

                var purge = Dat.Purge(id);
                var put = Dat.Put(id, buffer, 0, data.Length, 69, 69);

                Marshal.FreeHGlobal(buffer);

                #if DEBUG

                Logger.Write($"NinjaMark #{id}[{purge} | {put}]");

                #endif

                if (put != data.Length)
                {
                    Logger.Write("ошибка записи метки патчей в файл данных.");
                }

                Close();

                return true;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                return false;
            }
        }

        public async Task<bool> WipeNinjaMark()
        {
            try
            {
                if (!await ReOpen(false))
                {
                    return false;
                }

                var id = Settings.Default.MarkId;
                if (Dat.SubfileInfo.ContainsKey(id))
                {
                    var purge = Dat.Purge(id);
                    Dat.Flush();

                    Logger.Write(purge == 0
                        ? "метка патчей успешно стёрта из файла данных."
                        : "ошибка удаления метки патчей из файла данных.");
                }

                Close();

                return true;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                return false;
            }
        }

        public bool InstallOrNot(Patch.Patch patch)
        {
            return DatPatches[patch.ContentType].InstallOrNot(patch);
        }

        public void RenewMark(Patch.Patch patch)
        {
            DatPatches[patch.ContentType].ReceiveMark(NinjaMark.CreateMark(patch.PatchVersion.Full, patch.Date, true));
        }

        public void ForceRenewMark()
        {
            foreach (var dict in DatPatches)
            {
                dict.Value.ReceiveMark(NinjaMark.CreateMark("null_u0_v100", dict.Value.Date, dict.Value.IsApplySubscribed));
            }
        }
    }

    public class DatPatchInfo
    {
        public DatPatchInfo(PatchContentType contentType)
        {
            ContentType = contentType;
        }

        public string Date { get; private set; }

        public string Version { get; private set; } = "null_u0_v100";

        private bool _isApplySubscribed;

        public bool IsApplySubscribed
        {
            get
            {
                return ContentType == PatchContentType.Font || ContentType == PatchContentType.Texture ||
                       _isApplySubscribed;
            }
            set { _isApplySubscribed = value; }
        }

        public bool IsApplied => !string.IsNullOrWhiteSpace(Date) && !string.IsNullOrWhiteSpace(Version);

        public PatchContentType ContentType { get; }

        public long Token => (long)ContentType;

        public bool IsValid { get; private set; }

        public string ViewDate
        {
            get
            {
                var date = ContentType == PatchContentType.Video ? "В разработке" : Date;

                return string.IsNullOrWhiteSpace(date) ? "Оригинал" : date;
            }
        }

        public void ReceiveMark(string mark)
        {
            string version = null;
            string date = null;
            var subscribed = false;

            var decomposed = NinjaMark.DecomposeMark(mark, ref version, ref date, ref subscribed);
            if (decomposed)
            {
                Version = version;
                Date = date;
                IsApplySubscribed = subscribed;

                IsValid = true;
            }
        }

        public bool InstallOrNot(Patch.Patch patch)
        {
            if (!patch.IsDownloaded)
            {
                return false;
            }

            if (IsApplySubscribed && (!IsValid || patch.IsHigherThan(Version)))
            {
                return true;
            }

            return false;
        }
    }
}
