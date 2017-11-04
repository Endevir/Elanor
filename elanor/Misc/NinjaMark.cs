using System;
using Elanor.DatFile;
using shared;

namespace Elanor.Misc
{
    internal class NinjaMark
    {
        public static string CreateMark(string version, string date, bool subscribed)
        {
            return $"Ru&{version}&{date}&{subscribed}";
        }

        public static string ComposeMark(DatPatchInfo patch)
        {
            return $"Ru&{patch.Version}&{patch.Date}&{patch.IsApplySubscribed}";
        }

        public static bool DecomposeMark(string mark, ref string version, ref string date, ref bool subscribed)
        {
            try
            {
                var split = mark.Split('&');
                if (split.Length == 4 && string.CompareOrdinal(split[0], "Ru") == 0)
                {
                    version = split[1];
                    date = split[2];
                    subscribed = Convert.ToBoolean(split[3]);

                    return true;
                }

                Logger.Write("ошибка при декомпозиции метки.");
                return false;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                return false;
            }
        }
    }
}
