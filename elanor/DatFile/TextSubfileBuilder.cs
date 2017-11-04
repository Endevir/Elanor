// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Elanor.Misc;
using shared;

namespace Elanor.DatFile
{
    internal class TextSubfileBuilder : SubfileBuilder
    {
        public TextSubfileBuilder(DatController datController, Patch.Patch patch)
        {
            Init(datController, patch);
        }

        public static byte[] CustomData(uint id, Dictionary<long, string> fragments)
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new BinaryWriter(ms, Encoding.Unicode))
                {
                    sw.Write(id);
                    sw.Write(1);
                    sw.Write((byte)0);
                    ByteOrShort(sw, (short)fragments.Count);

                    foreach (var fragment in fragments)
                    {
                        // write gossip_id
                        sw.Write(fragment.Key);

                        // write text pieces
                        sw.Write(1);

                        ByteOrShort(sw, (short)fragment.Value.Length);
                        sw.Write(Encoding.Unicode.GetBytes(fragment.Value));

                        // write arguments
                        sw.Write(0);

                        // write variables
                        sw.Write((byte)0);
                    }
                }

                return ms.ToArray();
            }
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

            var fragments = Database.GetContent(con, id);
            if (fragments.Count == 0)
            {
                ++errors;
                Logger.Write($"файл #{id} пуст, пропускаю.");
                return null;
            }

            if (!subfile.IsValid)
            {
                ++errors;
                return null;
            }

            var orig = new TextSubfile(subfile.Data);
            if (!orig.IsValid)
            {
                ++errors;
                Logger.Write($"ошибка чтения файла #{id}, пропускаю весь файл.");
                return null;
            }

            var skippedFragmentsCount = fragments.Count(fragment => !orig.Body.ContainsKey(fragment.Key));
            if (skippedFragmentsCount == fragments.Count)
            {
                ++errors;
                Logger.Write($"ни один фрагмент не подходит для перезаписи файла #{id}, пропускаю весь файл.");
                return null;
            }

            if (skippedFragmentsCount > 0)
            {
                Logger.Write($"{skippedFragmentsCount} неопознанных фрагментов в патче для файла #{id} будет пропущено.");
            }

            using (var ms = new MemoryStream())
            {
                using (var sw = new BinaryWriter(ms, Encoding.Unicode))
                {
                    //Logger.Write($"Кек #{orig.Head.Unknown}");
                    sw.Write(id);
                    sw.Write(orig.Head.Unknown);
                    //sw.Write(0);
                    sw.Write(orig.Head.Unknown2);
                    ByteOrShort(sw, orig.Head.Fragments);

                    var missingFragmentsCount = 0;
                    foreach (var body in orig.Body)
                    {
                        // detect possible errors
                        var useDefault = false;

                        // patch does not contain data for this fragment
                        if (!fragments.ContainsKey(body.Key))
                        {
                            missingFragmentsCount++;
                            useDefault = true;
                            Logger.Write($"файл #{id}, фрагмент #{body.Key} - отсутствует в патче, использую оригинал.");
                        }

                        // patch fragment args contain error
                        if (!useDefault && fragments[body.Key].IsArgsError)
                        {
                            missingFragmentsCount++;
                            useDefault = true;
                            Logger.Write($"файл #{id}, фрагмент #{body.Key} - ошибка в строке аргументов, использую оригинал.");
                        }

                        // patch fragment args order contains error
                        if (!useDefault && fragments[body.Key].IsOrderError)
                        {
                            missingFragmentsCount++;
                            useDefault = true;
                            Logger.Write($"файл #{id}, фрагмент #{body.Key} - ошибка в строке порядка, использую оригинал.");
                        }

                        // patch fragment text contain error
                        if (!useDefault && !fragments[body.Key].IsValid)
                        {
                            missingFragmentsCount++;
                            useDefault = true;
                            Logger.Write($"файл #{id}, фрагмент #{body.Key} - ошибка в содержимом, использую оригинал.");
                        }

                        var pieces = useDefault
                            ? body.Value.Pieces
                            : fragments[body.Key].Content.Split(new[] {"<--DO_NOT_TOUCH!-->"}, StringSplitOptions.None);

                        // patch args order was intentionally emptied, force it
                        if (!useDefault && fragments[body.Key].ArgsOrder.Length == 0 && fragments[body.Key].Args.Length > 0 && pieces.Length == 1)
                        {
                            fragments[body.Key].IsDefaultOrder = false;
                        }

                        // broken relation between args count and their plant holes
                        if (!useDefault &&
                            (!fragments[body.Key].IsDefaultOrder &&
                             fragments[body.Key].ArgsOrder.Length != pieces.Length - 1 ||
                             fragments[body.Key].IsDefaultOrder &&
                             fragments[body.Key].Args.Length != pieces.Length - 1))
                        {
                            missingFragmentsCount++;
                            useDefault = true;
                            pieces = body.Value.Pieces;
                            Logger.Write($"файл #{id}, фрагмент #{body.Key} - ошибка в строке порядка, использую оригинал.");
                        }

                        // write token
                        sw.Write(body.Key);

                        // write pieces
                        sw.Write(pieces.Length);

                        foreach (var piece in pieces)
                        {
                            var length = (short) piece.Length;
                            ByteOrShort(sw, length);
                            sw.Write(Encoding.Unicode.GetBytes(piece));
                        }

                        if (useDefault)
                        {
                            var args = body.Value.Arguments;
                            sw.Write(args.Length);

                            foreach (var arg in args)
                            {
                                sw.Write(arg);
                            }
                        }
                        else
                        {
                            if (fragments[body.Key].IsDefaultOrder)
                            {
                                // use patch default args (ignore patch args order or empty patch args order)
                                var args = fragments[body.Key].Args;
                                sw.Write(args.Length);

                                foreach (var arg in args)
                                {
                                    sw.Write(arg);
                                }
                            }
                            else
                            {
                                #if DEBUG

                                if (fragments[body.Key].IsExtraOrder)
                                {
                                    Logger.Write($"фрагмент #{body.Key} из файла #{id} - порядок аргументов длиннее исходного.");
                                }

                                #endif

                                // use patch alt args order
                                sw.Write(fragments[body.Key].ArgsOrder.Length);

                                foreach (var ord in fragments[body.Key].ArgsOrder)
                                {
                                    sw.Write(fragments[body.Key].Args[ord - 1]);
                                }
                            }                            
                        }

                        // use original variables
                        var vars = body.Value.Variables;
                        sw.Write((byte) vars.Length);
                        foreach (var pack in vars)
                        {
                            sw.Write(pack.Length);
                            foreach (var rice in pack)
                            {
                                var length = (short) rice.Length;
                                ByteOrShort(sw, length);
                                sw.Write(Encoding.Unicode.GetBytes(rice));
                            }
                        }
                    }

                    if (missingFragmentsCount > 0)
                    {
                        Logger.Write(
                            $"{missingFragmentsCount} из {orig.Head.Fragments} фрагментов в файле #{id} не изменились.");
                    }
                }

                return ms.ToArray();
            }
        }

        private static void ByteOrShort(BinaryWriter writer, short value)
        {
            if (value < 0x80)
            {
                var drop = (byte) value;

                writer.Write(drop);
            }
            else
            {
                var dropOne = (byte) ((value >> 8) ^ 0x80);
                var dropTwo = (byte) (value & 0xFF);

                writer.Write(dropOne);
                writer.Write(dropTwo);
            }
        }
    }
}
