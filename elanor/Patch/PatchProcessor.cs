// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Elanor.Properties;
using Elanor.DatFile;
using Elanor.Misc;
using shared;

namespace Elanor.Patch
{
    internal class PatchProcessor : INotifyPropertyChanged
    {
        public async Task<Dictionary<Patch, int>> PatchAsync(DatController datController, IEnumerable<Patch> patches)
        {
            return await Task.Run(() => RunAsync(datController, patches));
        }

        private async Task<Dictionary<Patch, int>> RunAsync(DatController mainDatController, IEnumerable<Patch> patches)
        {
            var results = new Dictionary<Patch, int>();
            var enumerable = patches as Patch[] ?? patches.ToArray();

            foreach (var patch in enumerable)
            {
                if (!mainDatController.InstallOrNot(patch))
                {
                    continue;
                }

                // recognize and prepare appropriate client dat files
                var clientDatList = new Dictionary<int, DatController> {{ClientDat.LocalEnglish.Id, mainDatController}};
                foreach (var clientDatId in patch.ClientDatRelated)
                {
                    if (clientDatId == ClientDat.LocalEnglish.Id)
                    {
                        continue;
                    }

                    var cdat = ClientDat.GetById(clientDatId);
                    if (cdat == null)
                    {
                        continue;
                    }

                    var controller = new DatController(cdat.FullPath);
                    if (controller.IsValid)
                    {
                        clientDatList.Add(clientDatId, controller);
                    }
                }

                int result;

                switch (patch.ContentType)
                {
                    case PatchContentType.Text:
                    case PatchContentType.Image:
                    case PatchContentType.Sound:
                    case PatchContentType.Font:
                    case PatchContentType.Texture:
                        result = await PatchDat(clientDatList, patch);
                        break;
                    case PatchContentType.Loadscreen:
                        result = ReplaceLoadScreens(Settings.Default.LotroPath, patch);
                        break;
                    default:
                        result = -1;
                        break;
                }

                if (result > -1)
                {
                    mainDatController.RenewMark(patch);
                }

                results.Add(patch, result);
                Logger.Write($"завершена установка патча {patch.PatchVersion.Full} с результатом {result}.");

                // old client dat, have to update it first
                if (result == -2)
                {
                    break;
                }
            }

            if (results.Count > 0)
            {
                if (!await mainDatController.WriteNinjaMark())
                {
                    Logger.Write("не удалось записать метку применения патчей.");
                }
            }

            return results;
        }

        private async Task<int> PatchDat(Dictionary<int, DatController> clientDatList, Patch patch)
        {
            var errors = 0;
            var dataErrors = 0;
            var contentType = patch.ContentType;

            try
            {
                foreach (var clientDatId in clientDatList.Keys)
                {
                    var datController = clientDatList[clientDatId];
                    if (!await datController.OpenAsync(true))
                    {
                        errors = -1;
                        continue;
                    }

                    var builder = SubfileBuilder.GetBuilder(datController, patch);
                    if (builder == null)
                    {
                        errors = -1;
                        continue;
                    }

                    if (builder.IsOldClientDat)
                    {
                        errors = -2;
                        return -2;
                    }
                
                    using (var con = new SQLiteConnection($"Data Source = {patch.Path}; Version = 3;"))
                    {
                        con.Open();

                        var currentCount = 0;
                        var overallCount = builder.Files.Count;

                        var preloader = new DatPreloader(datController, builder.Files);
                        int preloaded;

                        do
                        {
                            preloaded = await Task.Run(() => preloader.Preload());

                            if (!await datController.ReOpen(false))
                            {
                                Logger.Write("не удалось переключить режим файла данных.");
                                break;
                            }

                            foreach (var dict in preloader.Subfiles)
                            {
                                OnPatchProgressChanged(contentType, ++currentCount, overallCount);

                                var id = dict.Key;
                                var data = builder.Data(id, datController.Dat, con, dict.Value, contentType, ref dataErrors);

                                if (data == null)
                                {
                                    continue;
                                }

                                IntPtr buffer = Marshal.AllocHGlobal(data.Length);
                                Marshal.Copy(data, 0, buffer, data.Length);

                                datController.Dat.Purge(id);
                                var put = datController.Dat.Put(id, buffer, 0, data.Length, dict.Value.Version,
                                    datController.Dat.SubfileInfo[id].Iteration);

                                Marshal.FreeHGlobal(buffer);

                                if (put != data.Length)
                                {
                                    Logger.Write($"ошибка записи данных файла #{id}.");
                                }
                            }

                            datController.Close();
                        }
                        while (preloaded > 0);
                    }                    
                }

                return errors < dataErrors ? errors : dataErrors;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                errors = -1;
                return -1;
            }
            finally
            {
                OnPatchApplied(contentType, errors < dataErrors ? errors : dataErrors);
            }
        }

        private int ReplaceLoadScreens(string lotroPath, Patch patch)
        {
            var errors = 0;

            try
            {
                // check original dir
                var logoDir = Path.Combine(lotroPath, Resources.LotroLogoPath);
                if (!Directory.Exists(logoDir))
                {
                    Logger.Write("каталог загрузочных экранов не существует, не делаю ничего.");
                    errors = -1;
                    return -1;
                }

                // acquire existing images
                var ex = new DirectoryInfo(logoDir).GetFiles("*.jpg").Select(file => file.FullName).ToList();
               
                using (var con = new SQLiteConnection($"Data Source = {patch.Path}; Version = 3;"))
                {
                    con.Open();

                    var files = Database.GetFiles(con);
                    if (files.Count == 0)
                    {
                        Logger.Write("патч пуст.");
                        errors = -1;
                        return -1;
                    }

                    if (ex.Count != files.Count)
                    {
                        Logger.Write("число изображений в каталоге и в патче различается.");
                    }

                    for (var i = 0; i < ex.Count && i < files.Count; ++i)
                    {
                        OnPatchProgressChanged(patch.ContentType, i, Math.Max(ex.Count, files.Count));

                        var file = ex[i];
                        var blob = Database.GetBlob(con, files[i]);
                        if (blob == null)
                        {
                            Logger.Write($"не удалось получить данные патча для файла #{files[i]}.");
                            continue;
                        }

                        try
                        {
                            File.SetAttributes(file, FileAttributes.Normal);
                            using (var fs = File.Create(file))
                            {
                                fs.Write(blob, 0, blob.Length);
                            }

                            File.SetAttributes(file, FileAttributes.ReadOnly);

                            #if DEBUG

                            Logger.Write($"файл #{file} перезаписан.");

                            #endif
                        }
                        catch (Exception ex0)
                        {
                            ++errors;
                            Logger.Write(ex0.Message);
                        }
                    }
                }

                return errors;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                errors = -1;
                return -1;
            }
            finally
            {
                OnPatchApplied(patch.ContentType, errors);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<PatchProgressEventArgs> PatchProgressChanged;
        public event EventHandler<PatchProgressEventArgs> PatchListProgressChanged;
        public event EventHandler<PatchAppliedEventArgs> PatchApplied;

        protected void OnPatchProgressChanged(PatchContentType contentType, int current, int all)
        {
            PatchProgressChanged?.Invoke(this, new PatchProgressEventArgs(contentType, current, all));
        }

        protected void OnPatchListProgressChanged(PatchContentType contentType, int current, int all)
        {
            PatchListProgressChanged?.Invoke(this, new PatchProgressEventArgs(contentType, current, all));
        }

        protected void OnPatchApplied(PatchContentType contentType, int errors)
        {
            PatchApplied?.Invoke(this, new PatchAppliedEventArgs(contentType, errors));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class PatchProgressEventArgs : EventArgs
        {
            public PatchProgressEventArgs(PatchContentType contentType, int current, int all)
            {
                All = all;
                Current = current;
                ContentType = contentType;
            }

            public int Current { get; }

            public int All { get; }

            public PatchContentType ContentType { get; }
        }

        public class PatchAppliedEventArgs : EventArgs
        {
            public PatchAppliedEventArgs(PatchContentType contentType, int errors)
            {
                Errors = errors;
                ContentType = contentType;
            }

            public int Errors { get; }

            public PatchContentType ContentType { get; }
        }
    }
}
