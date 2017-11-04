// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Elanor.Properties;
using shared;

namespace Elanor
{
    public class Setup : INotifyPropertyChanged
    {
        public bool IsValid { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<SetupPropertyChangedEventArgs> LotroDirChanged;

        protected void OnLotroDirChanged(bool success)
        {
            IsValid = success;
            LotroDirChanged?.Invoke(this, new SetupPropertyChangedEventArgs(success));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class SetupPropertyChangedEventArgs : EventArgs
        {
            public SetupPropertyChangedEventArgs(bool success)
            {
                Success = success;
            }

            public bool Success { get; }
        }

        public static string TurbineLauncherVersion { get; private set; }

        private string _lotroDir = string.Empty;

        public string LotroDir
        {
            get { return string.IsNullOrWhiteSpace(_lotroDir) ? string.Empty : _lotroDir; }
            set
            {
                _lotroDir = value;

                if (string.IsNullOrEmpty(value))
                {
                    DatFile = string.Empty;
                    OnLotroDirChanged(false);
                }
                else
                {
                    var pathDat = Path.Combine(value, Resources.DatFile);
                    var pathExe = Path.Combine(value, Resources.TurbineLauncherExe);
                    if (File.Exists(pathDat) && File.Exists(pathExe))
                    {
                        DatFile = pathDat;

                        TurbineLauncherVersion = ExecutableVersion(pathExe);

                        OnLotroDirChanged(true);
                    }
                    else
                    {
                        DatFile = string.Empty;
                        _lotroDir = string.Empty;
                        OnLotroDirChanged(false);
                        Logger.Write("Файл исходных данных игры не найден в указанном каталоге.");
                    }
                }
            }
        }

        public static bool IsDirectoryValid(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
            {
                return false;
            }

            if (File.Exists(Path.Combine(dir, Resources.DatFile)) && File.Exists(Path.Combine(dir, Resources.TurbineLauncherExe)))
            {
                return true;
            }

            return false;
        }

        public static string ExecutableVersion(string path)
        {
            var versInfo = FileVersionInfo.GetVersionInfo(path);

            return
                $"{versInfo.FileMajorPart}.{versInfo.FileMinorPart}.{versInfo.FileBuildPart}.{versInfo.FilePrivatePart}";
        }

        public string DatFile { get; private set; }

        public static Dictionary<PatchContentType, bool> Subscription => new Dictionary<PatchContentType, bool>
        {
            {PatchContentType.Text, Settings.Default.SubscribeTexts},
            {PatchContentType.Font, Settings.Default.SubscribeFonts},
            {PatchContentType.Sound, Settings.Default.SubscribeSounds},
            {PatchContentType.Video, Settings.Default.SubscribeVideos},
            {PatchContentType.Image, Settings.Default.SubscribeMaps},
            {PatchContentType.Loadscreen, Settings.Default.SubscribeLoadscreens},
            {PatchContentType.Texture, Settings.Default.SubscribeTextures},
            {PatchContentType.Undef, false}
        };
    }
}
