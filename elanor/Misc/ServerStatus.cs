using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Xml;
using shared;

namespace Elanor.Misc
{
    public class ServerStatus
    {
        public ServerStatus(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            _worker = new BackgroundWorker();
            _worker.DoWork += delegate { UpdateAsync(); };

            _timer = new Timer(2 * 60 * 1000);
            _timer.Elapsed += delegate { if (!_worker.IsBusy) _worker.RunWorkerAsync(); };
            _timer.Start();

            Belegaer = new Server("Belegaer");
            Gwaihir = new Server("Gwaihir");
            Laurelin = new Server("Laurelin");
            Evernight = new Server("Evernight");
            Sirannon = new Server("Sirannon");
            Arkenstone = new Server("Arkenstone");
            Brandywine = new Server("Brandywine");
            Bullroarer = new Server("Bullroarer");
            Gladden = new Server("Gladden");
            Landroval = new Server("Landroval");
            Crickhollow = new Server("Crickhollow");

            _mainWindow.Dispatcher.Invoke(() => _mainWindow.UpdateServerStatusTarget());

            Servers = new List<Server>
            {
                Belegaer,
                Gwaihir,
                Laurelin,
                Evernight,
                Sirannon,
                Arkenstone,
                Brandywine,
                Bullroarer,
                Gladden,
                Landroval,
                Crickhollow
            };

            UpdateAsync();
        }

        private const string OnlineString = "offen";
        private const string OfflineString = "zu";

        private readonly MainWindow _mainWindow;

        private readonly BackgroundWorker _worker;
        private readonly Timer _timer;

        public List<Server> Servers { get; }

        public Server Belegaer { get; }

        public Server Gwaihir { get; }

        public Server Laurelin { get; }

        public Server Evernight { get; }

        public Server Sirannon { get; }

        public Server Arkenstone { get; }

        public Server Brandywine { get; }

        public Server Bullroarer { get; }

        public Server Gladden { get; }

        public Server Landroval { get; }

        public Server Crickhollow { get; }

        public async void UpdateAsync()
        {
            await Task.Run(() => Update());
        }

        private void Update()
        {
            const string url = "http://lux-hdro.de/serverstatus-rss.php";

            XmlReader reader = null;
            SyndicationFeed feed = null;

            try
            {
                reader = XmlReader.Create(url);
                feed = SyndicationFeed.Load(reader);
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
            }
            finally
            {
                reader?.Close();
            }

            if (feed != null)
            {
                foreach (var item in feed.Items)
                {
                    //var subject = item.Title.Text;
                    var summary = item.Summary.Text;
                    foreach (var server in Servers)
                    {
                        if (summary.Contains(server.Name))
                        {
                            if (summary.Contains(OnlineString))
                            {
                                server.IsOnline = true;
                                break;
                            }

                            if (summary.Contains(OfflineString))
                            {
                                server.IsOnline = false;
                                break;
                            }

                            server.IsOnline = null;
                            break;
                        }
                    }
                }
            }
            else
            {
                SetAll(null);
            }

            _mainWindow.Dispatcher.Invoke(() => _mainWindow.UpdateServerStatusTarget());
        }

        private void SetAll(bool? state)
        {
            foreach (var server in Servers)
            {
                server.IsOnline = state;
            }
        }
    }

    public class Server
    {
        public Server(string name)
        {
            _styleOnline = (Style)Application.Current.Resources["ServerStatusOnline"];
            _styleOffline = (Style)Application.Current.Resources["ServerStatusOffline"];
            _styleUnknown = (Style)Application.Current.Resources["ServerStatusUnknown"];

            IsOnline = null;
            Name = name;
        }

        private bool? _isOnline;

        public bool? IsOnline
        {
            get { return _isOnline; }
            set
            {
                _isOnline = value;
                switch (value)
                {
                    case null:
                        Style = _styleUnknown;
                        break;
                    case true:
                        Style = _styleOnline;
                        break;
                    case false:
                        Style = _styleOffline;
                        break;
                }
            }
        }

        public string Name { get; }

        public Style Style { get; private set; }

        private readonly Style _styleOnline;
        private readonly Style _styleOffline;
        private readonly Style _styleUnknown;
    }
}
