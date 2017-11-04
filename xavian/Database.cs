using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using shared;

namespace Xavian
{
    internal class Database
    {
        private static readonly string CreateBinaryTableString = $"CREATE TABLE `{DatabaseShared.PatchDataTable}` (" +
                                                                "`file_id` INTEGER NOT NULL DEFAULT '0', " +
                                                                "`data` BLOB, " +
                                                                "`dat_id` INTEGER NOT NULL DEFAULT '-1', " +
                                                                "PRIMARY KEY (`file_id`));";

        private static readonly string CreateTextTableString = $"CREATE TABLE `{DatabaseShared.PatchDataTable}` (" +
                                                              "`file_id` INTEGER NOT NULL DEFAULT '0', " +
                                                              "`gossip_id` INTEGER NOT NULL DEFAULT '0', " +
                                                              "`content` TEXT, " +
                                                              "`args` TEXT, " +
                                                              "`dat_id` INTEGER NOT NULL DEFAULT '-1', " +
                                                              "PRIMARY KEY (`file_id`, `gossip_id`));";

        private static readonly string CreateMetadataTableString = $"CREATE TABLE `{DatabaseShared.PatchMetadataTable}` (" +
                                                         "`name` TEXT NOT NULL, " +
                                                         "`description` TEXT NOT NULL, " +
                                                         "`date` TEXT NOT NULL, " +
                                                         "`author` TEXT NOT NULL, " +
                                                         "`version` TEXT NOT NULL, " +
                                                         "`link` TEXT NOT NULL, " +
                                                         "`content_type` TEXT NOT NULL, " +
                                                         "PRIMARY KEY (`version`));";

        public static Dictionary<PatchContentType, DatabaseInfo> DatabaseInfoStore =
            new Dictionary<PatchContentType, DatabaseInfo>
            {
                {
                    PatchContentType.Text,
                    new DatabaseInfo(CreateTextTableString, DatabaseShared.PatchDataTable, "texts",
                        "Оригинальные тексты", "Тексты, извлечённые из оригинального файла ресурсов игры.", "[text]",
                        "oTexts_U21.1_v1.0.0")
                },
                {
                    PatchContentType.Sound,
                    new DatabaseInfo(CreateBinaryTableString, DatabaseShared.PatchDataTable, "sounds",
                        "Оригинальная озвучка", "Озвучка, извлечённая из оригинального файла ресурсов игры.", "[sound]",
                        "oSounds_U21.1_v1.0.0")
                },
                {
                    PatchContentType.Image,
                    new DatabaseInfo(CreateBinaryTableString, DatabaseShared.PatchDataTable, "images",
                        "Оригинальные изображения", "Изображения, извлечённые из оригинального файла ресурсов игры.",
                        "[image]", "oImages_U21.1_v1.0.0")
                },
                {
                    PatchContentType.Font,
                    new DatabaseInfo(CreateBinaryTableString, DatabaseShared.PatchDataTable, "fonts",
                        "Оригинальные шрифты", "Шрифты, извлечённые из оригинального файла ресурсов игры.", "[font]",
                        "oFonts_U21.1_v1.0.0")
                },
                {
                    PatchContentType.Texture,
                    new DatabaseInfo(CreateBinaryTableString, DatabaseShared.PatchDataTable, "textures",
                        "Оригинальные текстуры", "Текстуры, извлечённые из оригинального файла ресурсов игры.", "[dds]",
                        "oTextures_U21.1_v1.0.0")
                }
            };

        public static void CreateDatabase(PatchContentType contentType, string dir, string name)
        {
            try
            {
                var di = DatabaseInfoStore[contentType];
                
                di.DatabaseName = $"{dir}\\{name}.db";

                SQLiteConnection.CreateFile($"{di.DatabaseName}");

                using (var mDbConnection = new SQLiteConnection($"Data Source={di.DatabaseName};Version=3;"))
                {
                    mDbConnection.Open();

                    var cmdText = string.Format(di.CreateString, di.DataTableName);
                    using (var command = new SQLiteCommand(cmdText, mDbConnection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (var command = new SQLiteCommand(CreateMetadataTableString, mDbConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
        }

        public static void PutMetadata(PatchContentType contentType)
        {
            try
            {
                var di = DatabaseInfoStore[contentType];

                using (var mDbConnection = new SQLiteConnection($"Data Source={di.DatabaseName};Version=3;"))
                {
                    mDbConnection.Open();

                    var cmdText =
                        $"INSERT INTO `{DatabaseShared.PatchMetadataTable}` VALUES (@name, @descr, '', '', @version, '', @contentType);";

                    using (var cmd = new SQLiteCommand(cmdText, mDbConnection))
                    {
                        cmd.Parameters.AddWithValue("@name", di.PatchName);
                        cmd.Parameters.AddWithValue("@descr", di.PatchDescr);
                        cmd.Parameters.AddWithValue("@version", di.Version);
                        cmd.Parameters.AddWithValue("@contentType", di.ContentType);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
        }

        private static readonly object ResLockObj = new object();

        public static Dictionary<PatchContentType, int> Import(DatFile dat, ExtractMarker extractMarker, string datname)
        {
            Logger.Write("инициализация извлечения.");

            var cons = new Dictionary<PatchContentType, SQLiteConnection>();
            var ins = new Dictionary<PatchContentType, Inserter>();
            var results = new Dictionary<PatchContentType, int>();
            var pathes = new Dictionary<PatchContentType, string>();
            var filenames = new Dictionary<PatchContentType, string>();

            var datetime = $"{DateTime.Now:yy-MM-dd-HH-mm-ss}";

            foreach (var type in extractMarker.Overall)
            {
                filenames.Add(type, $"{DatabaseInfoStore[type].DefaultDatabaseName}_U21_v1.0.0");
            }

            var dir = Directory.CreateDirectory($"{datname}_{datetime}");

            foreach (var type in extractMarker.ToDatabase)
            {
                var di = DatabaseInfoStore[type];

                CreateDatabase(type, dir.Name, filenames[type]);
                PutMetadata(type);

                var con = new SQLiteConnection($"Data Source={di.DatabaseName};Version=3;");
                con.Open();

                cons.Add(type, con);

                var cmd = new SQLiteCommand(con);
                ins.Add(type,
                    new Inserter(cmd, $"INSERT INTO `{DatabaseInfoStore[type].DataTableName}` VALUES",
                        type != PatchContentType.Text, dat.ClientDat.Id));

                results.Add(type, 0);
            }

            foreach (var type in extractMarker.ToFile)
            {
                var intdir = Directory.CreateDirectory($"{dir}\\{filenames[type]}");
                pathes.Add(type, $"{intdir.FullName}");

                if (!results.ContainsKey(type))
                {
                    results.Add(type, 0);
                }
            }

            Logger.Write("начинаю извлечение.");

            try
            {
                Parallel.ForEach(dat.SubfileInfo, si =>
                {
                    var subfile = dat[si.Key];
                    if (!extractMarker.Overall.Contains(subfile.ContentType))
                    {
                        return;
                    }

                    switch (subfile.Extension)
                    {
                        case Extension.Txt:
                            var tsf = subfile.PrepareText(true);
                            if (extractMarker.ToDatabase.Contains(subfile.ContentType))
                            {
                                foreach (var dict in tsf.TextProduct)
                                {
                                    ins[subfile.ContentType].Push(new TextParams(subfile.FileId, dict.Key, dict.Value, tsf.ArgsProduct[dict.Key]));
                                }                                 
                            }

                            if (extractMarker.ToFile.Contains(subfile.ContentType))
                            {
                                // TODO
                            }

                            break;
                        case Extension.Dds:
                        case Extension.FontBin:
                        case Extension.Jpg:
                        case Extension.Ogg:
                        case Extension.Wav:
                            var data = subfile.CutHeader();
                            if (extractMarker.ToDatabase.Contains(subfile.ContentType))
                            {
                                ins[subfile.ContentType].Push(new BinParams(subfile.FileId, data));
                            }

                            if (extractMarker.ToFile.Contains(subfile.ContentType))
                            {
                                File.WriteAllBytes(
                                    $"{pathes[subfile.ContentType]}\\{dat.ClientDat.Name}${subfile.FileId}.{subfile.Extension}",
                                    data);
                            }

                            break;
                    }

                    lock (ResLockObj)
                    {
                        results[subfile.ContentType]++;
                    }
                });

                foreach (var inserter in ins)
                {
                    inserter.Value.Flush();
                }

                return results;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return results;
            }
            finally
            {
                foreach (var con in cons)
                {
                    con.Value?.Close();
                }

                Logger.Write("извлечение завершено.");
            }
        }
    }

    internal class DatabaseInfo
    {
        public DatabaseInfo(string createString, string dataTableName, string databaseName, string patchName,
            string patchDescr, string contentType, string version)
        {
            Version = version;
            ContentType = contentType;
            PatchDescr = patchDescr;
            PatchName = patchName;
            DatabaseName = databaseName;
            DefaultDatabaseName = databaseName;
            DataTableName = dataTableName;        
            CreateString = createString;
        }

        public string DefaultDatabaseName { get; }

        public string CreateString { get; }

        public string DataTableName { get; }

        public string DatabaseName { get; set; }
        
        public string PatchName { get; }

        public string PatchDescr { get; }

        public string ContentType { get; }

        public string Version { get; }
    }
}
