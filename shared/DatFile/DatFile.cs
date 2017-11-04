// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Text;

namespace shared
{
    public class DatFile : IDisposable
    {
        public DatFile()
        {
            _handle = -1;
            _disposed = false;
            SubfileInfo = new Dictionary<uint, SubfileInfo>();
        }

        private int _handle;

        public static bool[] HandleAllocArray = new bool[64];
        public static readonly object HandleAllocArrayLock = new object();

        public ClientDat ClientDat { get; private set; }

        private bool _disposed;
        public readonly Dictionary<uint, SubfileInfo> SubfileInfo;

        public Subfile this[uint index]
        {
            get
            {
                Subfile subfile = null;

                if (SubfileInfo.ContainsKey(index))
                {
                    var si = SubfileInfo[index];
                    subfile = new Subfile(_handle, index, si.Size, si.Iteration, DatExport.GetSubfileVersion(_handle, (int)index));
                }

                return subfile;
            }
        }

        private static int AllocHandle()
        {
            lock (HandleAllocArrayLock)
            {
                for (var i = 0; i < HandleAllocArray.Length; i++)
                {
                    if (!HandleAllocArray[i])
                    {
                        HandleAllocArray[i] = true;
                        return i;
                    }
                }

                throw new DatFileException("Too many files opened already.");
            }
        }

        public int Put(uint id, IntPtr buffer, int offset, int size, int version, int iteration, bool compress = false)
        {
            return DatExport.PutSubfileData(_handle, (int)id, buffer, offset, size, version, iteration, compress);
        }

        public int Purge(uint id)
        {
            return DatExport.PurgeSubfileData(_handle, (int)id);
        }

        public void Flush()
        {
            DatExport.Flush(_handle);
        }

        public void Close()
        {
            DatExport.CloseDatFile(_handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Close();
                FreeHandle(_handle);
            }

            _disposed = true;
        }

        ~DatFile()
        {
            Dispose(false);
        }

        private static void FreeHandle(int handle)
        {
            lock (HandleAllocArrayLock)
            {
                HandleAllocArray[handle] = false;
            }
        }

        private void Load()
        {
            var count = DatExport.GetNumSubfiles(_handle);
            for (var i = 0; i < count; ++i)
            {
                int fileId;
                int size;
                int iteration;
                DatExport.GetSubfileSizes(_handle, out fileId, out size, out iteration, i, 1);

                SubfileInfo.Add((uint)fileId, new SubfileInfo((uint)fileId, size, iteration));
            }            
        }

        public void Open(string path, bool readOnly)
        {
            // detect
            ClientDat = ClientDat.WhoIs(path);

            // open
            _handle = AllocHandle();

            try
            {
                var datIdStamp = new StringBuilder(64);
                var firstIterGuid = new StringBuilder(64);
                uint num = 130u;

                if (readOnly)
                {
                    num |= 4u;
                }

                int num2;
                int num3;
                int num4;
                int num5;
                ulong num6;

                if (DatExport.OpenDatFile(_handle, path, num, out num2, out num3, out num4, out num5, out num6, datIdStamp, firstIterGuid) == -1)
                {
                    throw new DatFileException("Unable to open file [ " + path + " ]");
                }

                Load();
            }
            catch
            {
                FreeHandle(_handle);
                throw;
            }
        }
    }

    public class SubfileInfo
    {
        public SubfileInfo(uint dataId, int size, int iteration)
        {
            DataId = dataId;
            Size = size;
            Iteration = iteration;
        }

        public uint DataId { get; }
        public int Size { get; }
        public int Iteration { get; }
    }
}
