// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Linq;
using Microsoft.Win32;
using shared;

namespace Elanor.Misc
{
    internal class Registry
    {
        public static string LookupDisplayName(string branch, string target)
        {
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                    Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
                {
                    using (var key = baseKey.OpenSubKey(branch, RegistryKeyPermissionCheck.ReadSubTree))
                    {
                        if (key == null)
                        {
                            return null;
                        }

                        if (key.GetSubKeyNames().Contains(target))
                        {
                            using (var subkey = key.OpenSubKey(target, RegistryKeyPermissionCheck.ReadSubTree))
                            {
                                var val = subkey?.GetValue("InstallLocation");
                                return val?.ToString();
                            }
                        }
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                return null;
            }
        }
    }
}
