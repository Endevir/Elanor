using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Mojo
{
    public class DestFontInfo
    {
        public DestFontInfo(SourceFontInfo info)
        {
            FontSize = Math.Abs(info.FontSize);
            Base = info.Base;
            LineHeight = (short)info.LineHeight;
            Charset = info.Charset;
            CharsCount = info.CharsCount;
            Padding = info.Padding.Left;

            Chars = new DestCharInfo[CharsCount];
            for (var i = 0; i < CharsCount; i++)
            {
                Chars[i] = new DestCharInfo(info.Chars[i]);
            }
        }

        public int FontSize { get; }
        public int Base { get; }
        public short LineHeight { get; }
        public short Charset { get; }
        public byte Padding { get; }

        public int CharsCount { get; }
        public DestCharInfo[] Chars { get; }

        public void Import(string fontBinFilename)
        {
            var dir = Path.Combine(Application.StartupPath, $"output_{DateTime.Now:dd-mm-yyyy-hh-mm}");
            Directory.CreateDirectory(dir);

            var path = Path.Combine(dir, Path.GetFileName(fontBinFilename));

            uint id;
            int unk;
            uint ddsId1, ddsId2;
            using (var bfs = new FileStream(fontBinFilename, FileMode.Open))
            {
                using (var br = new BinaryReader(bfs, Encoding.Unicode))
                {
                    id = br.ReadUInt32();

                    br.BaseStream.Position = br.BaseStream.Length - 12;

                    unk = br.ReadInt32();
                    ddsId1 = br.ReadUInt32();
                    ddsId2 = br.ReadUInt32();
                }
            }

            using (var fs = new FileStream(path, FileMode.CreateNew))
            {
                using (var bw = new BinaryWriter(fs, Encoding.Unicode))
                {
                    bw.Write(id);
                    bw.Write(FontSize);
                    bw.Write(Base);
                    bw.Write(CharsCount);
                    bw.Write(Padding);
                    bw.Write(LineHeight);
                    bw.Write(Charset);

                    for (var i = 0; i < CharsCount; i++)
                    {
                        bw.Write(Chars[i].Id);
                        bw.Write(Chars[i].X);
                        bw.Write(Chars[i].Y);
                        bw.Write(Chars[i].Width);
                        bw.Write(Chars[i].Height);
                        bw.Write(Chars[i].XOffset);
                        bw.Write(Chars[i].YOffset);
                        bw.Write(Chars[i].XAdvance);
                    }

                    bw.Write((byte)0);
                    bw.Write((short)0);

                    bw.Write(unk);
                    bw.Write(ddsId1);
                    bw.Write(ddsId2);
                }
            }
        }
    }

    public class DestCharInfo
    {
        public DestCharInfo(SourceCharInfo info)
        {
            Id = (ushort)info.Id;
            X = info.X;
            Y = info.Y;
            Width = (byte)info.Width;
            Height = (byte)info.Height;
            XOffset = (sbyte)info.XOffset;
            YOffset = (sbyte)info.YOffset;
            XAdvance = (byte)info.XAdvance;
        }

        public ushort Id { get; }
        public ushort X { get; }
        public ushort Y { get; }
        public byte Width { get; }
        public byte Height { get; }
        public sbyte XOffset { get; }
        public sbyte YOffset { get; }
        public byte XAdvance { get; }
    }
}
