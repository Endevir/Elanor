using System;
using System.Collections;
using System.IO;

namespace Mojo
{
    public class SourceFontInfo
    {
        public SourceFontInfo(string filename)
        {
            Filename = filename;
        }

        private string _filename;
        public string Filename
        {
            get { return _filename; }
            set
            {
                _filename = value;

                IsValid = Read(value);
            }
        }

        public bool IsValid { get; private set; }

        private bool Read(string filename)
        {
            try
            {
                var data = File.ReadAllBytes(filename);

                // file identifier: BMF3
                if (data[0] != 66 || data[1] != 77 || data[2] != 70 || data[3] != 3)
                {
                    return false;
                }

                using (var mr = new MemoryStream(data))
                {
                    using (var br = new BinaryReader(mr))
                    {
                        // file identifier
                        br.ReadInt32();

                        // block type 1: info
                        // read type identifier and size
                        br.ReadByte();
                        var block1Size = br.ReadInt32();

                        // read content
                        FontSize = br.ReadInt16();

                        var bitField = new BitArray(new[] { br.ReadByte() });
                        Smooth = bitField.Get(0);
                        Unicode = bitField.Get(1);
                        Italic = bitField.Get(2);
                        Bold = bitField.Get(3);
                        FixedHeight = bitField.Get(4);

                        Charset = br.ReadByte();
                        StretchH = br.ReadUInt16();
                        Antialiasing = br.ReadByte();
                        Padding = new Padding(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
                        Spacing = new Spacing(br.ReadByte(), br.ReadByte());
                        Outline = br.ReadByte();
                        FontName = new string(br.ReadChars(block1Size - 14));

                        // block type 2: common
                        // read type identifier and size
                        br.ReadByte();
                        br.ReadInt32();

                        // read content
                        LineHeight = br.ReadUInt16();
                        Base = br.ReadUInt16();
                        ScaleW = br.ReadUInt16();
                        ScaleH = br.ReadUInt16();
                        Pages = br.ReadUInt16();

                        // skip bit field
                        br.ReadByte();

                        AlphaChannel = br.ReadByte();
                        RedChannel = br.ReadByte();
                        GreenChannel = br.ReadByte();
                        BlueChannel = br.ReadByte();

                        // block type 3: pages
                        // read type identifier and size
                        br.ReadByte();
                        var block3Size = br.ReadInt32();

                        // read content
                        // assume we have no more than 1 page here
                        DdsFilename = new string(br.ReadChars(block3Size));

                        // block type 4: chars
                        // read type identifier and size
                        br.ReadByte();
                        var block4Size = br.ReadInt32();

                        // read content
                        CharsCount = block4Size / 20;
                        Chars = new SourceCharInfo[CharsCount];
                        for (var i = 0; i < CharsCount; i++)
                        {
                            Chars[i] = new SourceCharInfo(br.ReadUInt32(), br.ReadUInt16(), br.ReadUInt16(),
                                br.ReadUInt16(), br.ReadUInt16(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(),
                                br.ReadByte(), br.ReadByte());
                        }

                        // skip block 5
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // block 1: info
        public string FontName { get; private set; }
        public short FontSize { get; private set; }
        public bool Bold { get; private set; }
        public bool Italic { get; private set; }
        public bool FixedHeight { get; private set; }
        public byte Charset { get; private set; }
        public bool Unicode { get; private set; }
        public ushort StretchH { get; private set; }
        public bool Smooth { get; private set; }
        public byte Antialiasing { get; private set; }
        public Padding Padding { get; private set; }
        public Spacing Spacing { get; private set; }
        public byte Outline { get; private set; }

        // block 2: common
        public ushort LineHeight { get; private set; }
        public ushort Base { get; private set; }
        public ushort ScaleW { get; private set; }
        public ushort ScaleH { get; private set; }
        public ushort Pages { get; private set; }
        public byte AlphaChannel { get; private set; }
        public byte RedChannel { get; private set; }
        public byte GreenChannel { get; private set; }
        public byte BlueChannel { get; private set; }

        public string DdsFilename { get; private set; }
        public int CharsCount { get; private set; }
        public SourceCharInfo[] Chars { get; private set; }

        // dds
        public int DdsWidth { get; private set; }
        public int DdsHeight { get; private set; }
    }

    public class Padding
    {
        public Padding(byte up, byte right, byte down, byte left)
        {
            Left = left;
            Right = right;
            Up = up;
            Down = down;
        }

        public byte Left { get; }
        public byte Right { get; }
        public byte Up { get; }
        public byte Down { get; }
    }

    public class Spacing
    {
        public Spacing(byte horizontal, byte vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        public byte Horizontal { get; }
        public byte Vertical { get; }
    }

    public class SourceCharInfo
    {
        public SourceCharInfo(uint id, ushort x, ushort y, ushort width, ushort height, short xoffset, short yoffset,
            short xadvance, byte page, byte channel)
        {
            Id = id;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            XOffset = xoffset;
            YOffset = yoffset;
            XAdvance = xadvance;
            Page = page;
            Channel = channel;
        }

        public uint Id { get; }
        public ushort X { get; }
        public ushort Y { get; }
        public ushort Width { get; }
        public ushort Height { get; }
        public short XOffset { get; }
        public short YOffset { get; }
        public short XAdvance { get; }
        public byte Page { get; }
        public byte Channel { get; }
    }
}
