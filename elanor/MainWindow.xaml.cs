using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Elanor.Properties;
using Elanor.Misc;
using Elanor.Patch;
using shared;
using WPFFolderBrowser;
using Application = System.Windows.Application;
using ButtonType = Elanor.Misc.ButtonType;
using Timer = System.Threading.Timer;

namespace Elanor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            ServicePointManager.DefaultConnectionLimit = 40;
            _message = new Messaging(this);

            AddBinding();
            DisplayVersion();
            UpgradeSettings();
            CleanLog();

            ServerStatus = new ServerStatus(this);
            DowntimeNotice = new DownTimeNotice(this);
            CouponCode = new Coupon(this);
        }

        private readonly Setup _setup = new Setup();
        private readonly PatchManager _patch = new PatchManager();
        private readonly Messaging _message;       
        public SwapManager SwapManager;

        private bool _locked;

        public ServerStatus ServerStatus { get; }
        public DownTimeNotice DowntimeNotice { get; }
        public Coupon CouponCode { get; }

        public void Lock()
        {
            Dispatcher.Invoke(() =>
            {
                if (Locker.Visibility == Visibility.Visible)
                {
                    return;
                }

                Locker.Visibility = Visibility.Visible;
                TextBlockLock.Visibility = Visibility.Visible;
                CouponButton.Visibility = Visibility.Collapsed;

                _locked = true;
            });
        }

        private void Unlock()
        {
            Dispatcher.Invoke(() =>
            {
                Locker.Visibility = Visibility.Collapsed;
                TextBlockLock.Visibility = Visibility.Collapsed;
                CouponButton.Visibility = Visibility.Visible;

                _locked = false;
            });
        }

        private void UpdateTip(object obj)
        {
            Dispatcher.Invoke(() => TextBlockLock.Text =
                StaticData.PleaseWait[new Random().Next(0, StaticData.PleaseWait.Length)]);
        }

        private static void CleanLog()
        {
            const string log = "log.txt";
            if (File.Exists(log))
            {
                var fi = new FileInfo(log);
                if (fi.Length > 5 * 1024 * 1024)
                {
                    try
                    {
                        File.Delete(log);
                        Logger.Write("размер лога превысил лимит, очищаю.");
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex.Message);
                    }                    
                }
            }
        }

        public void LoadPatchGridData()
        {
            var patches = SwapManager.EntityA.DatController.DatPatches;

            LabelTexts.Content = patches[PatchContentType.Text].ViewDate;
            LabelFonts.Content = patches[PatchContentType.Font].ViewDate;
            LabelSounds.Content = patches[PatchContentType.Sound].ViewDate;
            LabelMaps.Content = patches[PatchContentType.Image].ViewDate;
            LabelLoadscreens.Content = patches[PatchContentType.Loadscreen].ViewDate;
            LabelVideos.Content = patches[PatchContentType.Video].ViewDate;
        }

        private void AddBinding()
        {
            // dev menu binding
            var b = new KeyBinding
            {
                Command = new ActionCommand(async () =>
                {
                    if (!_locked)
                    {
                        await _message.ShowDevDialog(this, SwapManager);
                    }                   
                }),
                Modifiers = ModifierKeys.Control,
                Key = Key.F5
            };
            InputBindings.Add(b);

            // swap menu binding
            // Deprecated due to fucking useless
            var b2 = new KeyBinding
            {
                Command = new ActionCommand(async () =>
                {
                    if (!_locked)
                    {
                        _message.ShowMonolog(MessageType.Green, "В текущей версии меню переключения файлов локализации недоступно. Для выбора устанавливаемых патчей пользуйтесь кнопкой \"Настройки\"\n\n Нажмите на сообщение, чтобы закрыть его.");
                        //await _message.ShowSwapDialogAsync(this, SwapManager, false);
                    }
                }),
                Modifiers = ModifierKeys.Control,
                Key = Key.F6
            };
            InputBindings.Add(b2);
            
        }

        private void DisplayVersion()
        {
            TextBlockVersion.Text = "v." + Assembly.GetExecutingAssembly().GetName().Version;
        }

        private static void UpgradeSettings()
        {
            if (Settings.Default.SettingsUpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.SettingsUpgradeRequired = false;
                Settings.Default.Save();
            }
        }

        private void DownloadAsync(string uri)
        {
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
                    wc.Proxy = null;
                    wc.Credentials = CredentialCache.DefaultCredentials;

                    long size;
                    using (wc.OpenRead(uri))
                    {
                        size = Convert.ToInt64(wc.ResponseHeaders["Content-Length"]);
                    }

                    if (!Drive.IsEnoughSpace(_setup.LotroDir.Substring(0, 3), size))
                    {
                        Logger.Write("Недостаточно места на диске.");
                        return;
                    }

                    var notifier = new AutoResetEvent(false);
                    var filename = Path.GetFileName(uri);
                    var path = Properties.Resources.DownloadsPath + $@"\{filename}";
                    var contentType = Patch.Patch.TypeFromName(filename);

                    wc.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                    {
                        var bytesIn = double.Parse(e.BytesReceived.ToString());
                        var totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                        var percentage = int.Parse(Math.Truncate(bytesIn / totalBytes * 100).ToString(CultureInfo.InvariantCulture));
                        var mBytesIn = bytesIn / 1048576;
                        var mBytesTotal = totalBytes / 1048576;

                        SetPatchInfo(contentType, $"Загрузка: {mBytesIn:F2}/{mBytesTotal:F2} Мб ( {percentage}% )");
                    };

                    wc.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
                    {
                        if (e?.Error != null)
                        {
                            Logger.Write(e.Error.Message);
                        }

                        notifier.Set();
                    };

                    wc.DownloadFileAsync(new Uri(uri), path);

                    notifier.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
        }

        private void SetPatchInfo(PatchContentType contentType, string text)
        {
            Dispatcher.Invoke(() =>
            {
                switch (contentType)
                {
                    case PatchContentType.Text:
                        LabelTexts.Content = text;
                        break;
                    case PatchContentType.Image:
                        LabelMaps.Content = text;
                        break;
                    case PatchContentType.Sound:
                        LabelSounds.Content = text;
                        break;
                    case PatchContentType.Font:
                        LabelFonts.Content = text;
                        break;
                    case PatchContentType.Loadscreen:
                        LabelLoadscreens.Content = text;
                        break;
                    case PatchContentType.Video:
                        LabelVideos.Content = text;
                        break;
                }
            });
        }

        public async Task RoutineAsync()
        {
            using (new Timer(UpdateTip, null, 0, 20000))
            {
                Lock();

                _patch.Scan();

                if (!SwapManager.EntityA.IsInit)
                {
                    await SwapManager.EntityA.InitAsync();
                }

                if (SwapManager.EntityA.IsInit)
                {
                    // import default apply subscription from dl subscription
                    //if (SwapManager.EntityB == null && !SwapManager.EntityA.DatController.IsSubscriptionNotEmpty)
                    //if (!SwapManager.EntityA.DatController.IsSubscriptionNotEmpty)
                    //{
                        var dls = Setup.Subscription;

                        foreach (var dict in SwapManager.EntityA.DatController.DatPatches)
                        {
                            dict.Value.IsApplySubscribed = dls[dict.Key];
                        }
                    //}

                    await RequestPatchesAsync();
                    await ApplyPatchesAsync();

                    LoadPatchGridData();
                }
                else
                {
                    Unlock();

                    _message.ShowMonolog(MessageType.Red, StaticData.DatOpenError, StaticData.Error);

                    Application.Current.Shutdown();
                }
            }
        }

        public async Task ApplyPatchesAsync()
        {
            var processor = new PatchProcessor();
            processor.PatchProgressChanged += delegate (object sender, PatchProcessor.PatchProgressEventArgs e)
            {
                var percentage = int.Parse(Math.Truncate((double)e.Current / e.All * 100).ToString(CultureInfo.InvariantCulture));
                SetPatchInfo(e.ContentType, $"Применение: {percentage}%");
            };

            processor.PatchApplied += delegate (object sender, PatchProcessor.PatchAppliedEventArgs e)
            {
                SetPatchInfo(e.ContentType, e.Errors < 0 ? "Неудача" : "Успех");
            };

            var results = await processor.PatchAsync(SwapManager.EntityA.DatController, _patch.Patches.Values);
            _patch.Scan();

            Unlock();
            Dispatcher.Invoke(() =>
            {
                if (results.Count > 0)
                {
                    if (results.Any(x => x.Value == -2))
                    {
                        _message.ShowMonolog(MessageType.Red, StaticData.OldClientDat);
                        Application.Current.Shutdown();
                        return;
                    }

                    string summary;
                    _message.ShowMonolog(
                        !Utils.PreparePatchMessage(results, out summary) ? MessageType.Red : MessageType.Blue, summary,
                        "Отчёт");
                }
            });
        }

        private async Task CheckForUnCatchedUpdates()
        {
            try
            {
                // prepare existing patch list
                var ex = _patch.Patches.ToDictionary(dict => dict.Value.RequestContentType,
                    dict => dict.Value.IsDownloadSubscribed ? "999" : dict.Value.PatchVersion.PatchUpdate.ToString());

                // prepare GET request
                var baseUrl = Settings.Default.UrlPatchRequest;
                var baseDomain = Settings.Default.UrlBaseDomain;
                var url = string.Format(baseUrl + "?{0}", string.Join("&", ex.Select(x => $"{x.Key}={x.Value}")));
                // fire the request
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(responseString))
                    {
                        TextBlockUpdates.Text = "";
                        return;
                    } else
                    {
                        TextBlockUpdates.Text = StaticData.UncatchedUpdates;
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
        }

        private async Task<bool> CheckForCatchedUpdates()
        {
            try
            {
                // prepare existing patch list
                var ex = _patch.Patches.ToDictionary(dict => dict.Value.RequestContentType,
                    dict => dict.Value.IsDownloadSubscribed ? dict.Value.PatchVersion.PatchUpdate.ToString() : "999");

                // prepare GET request
                var baseUrl = Settings.Default.UrlPatchRequest;
                var baseDomain = Settings.Default.UrlBaseDomain;
                var url = string.Format(baseUrl + "?{0}", string.Join("&", ex.Select(x => $"{x.Key}={x.Value}")));

                // fire the request
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(responseString))
                    {
                        return false;
                    }
                    if (_message.ShowDialog(MessageType.White, StaticData.NewPatchMessage,
                        StaticData.NewPatchMessageCaption, ButtonType.YesNo) == ResultType.Yes)
                    {
                        _message.ShowSetupDialog();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return true;
            }
        }

        private async Task RequestPatchesAsync()
        {
            try
            {
                await CheckForCatchedUpdates();
                await CheckForUnCatchedUpdates();

                //if (await CheckForCatchedUpdates() == false)
                //    return;
                // prepare existing patch list
                var ex = _patch.Patches.ToDictionary(dict => dict.Value.RequestContentType,
                    dict => dict.Value.IsDownloadSubscribed ? dict.Value.PatchVersion.PatchUpdate.ToString() : "999");

                // prepare GET request
                var baseUrl = Settings.Default.UrlPatchRequest;
                var baseDomain = Settings.Default.UrlBaseDomain;
                var url = string.Format(baseUrl + "?{0}", string.Join("&", ex.Select(x => $"{x.Key}={x.Value}")));

                // fire the request
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);

                    response.EnsureSuccessStatusCode();
                
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(responseString))
                    {
                        return;
                    }

                    if (!Settings.Default.MultithreadedDownload)
                    {
                        foreach (var uri in responseString.Split('|'))
                        {
                            await Task.Run(() => DownloadAsync(baseDomain + uri));
                        }
                    }
                    else
                    {
                        var tasks = new List<Task>();
                        foreach (var uri in responseString.Split('|'))
                        {
                            var task = new Task(() => DownloadAsync(baseDomain + uri));
                            tasks.Add(task);
                            task.Start();
                        }

                        await Task.WhenAll(tasks);
                    }
                }

                _patch.Scan();
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
        }

        private void ShowUpdateNotes()
        {
            // if just updated, show update notes if some
            if (Environment.GetCommandLineArgs().Contains("-update"))
            {
                if (_message.ShowDialog(MessageType.Blue, StaticData.AppUpdatedText, StaticData.AppUpdatedCaption, ButtonType.YesNo) == ResultType.Yes)
                {
                    if (File.Exists("whatsnew.txt"))
                    {
                        try
                        {
                            Process.Start("whatsnew.txt");
                        }
                        catch (Exception ex)
                        {
                            Logger.Write(ex.Message);
                        }
                    }                   
                }
            }
        }

        private void LaunchLotro(bool withArgs)
        {
            var path = Path.Combine(_setup.LotroDir, Properties.Resources.TurbineLauncherExe);
            var args = withArgs ? Properties.Resources.TurbineLauncherArgs : string.Empty;

            try
            {
                Process.Start(path, args);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
        }

        private async void ButtonGame_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_locked && _setup.IsValid)
            {

                LaunchLotro(true);
                //if (SwapManager.EntityB != null && SwapManager.EntityB.IsValid)
                //{
                //if (await _message.ShowSwapDialogAsync(this, SwapManager, true))
                //{
                //LaunchLotro(true);
                // }
                    return;
            }
        }

        private void ButtonAbout_OnClick(object sender, RoutedEventArgs e)
        {
            _message.ShowAboutDialog();
        }

        private void ButtonBug_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Settings.Default.UrlBug);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
        }

        private void ButtonManual_OnClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists("readme.txt"))
            {
                try
                {
                    Process.Start("readme.txt");
                }
                catch (Exception ex)
                {
                    Logger.Write(ex.Message);
                }                
            }
        }

        private void ButtonSetup_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_locked)
            {
                _message.ShowSetupDialog(this);
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Settings.Default.Save();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            PatchContentType contentType;
            Enum.TryParse(e.Uri.ToString(), out contentType);
            _message.ShowPatchInfoDialog(_patch.Patches[contentType]);
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            // update visual state of server notifiers
            UpdateDowntimeNoticeTarget();
            UpdateServerStatusTarget();
            UpdateCouponCodeTarget();

            // show update notes if some
            ShowUpdateNotes();

            // minor fool's protection
            if (!CloseYourClient())
            {
                Application.Current.Shutdown();
                return;
            }

            // display first launch greetings
            Greetings();

            // recreate downloads directory
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Properties.Resources.DownloadsPath));

            // update lotro dir status
            _setup.LotroDir = Settings.Default.LotroPath;
            _setup.LotroDirChanged += delegate
            {
                Settings.Default.LotroPath = _setup.LotroDir;
            };

            // lotro dir lookup
            if (!WhereIsLotro())
            {
                Application.Current.Shutdown();
                return;
            }

            // immediate or delayed update check
            AreYouAdvanced();

            // 
            InitSwapManager();

            // run update check and apply
            await RoutineAsync();
        }

        private void InitSwapManager()
        {
            SwapManager = new SwapManager(_setup.DatFile, Settings.Default.ReserveDat);
        }

        private bool WhereIsLotro()
        {
            if (_setup.IsValid)
            {
                return true;
            }

            // trysearch registry
            var dir = Searcher.LotroDirLookup();
            var valid = Setup.IsDirectoryValid(dir);
            if (valid && _message.ShowDialog(MessageType.White, string.Format(StaticData.LotroFound, dir), "", ButtonType.YesNo) == ResultType.Yes)
            {
                _setup.LotroDir = dir;
                return true;
            }

            // if nothing found in registry or value found is corrupt or another dir required
            // ask for manual input
            _message.ShowMonolog(MessageType.White, StaticData.LotroDirLookupFailed);

            var dlg = new WPFFolderBrowserDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
            };

            // nothing to do untill we have the path
            do
            {
                var pathWasSelected = dlg.ShowDialog(this);
                if (pathWasSelected.HasValue)
                {
                    if (pathWasSelected.Value)
                    {
                        _setup.LotroDir = dlg.FileName;
                    }
                    else
                    {
                        Logger.Write("не указан каталог игры, завершение работы.");
                        return false;
                    }
                }
                else
                {
                    Logger.Write("не удалось вызвать диалоговое окно выбора каталога игры.");
                    return false;
                }
            } while (string.IsNullOrEmpty(_setup.LotroDir));

            return true;
        }

        private bool CloseYourClient()
        {
            // force user to close running lotro processes
            while (Utils.DetectProcesses())
            {
                if (_message.ShowDialog(MessageType.Red, StaticData.CloseApp, StaticData.CloseAppCaption, ButtonType.DoneExit) == ResultType.Exit)
                {
                    return false;
                }
            }

            return true;
        }

        private void Greetings()
        {
            // display first launch greetings
            if (Settings.Default.IsFirstRun)
            {
                _message.ShowMonolog(MessageType.Green, StaticData.FirstLaunch.ToList(), "");
            }
        }

        private void AreYouAdvanced()
        {
            // ask user if he's skilled enough to change some settings first
            if (Settings.Default.IsFirstRun)
            {
                    _message.ShowMonolog(MessageType.White, StaticData.AreYouSkilled, StaticData.AreYouSkilledCaption);
                    //if (_message.ShowDialog(MessageType.White, StaticData.AreYouSkilled, 
                    //    StaticData.AreYouSkilledCaption, ButtonType.YesNo) == ResultType.Yes)
                    //{
                    // Deleted because of showing SetupDialog before every Update
                    //_message.ShowSetupDialog();
               // }
                Settings.Default.IsFirstRun = false;
            }
        }

        private void LabelClose_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LabelMinimize_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void UpdateServerStatusTarget()
        {
            TextBlockServerBelegaer.GetBindingExpression(StyleProperty)?.UpdateTarget();
            TextBlockServerGwaihir.GetBindingExpression(StyleProperty)?.UpdateTarget();
            TextBlockServerLaurelin.GetBindingExpression(StyleProperty)?.UpdateTarget();
            TextBlockServerEvernight.GetBindingExpression(StyleProperty)?.UpdateTarget();
            TextBlockServerSirannon.GetBindingExpression(StyleProperty)?.UpdateTarget();
            TextBlockServerArkenstone.GetBindingExpression(StyleProperty)?.UpdateTarget();
            TextBlockServerBrandywine.GetBindingExpression(StyleProperty)?.UpdateTarget();
            TextBlockServerBullroarer.GetBindingExpression(StyleProperty)?.UpdateTarget();
            TextBlockServerGladden.GetBindingExpression(StyleProperty)?.UpdateTarget();
            TextBlockServerLandroval.GetBindingExpression(StyleProperty)?.UpdateTarget();
            TextBlockServerCrickhollow.GetBindingExpression(StyleProperty)?.UpdateTarget();
        }

        public void UpdateDowntimeNoticeTarget()
        {
            TextBlockNotice.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
        }

        public void UpdateCouponCodeTarget()
        {
            CouponText.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr CloseClipboard();

        private void CouponButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseClipboard();
                Clipboard.SetText(CouponText.Text, TextDataFormat.Text);
            }
            catch (Exception exception)
            {
                Logger.Write(exception.Message);
            }            
        }
    }
}
