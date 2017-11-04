// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Elanor.Misc;
using shared;

namespace Elanor.DatFile
{
    internal abstract class SubfileBuilder
    {
        public List<uint> Files { get; protected set; } = new List<uint>();

        public int InitResult { get; protected set; }

        public bool IsOldClientDat { get; private set; }

        public abstract byte[] Data(uint id, shared.DatFile dat, SQLiteConnection con, Subfile subfile,
            PatchContentType contentType,
            ref int errors);

        protected void Init(DatController datController, Patch.Patch patch)
        {
            try
            {
                using (var con = new SQLiteConnection($"Data Source = {patch.Path}; Version = 3;"))
                {
                    con.Open();

                    Files = Database.GetFilesByClientDat(con, datController.Dat.ClientDat.Id);
                }

                IsOldClientDat = Files.Count > datController.Dat.SubfileInfo.Count;

                InitResult = 0;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                Files.Clear();
                InitResult = -1;
            }
        }

        public static SubfileBuilder GetBuilder(DatController datController, Patch.Patch patch)
        {
            try
            {
                SubfileBuilder builder;
                switch (patch.ContentType)
                {
                    case PatchContentType.Text:
                        builder = new TextSubfileBuilder(datController, patch);
                        break;
                    case PatchContentType.Image:
                    case PatchContentType.Sound:
                    case PatchContentType.Font:
                    case PatchContentType.Texture:
                        builder = new BinarySubfileBuilder(datController, patch);
                        break;
                    default:
                        throw new ArgumentException($"тип файлов {patch.ContentType} не обрабатывается программой.");
                }

                return builder.InitResult == -1 ? null : builder;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                return null;
            }
        }
    }
}
