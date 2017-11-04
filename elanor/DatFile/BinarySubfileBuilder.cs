// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Elanor.Misc;
using shared;

namespace Elanor.DatFile
{
    internal class BinarySubfileBuilder : SubfileBuilder
    {
        public BinarySubfileBuilder(DatController datController, Patch.Patch patch)
        {
            Init(datController, patch);
        }

        public override byte[] Data(uint id, shared.DatFile dat, SQLiteConnection con, Subfile subfile, 
            PatchContentType contentType,
            ref int errors)
        {
            if (!dat.SubfileInfo.ContainsKey(id))
            {
                ++errors;
                Logger.Write($"файл #{id} не существует в ресурсах игры, пропускаю.");
                return null;
            }

            var blob = Database.GetBlob(con, id);
            if (blob == null)
            {
                ++errors;
                Logger.Write($"файл #{id} не удалось прочитать из патча, пропускаю.");
                return null;
            }

            using (var ms = new MemoryStream())
            {
                using (var sw = new BinaryWriter(ms, Encoding.Unicode))
                {
                    if (!subfile.IsValid)
                    {
                        ++errors;
                        return null;
                    }

                    switch (contentType)
                    {
                        case PatchContentType.Image:
                            for (var i = 0; i < 20; ++i)
                            {
                                sw.Write(subfile.Data[i]);
                            }

                            sw.Write(blob.Length);
                            break;
                        case PatchContentType.Sound:
                            for (var i = 0; i < 4; ++i)
                            {
                                sw.Write(subfile.Data[i]);
                            }

                            sw.Write(blob.Length - 8);
                            break;
                        case PatchContentType.Texture:
                            // TODO - in case of modified width/height
                            for (var i = 0; i < 24; ++i)
                            {
                                sw.Write(subfile.Data[i]);
                            }

                            blob = blob.Skip(128).ToArray();
                            break;
                    }

                    sw.Write(blob);
                }

                return ms.ToArray();
            }
        }
    }
}
