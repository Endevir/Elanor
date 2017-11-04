// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.IO;
using shared;

namespace Elanor.Misc
{
    internal class Drive
    {
        public static long GetFreeSpace(string drive)
        {
            try
            {
                foreach (var d in DriveInfo.GetDrives())
                {
                    if (string.CompareOrdinal(drive.ToLower(), d.Name.ToLower()) == 0)
                    {
                        return d.AvailableFreeSpace;
                    }
                }

                return -1;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return -1;
            }
        }

        public static bool IsEnoughSpace(string drive, long bytes)
        {
            return GetFreeSpace(drive) > bytes;
        }

        public static bool IsEnoughSpace(string drive, string file)
        {
            try
            {
                return GetFreeSpace(drive) > new FileInfo(file).Length;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return false;
            }
        }

        public static bool IsSameSize(string fileA, string fileB)
        {
            try
            {
                return new FileInfo(fileA).Length == new FileInfo(fileB).Length;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return false;
            }
        }

        public static bool IsSameSize(string fileA, long size)
        {
            try
            {
                return new FileInfo(fileA).Length == size;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return false;
            }
        }
    }
}
