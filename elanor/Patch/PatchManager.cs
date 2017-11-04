// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Elanor.Properties;
using Elanor.Misc;
using shared;

namespace Elanor.Patch
{
    internal class PatchManager
    {
        public PatchManager()
        {
            Reset();
        }

        public Dictionary<PatchContentType, Patch> Patches { get; private set; }

        private void Reset()
        {
            Patches = new Dictionary<PatchContentType, Patch>();
            foreach (var contentType in EnumHelper<PatchContentType>.GetValues(PatchContentType.Video))
            {
                Patches.Add(contentType, new Patch(contentType));
            }
        }

        public bool Scan()
        {
            try
            {
                Reset();
                foreach (var file in new DirectoryInfo(Resources.DownloadsPath).GetFiles("*.db"))
                {
                    AcquirePatchInfo(file.FullName);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return false;
            }
        }

        private void AcquirePatchInfo(string path)
        {
            try
            {
                var patch = new Patch(path);
                using (var con = new SQLiteConnection($"Data Source = {path}; Version = 3;"))
                {
                    con.Open();
                    
                    patch.IsValid = Database.TableExists(con, DatabaseShared.PatchDataTable) &&
                                    Database.TableExists(con, DatabaseShared.PatchMetadataTable) &&
                                    AcquireMetadata(ref patch, con);

                    if (patch.IsValid)
                    {
                        patch.ClientDatRelated = new List<int> { 0 };//Database.GetClientDatList(con);
                    }
                }

                if (patch.IsValid)
                {
                    patch.Path = path;

                    if (Patches[patch.ContentType].IsDownloaded)
                    {
                        if (patch.IsHigherThan(Patches[patch.ContentType]))
                        {
                            if (Settings.Default.AutoClean)
                            {
                                Logger.Write($"патч {Patches[patch.ContentType].Path} устарел, удаляю.");
                                Patches[patch.ContentType].Delete();
                            }

                            Patches[patch.ContentType] = patch;
                        }
                        else
                        {
                            if (Settings.Default.AutoClean)
                            {
                                Logger.Write($"патч {patch.Path} устарел, удаляю.");
                                patch.Delete();
                            }
                        }
                    }
                    else
                    {
                        Patches[patch.ContentType] = patch;
                    }                 
                }
                else
                {
                    Logger.Write($"патч {Path.GetFileName(path)} повреждён, удаляю.");
                    patch.Delete();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
        }

        private static bool AcquireMetadata(ref Patch patch, SQLiteConnection con)
        {
            try
            {
                var cmdText = $"SELECT * FROM `{DatabaseShared.PatchMetadataTable}` LIMIT 1;";
                using (var cmd = new SQLiteCommand(cmdText, con))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            patch.Name = rdr.GetString(0);
                            patch.Description = rdr.GetString(1);
                            patch.Date = rdr.GetString(2);
                            patch.Author = rdr.GetString(3);
                            patch.PatchVersion = new PatchVersion(rdr.GetString(4));
                            patch.Link = rdr.GetString(5);

                            var cType = rdr.GetString(6);
                            switch (cType)
                            {
                                case "[map]":
                                case "[image]":
                                    patch.ContentType = PatchContentType.Image;
                                    break;
                                case "[text]":
                                    patch.ContentType = PatchContentType.Text;
                                    break;
                                case "[sound]":
                                    patch.ContentType = PatchContentType.Sound;
                                    break;
                                case "[font]":
                                    patch.ContentType = PatchContentType.Font;
                                    break;
                                case "[loadscreen]":
                                    patch.ContentType = PatchContentType.Loadscreen;
                                    break;
                                case "[video]":
                                    patch.ContentType = PatchContentType.Video;
                                    break;
                                case "[dds]":
                                case "[texture]":
                                    patch.ContentType = PatchContentType.Texture;
                                    break;
                                default:
                                    throw new ArgumentException($"тип файлов {cType} не обрабатывается программой.");
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return false;
            }
        }
    }
}
