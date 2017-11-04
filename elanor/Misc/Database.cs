// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Elanor.DatFile;
using shared;

namespace Elanor.Misc
{
    internal class Database
    {
        public static bool TableExists(SQLiteConnection con, string table)
        {
            try
            {
                const string cmdText = "SELECT `name` FROM `sqlite_master` WHERE `type` = 'table' AND name = @table;";
                using (var cmd = new SQLiteCommand(cmdText, con))
                {
                    cmd.Parameters.AddWithValue("@table", table);

                    using (var rdr = cmd.ExecuteReader())
                    {
                        return rdr.StepCount > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return false;
            }
        }

        public static byte[] GetBlob(SQLiteConnection con, uint id)
        {
            byte[] blob = null;
            const string cmdText = "SELECT * FROM `patch_data` WHERE `file_id` = @id LIMIT 1;";
            using (var cmd = new SQLiteCommand(cmdText, con))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        blob = (byte[]) rdr["data"];
                    }
                }
            }

            return blob;
        }

        public static List<uint> GetFiles(SQLiteConnection con)
        {
            var list = new List<uint>();
            const string cmdText = "SELECT DISTINCT `file_id` FROM `patch_data` ORDER BY `file_id` ASC;";
            using (var cmd = new SQLiteCommand(cmdText, con))
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add((uint)rdr.GetInt32(0));
                    }
                }
            }

            return list;
        }

        public static List<uint> GetFilesByClientDat(SQLiteConnection con, int clientDatId)
        {
            var list = new List<uint>();
            var cmdText = $"SELECT DISTINCT `file_id` FROM `patch_data` WHERE `dat_id` = {clientDatId} ORDER BY `file_id` ASC;";

            try
            {
                using (var cmd = new SQLiteCommand(cmdText, con))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add((uint)rdr.GetInt32(0));
                        }
                    }
                }

                return list;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                Logger.Write("вероятно, патч старого формата, использую стандартное значение.");

                cmdText = "SELECT DISTINCT `file_id` FROM `patch_data` ORDER BY `file_id` ASC;";
                using (var cmd = new SQLiteCommand(cmdText, con))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add((uint)rdr.GetInt32(0));
                        }
                    }
                }

                return list;
            }           
        }

        public static List<int> GetClientDatList(SQLiteConnection con)
        {
            try
            {
                var list = new List<int>();
                const string cmdText = "SELECT DISTINCT `dat_id` FROM `patch_data` ORDER BY `dat_id` ASC;";
                using (var cmd = new SQLiteCommand(cmdText, con))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(rdr.GetInt32(0));
                        }
                    }
                }

                return list;
            }
            catch (Exception)
            {
                Logger.Write("патч старого формата, использую стандартное значение.");
                return new List<int> { 0 };
            }          
        }

        public static Dictionary<long, TextFragment> GetContent(SQLiteConnection con, uint id)
        {
            var dict = new Dictionary<long, TextFragment>();
            const string selectFragment = "SELECT `gossip_id`, `content`, `args_order`, `args` FROM `patch_data` WHERE `file_id` = @id;";
            using (var cmd = new SQLiteCommand(selectFragment, con))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        dict.Add(rdr.GetInt64(0), new TextFragment(rdr.GetString(1), rdr.GetString(2), rdr.GetString(3)));
                    }
                }
            }

            return dict;
        }
    }
}
