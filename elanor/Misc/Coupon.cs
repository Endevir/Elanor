using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using shared;

namespace Elanor.Misc
{
    public class Coupon
    {
        public Coupon(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            _worker = new BackgroundWorker();
            _worker.DoWork += delegate { GetCouponAsync(); };

            _timer = new Timer(60 * 60 * 1000);
            _timer.Elapsed += delegate { if (!_worker.IsBusy) _worker.RunWorkerAsync(); };
            _timer.Start();

            CouponCode = string.Empty;

            _mainWindow.Dispatcher.Invoke(() => _mainWindow.UpdateCouponCodeTarget());

            GetCouponAsync();
        }

        private readonly MainWindow _mainWindow;

        private readonly BackgroundWorker _worker;
        private readonly Timer _timer;

        public string CouponCode { get; private set; }

        public async Task GetCouponAsync()
        {
            CouponCode = "Ищем купон...";
            // get "issue" link
            var articles = await Task.Run(() => ReadContent("https://www.lotro.com/en/game/articles"));
            var issue = ExtractLink(articles, "The LOTRO Beacon: Issue ");

            var coupon = "";
            while (coupon == "")
            {
                // get coupon itself
                var content = await Task.Run(() => ReadContent(issue));
                coupon = ExtractCoupon(content, "Coupon Code <strong>");
            }

            CouponCode = coupon;

            _mainWindow.Dispatcher.Invoke(() => _mainWindow.UpdateCouponCodeTarget());
        }

        private static string ExtractLink(string source, string magicWord)
        {
            var index = source.IndexOf(magicWord, StringComparison.Ordinal);
            if (index < 100)
            {
                return string.Empty;
            }

            index += magicWord.Length;

            var issue = string.Empty;
            while (char.IsDigit(source[index]))
            {
                issue += source[index++];
            }

            if (string.IsNullOrWhiteSpace(issue))
            {
                return string.Empty;
            }

            return "https://www.lotro.com/en/game/articles/lotro-beacon-issue-" + issue;
        }

        private static string ExtractCoupon(string source, string magicWord)
        {
            var index = source.IndexOf(magicWord, StringComparison.Ordinal);
            if (index < 100)
            {
                return string.Empty;
            }

            index += magicWord.Length;

            var coupon = string.Empty;
            while (char.IsLetter(source[index]))
            {
                coupon += source[index++];
            }

            return coupon;
        }

        private static string ReadContent(string url)
        {
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
                    wc.Proxy = null;
                    wc.Credentials = CredentialCache.DefaultCredentials;

                    return wc.DownloadString(url);
                }
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                return string.Empty;
            }
        }
    }
}
