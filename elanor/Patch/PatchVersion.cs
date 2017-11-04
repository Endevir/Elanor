using System;
using shared;

namespace Elanor.Patch
{
    public class PatchVersion
    {
        public PatchVersion(string version)
        {
            Full = version;
        }

        public string Full { get; }

        private int this[int index]
        {
            get
            {
                var exploded = Full.Split('_');
                if (exploded.Length != 3)
                {
                    Logger.Write($"Ошибка чтения версии патча {Full}");
                    return 999;
                }

                return Convert.ToInt32(exploded[index].Substring(1, exploded[index].Length - 1).Replace(".", string.Empty));
            }
        }

        public int GameUpdate => this[1];

        public int PatchUpdate => this[2];
    }
}
