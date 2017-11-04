// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Elanor.Properties;

namespace Elanor.Misc
{
    internal class Utils
    {
        public static bool PreparePatchMessage(Dictionary<Patch.Patch, int> results, out string summary)
        {
            var fails = (from dict in results where dict.Value == -1 select dict.Key.Name).ToList();
            if (fails.Count > 0)
            {
                summary = fails.Aggregate("Не удалось применить: ", (current, name) => current + name + ", ");
                summary = summary.Substring(0, summary.Length - 2) + ".";
            }
            else
            {
                summary = "Все обновления успешно применены!";
            }

            return fails.Count == 0;
        }

        public static bool DetectProcesses()
        {
            return
                Process.GetProcesses()
                    .Any(
                        proc =>
                            string.CompareOrdinal(proc.ProcessName, Resources.TurbineLauncher) == 0 ||
                            string.CompareOrdinal(proc.ProcessName, Resources.LotroClient) == 0);
        }
    }
}
