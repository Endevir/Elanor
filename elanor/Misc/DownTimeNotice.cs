using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using shared;

namespace Elanor.Misc
{
    public class DownTimeNotice
    {
        public DownTimeNotice(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            _worker = new BackgroundWorker();
            _worker.DoWork += delegate { GetNoticeAsync(); };

            _timer = new Timer(30 * 60 * 1000);
            _timer.Elapsed += delegate { if (!_worker.IsBusy) _worker.RunWorkerAsync(); };
            _timer.Start();

            DowntimeNoticeRu = string.Empty;

            _mainWindow.Dispatcher.Invoke(() => _mainWindow.UpdateDowntimeNoticeTarget());

            GetNoticeAsync();
        }

        private readonly MainWindow _mainWindow;

        private readonly BackgroundWorker _worker;
        private readonly Timer _timer;

        public string DowntimeNotice { get; private set; }

        public string DowntimeNoticeRu { get; private set; }

        private bool DispatchNotice(string notice)
        {
            if (string.IsNullOrWhiteSpace(notice))
            {
                DowntimeNoticeRu = string.Empty;
                return false;
            }

            try
            {
                // extract time
                var fromMatch = Regex.Match(notice, "([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9] ([AaPp][Mm])");
                if (!fromMatch.Success)
                {
                    return false;
                }

                if (notice.Length < fromMatch.Groups[1].Index + 15)
                {
                    return false;
                }

                var part = notice.Substring(fromMatch.Groups[1].Index + 8,
                    notice.Length - fromMatch.Groups[1].Index - 8);
                var tillMatch = Regex.Match(part, "([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9] ([AaPp][Mm])");
                if (!tillMatch.Success)
                {
                    return false;
                }

                var fromTime = fromMatch.Groups[1].Value.Length > 1 ? fromMatch.Value : "0" + fromMatch.Value;
                var tillTime = tillMatch.Groups[1].Value.Length > 1 ? tillMatch.Value : "0" + tillMatch.Value;

                fromTime = fromTime.Substring(0, 5) + ":00" + fromTime.Substring(5);
                tillTime = tillTime.Substring(0, 5) + ":00" + tillTime.Substring(5);

                // extract date
                var dateMatch = Regex.Match(notice, "[A-Za-z]+ [0-9]+[a-z]+");
                if (!dateMatch.Success)
                {
                    return false;
                }

                var date = dateMatch.Value;
                var month = date.Substring(0, 3);
                var dayMatch = Regex.Match(date, "[0-9]+");
                var day = dayMatch.Value;
                var year = DateTime.Now.Year;

                if (day.Length == 1)
                {
                    day = "0" + day;
                }

                // create DateTime from extracted values
                var dtFrom = DateTime.ParseExact($"{year}-{month}-{day} {fromTime}-04:00", "yyyy-MMM-dd hh:mm:ss ttzzzzz", CultureInfo.InvariantCulture);
                var dtTill = DateTime.ParseExact($"{year}-{month}-{day} {tillTime}-04:00", "yyyy-MMM-dd hh:mm:ss ttzzzzz", CultureInfo.InvariantCulture);

                // create app notice if forum notice is not in the past
                if (dtTill > DateTime.Now)
                {
                    DowntimeNoticeRu = dtFrom.Day == dtTill.Day
                        ? $"Внимание! Плановая профилактика игровых серверов {dtFrom:M} c {dtFrom:t} до {dtTill:t} (МСК)."
                        : $"Внимание! Плановая профилактика игровых серверов с {dtFrom:t} {dtFrom:M} до {dtTill:t} {dtTill:M} (МСК).";

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                DowntimeNoticeRu = string.Empty;

                return false;
            }
        }

        public async Task GetNoticeAsync()
        {
            var content = await Task.Run(() => ReadContent());

            bool extract;
            var position = 0;
            string result;
            int index;

            do
            {
                extract = ExtractChunk(content.Substring(position), "Downtime Notice:", out result, out index);
                position += index + 10;
            }
            while (!extract && position < content.Length && index != -1);

            DowntimeNotice = result;

            if (!DispatchNotice(result))
            {
                position = 0;

                do
                {
                    extract = ExtractChunk(content.Substring(position), "Server Restart", out result, out index);
                    position += index + 10;
                }
                while (!extract && position < content.Length && index != -1);

                DispatchNotice(result);
                DowntimeNotice = result;
            }

            _mainWindow.Dispatcher.Invoke(() => _mainWindow.UpdateDowntimeNoticeTarget());
        }

        private static bool ExtractChunk(string source, string magicWord, out string result, out int index)
        {
            result = string.Empty;

            // get downtime notice
            index = source.IndexOf(magicWord, StringComparison.Ordinal);
            if (index < 50)
            {
                return false;
            }

            // ignore store downtime notice
            var chunkStore = source.Substring(index - 40, 100).ToLowerInvariant();
            if (chunkStore.IndexOf("store", StringComparison.Ordinal) != -1)
            {
                return false;
            }

            // cut extra symbols
            var chunk = source.Substring(index, 100);
            var endPos = chunk.IndexOf("<", StringComparison.Ordinal);
            if (endPos < 20)
            {
                return false;
            }

            result = chunk.Substring(0, endPos);

            return true;
        }

        private static string ReadContent()
        {
            try
            {
                const string url = "https://www.lotro.com/forums/forumdisplay.php?3-Announcements";
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
