// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace shared
{
    public class Logger
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Write(string line, [CallerMemberName] string callerMemberName = "")
        {
            using (var sw = File.AppendText("log.txt"))
            {
                var prefix = DateTime.Now + @" | " + callerMemberName + ": ";
                var output = prefix + line;
                sw.WriteLine(output);
            }
        }
    }
}
