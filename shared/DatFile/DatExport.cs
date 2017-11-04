// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace shared
{
    internal class DatExport
    {
        //public const uint DcofCreate = 8u;

        //public const uint DcofCreateIfNeeded = 16u;

        //public const uint DcofExpandable = 2u;

        //public const uint DcofFreeThreaded = 64u;

        //public const uint DcofJournalled = 256u;

        //public const uint DcofLoadIterations = 128u;

        //public const uint DcofOptionalFile = 32u;

        //public const uint DcofReadOnly = 4u;

        //public const uint DcofSkipIndexCheck = 512u;

        //public const uint DcofUseLru = 1u;

        [DllImport("DatExport.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CloseDatFile(int handle);

        [DllImport("DatExport.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetNumSubfiles(int handle);

        [DllImport("DatExport.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte GetSubfileCompressionFlag(int handle, int id);

        [DllImport("DatExport.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetSubfileData(int handle, int did, IntPtr buffer, int writeOffset, out int version);

        [DllImport("DatExport.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetSubfileSizes(int handle, out int did, out int size, out int iteration, int offset, int count);

        [DllImport("DatExport.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetSubfileVersion(int handle, int did);

        [DllImport("DatExport.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetNumIterations(int handle);

        [DllImport("DatExport.dll", EntryPoint = "OpenDatFileEx2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int OpenDatFile(int handle, string fileName, uint flags, out int didMasterMap, out int blockSize, out int vnumDatFile, out int vnumGameData, out ulong datFileId, [MarshalAs(UnmanagedType.LPStr)] StringBuilder datIdStamp, [MarshalAs(UnmanagedType.LPStr)] StringBuilder firstIterGuid);

        [DllImport("zlib1T", CallingConvention = CallingConvention.Cdecl)]
        public static extern int uncompress(byte[] dest, ref int destLen, byte[] source, int sourceLen);

        [DllImport("DatExport.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PurgeSubfileData(int handle, int did);

        [DllImport("DatExport.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PutSubfileData(int handle, int did, IntPtr buffer, int offset, int size, int version, int iteration, bool compress);

        [DllImport("DatExport.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PreallocateSubfile(int handle, int did, int size, int version, int iteration, bool compressed);

        [DllImport("DatExport.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReleaseSubfileSizeData(int did);

        [DllImport("DatExport.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Flush(int handle);
    }
}
