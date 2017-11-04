// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Jozo
{
    public class Logger
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Write(string line, bool caller = true)
        {
            using (var sw = File.AppendText("prelauncher_log.txt"))
            {
                var prefix = DateTime.Now + @" | ";
                if (caller)
                {
                    prefix += new StackFrame(1, true).GetMethod().Name + ": ";
                }

                var output = prefix + line;
                sw.WriteLine(output);
            }
        }
    }
}
