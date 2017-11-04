// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.IO;
using Elanor.Properties;
using shared;

namespace Elanor.Patch
{
    public class Patch
    {
        public Patch(PatchContentType contentType)
        {
            ContentType = contentType;
            PatchVersion = new PatchVersion("null_U1_v100");
        }

        public Patch(string path)
        {
            Path = path;
        }

        public string Path { get; set; }

        // patch_metadata section

        public string Name { get; set; }

        public string Description { get; set; }

        public string Date { get; set; }

        public string Author { get; set; }

        public PatchVersion PatchVersion { get; set; }

        public string Link { get; set; }

        public PatchContentType ContentType { get; set; }

        // patch_metadata section end

        public List<int> ClientDatRelated { get; set; }

        public string RequestContentType => ContentType.ToString().ToLowerInvariant();

        public bool IsValid { get; set; }

        public bool IsDownloaded => !string.IsNullOrWhiteSpace(Path) && IsValid;

        public bool IsDownloadSubscribed
        {
            get
            {
                switch (ContentType)
                {
                    case PatchContentType.Text:
                        return Settings.Default.SubscribeTexts;
                    case PatchContentType.Image:
                        return Settings.Default.SubscribeMaps;
                    case PatchContentType.Sound:
                        return Settings.Default.SubscribeSounds;
                    case PatchContentType.Font:
                        return Settings.Default.SubscribeFonts;
                    case PatchContentType.Loadscreen:
                        return Settings.Default.SubscribeLoadscreens;
                    case PatchContentType.Video:
                        return Settings.Default.SubscribeVideos;
                    case PatchContentType.Texture:
                        return Settings.Default.SubscribeTextures;
                    default:
                        return false;
                }
            }
        }

        public bool IsMatch(string version)
        {
            return string.CompareOrdinal(PatchVersion.Full, version) == 0;
        }

        public bool IsHigherThan(string version)
        {
            var compared = new PatchVersion(version);
            return PatchVersion.GameUpdate > compared.GameUpdate ||
                   PatchVersion.PatchUpdate > compared.PatchUpdate;
        }

        public bool IsHigherThan(Patch patch)
        {
            return IsHigherThan(patch.PatchVersion.Full);
        }

        public void Delete()
        {
            try
            {
                File.SetAttributes(Path, FileAttributes.Normal);
                File.Delete(Path);
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
            }
        }

        public static PatchContentType TypeFromName(string name)
        {
            var split = name.Split('_');
            if (split.Length != 3 || split[0].Length < 3)
            {
                throw new ArgumentException($"ошибка в типе данных патча: {name}.");
            }

            var typeName = split[0].ToLowerInvariant();
            switch (typeName)
            {
                case "texts":
                    return PatchContentType.Text;
                case "fonts":
                    return PatchContentType.Font;
                case "loadscreens":
                    return PatchContentType.Loadscreen;
                case "videos":
                    return PatchContentType.Video;
                case "sounds":
                    return PatchContentType.Sound;
                case "maps":
                case "images":
                    return PatchContentType.Image;
                case "dds":
                case "texture":
                    return PatchContentType.Texture;
                default:
                    throw new ArgumentException($"неопознанный тип данных патча: {typeName}.");
            }
        }
    }
}
