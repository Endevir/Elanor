// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace shared
{
    public class Subfile
    {
        internal Subfile(int handle, uint fileId, int size, int iteration, int version)
        {
            _handle = handle;
            FileId = fileId;
            Size = size;
            Iteration = iteration;
            Version = version;
            IsCompressed = 1 == DatExport.GetSubfileCompressionFlag(_handle, (int)fileId);

            GetData();
            CheckHeader();
        }

        public byte[] Data { get; private set; }

        private void GetData()
        {
            try
            {
                if (Data == null)
                {
                    Data = new byte[Size];
                    var dataPtr = Marshal.AllocHGlobal(Size);

                    int num;
                    DatExport.GetSubfileData(_handle, (int)FileId, dataPtr, 0, out num);

                    Marshal.Copy(dataPtr, Data, 0, Size);
                    Marshal.FreeHGlobal(dataPtr);

                    if (IsCompressed)
                    {
                        Data = Decompress();
                    }
                }

                IsValid = true;
            }
            catch
            {
                Logger.Write($"файл #{FileId} повреждён, пропускаю.");
            }
        }

        public uint FileId { get; }

        private readonly int _handle;

        public bool IsCompressed { get; }

        public int Iteration { get; }

        public int Size { get; }

        public int Version { get; }

        public bool IsValid { get; private set; }

        public Extension Extension { get; set; }

        public PatchContentType ContentType
        {
            get
            {
                switch (Extension)
                {
                    case Extension.Txt:
                        return PatchContentType.Text;
                    case Extension.Dds:
                        return PatchContentType.Texture;
                    case Extension.FontBin:
                        return PatchContentType.Font;
                    case Extension.Jpg:
                        return PatchContentType.Image;
                    case Extension.Ogg:
                    case Extension.Wav:
                        return PatchContentType.Sound;
                    default:
                        return PatchContentType.Undef;
                }
            }
        }

        private byte[] Decompress()
        {
            var size = Data.Length;
            var num = Data[3] << 24 | Data[2] << 16 | Data[1] << 8 | Data[0];
            var num2 = size - 4;
            var array2 = new byte[num];
            var array3 = new byte[num2];

            for (var j = 4; j < size; j++)
            {
                array3[j - 4] = Data[j];
            }

            DatExport.uncompress(array2, ref num, array3, num2);

            return array2;
        }

        public TextSubfile PrepareText(bool readable)
        {
            if (Extension != Extension.Txt)
            {
                return null;
            }

            var tsf = new TextSubfile(Data, readable);
            if (!tsf.IsValid)
            {
                return null;
            }

            return tsf;
        }

        private void CheckHeader()
        {
            // primitive text check
            if (FileId >> 24 == 0x25)
            {
                Extension = Extension.Txt;
                return;
            }

            // jpeg / dds check
            if (FileId >> 24 == 0x41)
            {
                if (Data.Length >= 34)
                {
                    using (var mr = new MemoryStream(Data))
                    {
                        using (var br = new BinaryReader(mr, Encoding.Unicode))
                        {
                            br.ReadInt32();
                            br.ReadInt32();
                            br.ReadInt32();
                            br.ReadInt32();
                            br.ReadInt32();
                            br.ReadInt32();

                            var soi = br.ReadUInt16();  // Start of Image (SOI) marker (FFD8)
                            var marker = br.ReadUInt16(); // JFIF marker (FFE0) EXIF marker (FFE1)
                            var markerSize = br.ReadUInt16(); // size of marker data (incl. marker)
                            var four = br.ReadUInt32(); // JFIF 0x4649464a or Exif  0x66697845

                            var isJpeg = soi == 0xd8ff && ((marker & 0xe0ff) == 0xe0ff || (marker & 0xe1ff) == 0xe1ff);
                            //var isExif = isJpeg && four == 0x66697845;
                            //var isJfif = isJpeg && four == 0x4649464a;

                            if (isJpeg)
                            {
                                Extension = Extension.Jpg;
                                return;
                            }
                        }                
                    }                
                }

                Extension = Extension.Dds;
                return;
            }

            // ogg / wav check
            if (Data.Length >= 12)
            {
                if (Data[8] == 82 && Data[9] == 73 && Data[10] == 70 && Data[11] == 70)
                {
                    Extension = Extension.Wav;
                    return;
                }

                if (Data[8] == 79 && Data[9] == 103 && Data[10] == 103 && Data[11] == 83)
                {
                    Extension = Extension.Ogg;
                    return;
                }
            }

            // primitive font check
            if (FileId >> 24 == 0x42)
            {
                Extension = Extension.FontBin;
                return;
            }

            Extension = Extension.Unknown;
        }

        public byte[] CutHeader()
        {
            switch (Extension)
            {
                case Extension.Jpg:
                    var jpgArray = new byte[Data.Length - 24];
                    Buffer.BlockCopy(Data, 24, jpgArray, 0, jpgArray.Length);
                    return jpgArray;
                case Extension.Ogg:
                case Extension.Wav:
                    var newArray = new byte[Data.Length - 8];
                    Buffer.BlockCopy(Data, 8, newArray, 0, newArray.Length);
                    return newArray;
                case Extension.Dds:
                    var ddsArray = new byte[Data.Length - 24 + 128];
                    Buffer.BlockCopy(Data, 24, ddsArray, 128, Data.Length - 24);
                    //File.WriteAllBytes($"dds\\11_{FileId}.dds", Data);
                    // dds magic
                    ddsArray[0] = 0x44; // D
                    ddsArray[1] = 0x44; // D
                    ddsArray[2] = 0x53; // S
                    ddsArray[3] = 0x20;
                    ddsArray[4] = 0x7C;

                    ddsArray[8] = 7;
                    ddsArray[9] = 0x10;

                    // width, height
                    ddsArray[12] = Data[12];
                    ddsArray[13] = Data[13];
                    ddsArray[14] = Data[14];
                    ddsArray[15] = Data[15];

                    ddsArray[16] = Data[8];
                    ddsArray[17] = Data[9];
                    ddsArray[18] = Data[10];
                    ddsArray[19] = Data[11];

                    var compression = BitConverter.ToUInt32(Data, 0x10);
                    switch (compression)
                    {
                        case 20:        // 14 00 00 00 - 888 (R8G8B8)
                            ddsArray[0x4C] = 0x20;  // ?
                            ddsArray[0x50] = 0x40;  // compressed or not

                            ddsArray[0x58] = 0x18;  // bytes per pixel
                            ddsArray[0x5E] = 0xFF;  
                            ddsArray[0x61] = 0xFF;  
                            ddsArray[0x64] = 0xFF;  
                             
                            return ddsArray;
                        case 21:        // 15 00 00 00 - 8888 (R8G8B8A8)
                            ddsArray[0x4C] = 0x20;  // ?
                            ddsArray[0x50] = 0x40;  // compressed or not

                            ddsArray[0x58] = 0x20;  // bytes per pixel
                            ddsArray[0x5E] = 0xFF;
                            ddsArray[0x61] = 0xFF;
                            ddsArray[0x64] = 0xFF;
                            ddsArray[0x6B] = 0xFF;

                            return ddsArray;
                        case 28:        // 1C 00 00 00 - 332 (?)
                            ddsArray[0x4C] = 0x20;  // ?
                            ddsArray[0x50] = 0x40;  // compressed or not

                            ddsArray[0x58] = 0x08;  // bytes per pixel
                            ddsArray[0x5E] = 0xFF;
                            ddsArray[0x61] = 0xFF;
                            ddsArray[0x64] = 0xFF;

                            return ddsArray;
                        case 827611204: // 44 58 54 31 - DXT1
                            ddsArray[76] = 32;
                            ddsArray[80] = 4;

                            ddsArray[84] = 68;
                            ddsArray[85] = 88;
                            ddsArray[86] = 84;
                            ddsArray[87] = 49;

                            return ddsArray;
                        case 861165636: // 44 58 54 33 - DXT3
                            ddsArray[22] = 1;
                            ddsArray[76] = 32;
                            ddsArray[80] = 4;

                            ddsArray[84] = 68;
                            ddsArray[85] = 88;
                            ddsArray[86] = 84;
                            ddsArray[87] = 51;

                            ddsArray[108] = 8;
                            ddsArray[109] = 16;
                            ddsArray[110] = 64;

                            return ddsArray;
                        case 894720068: // 44 58 54 35 - DXT5
                            ddsArray[10] = 8;
                            ddsArray[22] = 1;
                            ddsArray[28] = 1;
                            ddsArray[76] = 32;
                            ddsArray[80] = 4;

                            ddsArray[84] = 68;
                            ddsArray[85] = 88;
                            ddsArray[86] = 84;
                            ddsArray[87] = 53;

                            ddsArray[88] = 32;
                            ddsArray[94] = 255;
                            ddsArray[97] = 255;
                            ddsArray[100] = 255;
                            ddsArray[107] = 255;
                            ddsArray[109] = 16;

                            return ddsArray;
                    }
                    break;
            }

            return Data;
        }
    }
}
