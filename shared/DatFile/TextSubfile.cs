// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace shared
{
    public class TextSubfile
    {
        public TextSubfile(byte[] data, bool readable = false)
        {
            Parse(data, readable);
        }

        public TextSubfileHead Head { get; private set; }

        public Dictionary<long, TextSubfileBody> Body { get; } = new Dictionary<long, TextSubfileBody>();

        public bool IsValid { get; private set; }

        private void Parse(byte[] data, bool readable)
        {
            try
            {
                using (var memStream = new MemoryStream(data))
                {
                    using (var binaryReader = new BinaryReader(memStream, Encoding.Unicode))
                    {
                        Head = new TextSubfileHead(binaryReader);
                        for (var i = 0; i < Head.Fragments; ++i)
                        {
                            var limb = new TextSubfileBody(binaryReader);
                            Body.Add(limb.Token, limb);
                        }
                    }
                }

                IsValid = true;

                if (readable)
                {
                    PrepareReadable();
                }
            }
            catch (Exception ex)
            {
                IsValid = false;
                Logger.Write(ex.Message);
            }
        }

        public Dictionary<long, string> TextProduct { get; } = new Dictionary<long, string>();
        public Dictionary<long, string> ArgsProduct { get; } = new Dictionary<long, string>();

        private void PrepareReadable()
        {
            foreach (var body in Body)
            {
                TextProduct.Add(body.Key, "[" + string.Join("<--DO_NOT_TOUCH!-->", body.Value.Pieces) + "]");
                ArgsProduct.Add(body.Key, string.Join("-", body.Value.Arguments.Select(arg => arg.ToString()).ToList()));
            }
        }
    }

    public class TextSubfileHead
    {
        public TextSubfileHead(BinaryReader reader)
        {
            DataId = reader.ReadInt32();
            Unknown = reader.ReadInt32();
            Unknown2 = reader.ReadByte();
            Fragments = reader.ReadByte();

            if ((Fragments & 128) != 0)
            {
                Fragments = (short)((Fragments ^ 128) << 8 | reader.ReadByte());
            }
        }

        public int DataId { get; }

        public int Unknown { get; }

        public byte Unknown2 { get; }

        public short Fragments { get; }
    }

    public class TextSubfileBody
    {
        public TextSubfileBody(BinaryReader reader)
        {
            Token = reader.ReadInt64();

            // text pieces section
            var pieces = reader.ReadInt32();
            Pieces = new string[pieces];
            for (var i = 0; i < pieces; ++i)
            {
                Pieces[i] = ReadString(reader);
            }

            // arguments section
            var length = reader.ReadInt32();
            Arguments = new int[length];
            for (var i = 0; i < length; ++i)
            {
                Arguments[i] = reader.ReadInt32();
            }

            // variables section
            var packs = reader.ReadByte();
            Variables = new string[packs][];
            for (var j = 0; j < packs; ++j)
            {
                var variables = reader.ReadInt32();
                Variables[j] = new string[variables];
                for (var i = 0; i < variables; ++i)
                {
                    Variables[j][i] = ReadString(reader);
                }
            }
        }

        public long Token { get; }

        public string[] Pieces { get; }

        public int[] Arguments { get; }

        public string[][] Variables { get; }

        private static string ReadString(BinaryReader reader)
        {
            short length = reader.ReadByte();
            if ((length & 128) != 0)
            {
                length = (short)((length ^ 128) << 8 | reader.ReadByte());
            }

            return new string(reader.ReadChars(length));
        }
    }
}
