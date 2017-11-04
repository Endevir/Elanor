// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Runtime.Serialization;

namespace shared
{
    internal class DatFileException : ApplicationException
    {
        public DatFileException()
        {
        }

        public DatFileException(string message) : base(message)
        {
        }

        public DatFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DatFileException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
