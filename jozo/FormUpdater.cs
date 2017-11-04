// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jozo.Properties;

namespace Jozo
{
    public partial class FormUpdater : Form
    {
        public FormUpdater()
        {
            InitializeComponent();

            ServicePointManager.DefaultConnectionLimit = 40;
        }

        private string _updateDir;

        public bool InitVariables()
        {
            var updateDir = Path.Combine(Application.StartupPath, Resources.TempUnzipPath);
            if (InitDirectory(updateDir))
            {
                _updateDir = updateDir;
                return true;
            }

            return false;
        }

        public static bool InitDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return false;
            }
        }

        public static string ExecutableVersion(string path)
        {
            var versInfo = FileVersionInfo.GetVersionInfo(path);

            return
                $"{versInfo.FileMajorPart}.{versInfo.FileMinorPart}.{versInfo.FileBuildPart}.{versInfo.FilePrivatePart}";
        }

        private void TryUpdate()
        {
            var patcherPath = Path.Combine(Application.StartupPath, Resources.LauncherExe);
            var version = File.Exists(patcherPath) ? ExecutableVersion(patcherPath) : "0.0.0.0";

            SetStatus("Поиск обновлений...");
            
            RequestUpdate(version)
                .ContinueWith(task =>
                {
                    if (!task.Result)
                    {
                        LaunchPatcher(Resources.ArgsDefault);
                    }
                });
        }

        private async Task<bool> RequestUpdate(string version)
        {
            try
            {
                var remoteVersion = ReadRemoteVersion();
                var compareResult = CompareVersion(version, remoteVersion);
                if (compareResult > 0 && InitVariables())
                {
                    SetStatus("Загрузка обновлений...");
                    
                    await Task.Run(() => DownloadAsync(Settings.Default.UrlUpdate, Resources.LauncherUpdateName));
                }
                else
                {
                    LaunchPatcher(Resources.ArgsDefault);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return false;
            }
        }

        private static int CompareVersion(string current, string remote)
        {
            var splitCurrent = current.Split('.');
            var splitRemote = remote.Split('.');

            if (splitCurrent.Length != splitRemote.Length)
            {
                return -1;
            }

            for (var i = 0; i < splitCurrent.Length; ++i)
            {
                var rem = Convert.ToInt32(splitRemote[i]);
                var cur = Convert.ToInt32(splitCurrent[i]);

                if (rem > cur)
                {
                    return 1;
                }

                if (cur > rem)
                {
                    break;
                }
            }

            return 0;
        }

        private static string ReadRemoteVersion()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead(Settings.Default.UrlVersionInfo))
                    {
                        if (stream != null)
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                return reader.ReadLine();
                            }
                        }

                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return string.Empty;
            }
        }

        private void DownloadAsync(string uri, string name)
        {
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
                    wc.Proxy = null;
                    wc.Credentials = CredentialCache.DefaultCredentials;

                    wc.DownloadFileCompleted += OnDownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri(uri), Path.Combine(_updateDir, name));
                    while (wc.IsBusy)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
        }

        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            SetStatus("Применение обновлений...");

            // extract update archieve
            var extractDir = Path.Combine(_updateDir, "upd");
            var zipName = Path.Combine(_updateDir, Resources.LauncherUpdateName);
            if (Unzip(zipName, extractDir))
            {
                if (ReplaceFiles(extractDir, Application.StartupPath))
                {
                    Directory.Delete(_updateDir, true);
                    LaunchPatcher(Resources.ArgsUpdate);
                }
                else
                {
                    // TODO - else (backup of files, rollback on fail)
                    SafeControl(this, () =>
                    {
                        MessageBox.Show(@"Ошибка при попытке обновления файлов. В разработке.");
                        Application.Exit();
                    });
                }
            }
            else
            {
                Logger.Write("Не удалось распаковать обновление.");
                SetDetails("Не удалось распаковать обновление.");

                Directory.Delete(_updateDir, true);

                LaunchPatcher(Resources.ArgsDefault);
            }
        }

        private static bool ReplaceFiles(string source, string dest)
        {
            try
            {
                foreach (var file in new DirectoryInfo(source).GetFiles("*.*"))
                {
                    var destName = Path.Combine(dest, file.Name);
                    if (File.Exists(destName))
                    {
                        File.SetAttributes(destName, FileAttributes.Normal);
                        File.Delete(destName);
#if DEBUG
                        Logger.Write("Удаление файла " + destName);
#endif
                    }

                    File.Copy(file.FullName, destName);
#if DEBUG
                    Logger.Write("Копирование файла " + destName);
#endif
                }

                foreach (var dir in new DirectoryInfo(source).GetDirectories())
                {
                    if (!ReplaceDir(dir.FullName, Path.Combine(dest, dir.Name)))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return false;
            }
        }

        private static bool ReplaceDir(string sourceDir, string targetDir)
        {
            try
            {
                if (Directory.Exists(targetDir))
                {
                    Directory.Delete(targetDir, true);
#if DEBUG
                    Logger.Write("Удаление каталога " + targetDir);
#endif
                }

                Directory.CreateDirectory(targetDir);

                foreach (var file in new DirectoryInfo(sourceDir).GetFiles("*.*"))
                {
                    File.Copy(file.FullName, Path.Combine(targetDir, file.Name));
                }
#if DEBUG
                Logger.Write("Копирование каталога " + sourceDir);
#endif

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return false;
            }
        }

        private void LaunchPatcher(string args)
        {
            SetStatus("Запуск...");

            var path = Path.Combine(Application.StartupPath, Resources.LauncherExe);

            try
            {
                Process.Start(path, args);
                Application.Exit();
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                SetDetails("Не удалось запустить приложение.");
                Application.Exit();
            }
        }

        private static bool Unzip(string source, string dest)
        {
            try
            {
                ZipFile.ExtractToDirectory(source, dest);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return false;
            }
        }

        private void SetStatus(string text)
        {
            //SafeControl(textBoxStatus, () => textBoxStatus.Text = text);
        }

        private void SetDetails(string text)
        {
            //SafeControl(textBoxDetails, () => textBoxDetails.Text = text);
        }

        public static void SafeControl(Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action<Control, Action>(SafeControl), control, action);
                return;
            }

            action();
        }

        private void FormUpdater_Shown(object sender, EventArgs e)
        {
            TryUpdate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // prevent key events on form

            //return base.ProcessCmdKey(ref msg, keyData);
            return true;
        }
    }
}
