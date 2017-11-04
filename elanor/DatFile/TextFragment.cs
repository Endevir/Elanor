// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using shared;

namespace Elanor.DatFile
{
    internal class TextFragment
    {
        public TextFragment(string content, string argsOrder, string args)
        {
            Content = Unbox(content);

            IsDefaultOrder = true;
            DecomposeArgs(args);
            DecomposeOrder(argsOrder);            
        }

        public string Content { get; }

        public int[] ArgsOrder { get; private set; }

        public int[] Args { get; private set; }

        public bool IsDefaultOrder { get; set; }

        public bool IsValid { get; private set; }

        public bool IsOrderError { get; private set; }

        public bool IsArgsError { get; private set; }

        public bool IsExtraOrder { get; private set; }

        private string Unbox(string content)
        {
            // remove extra brackets
            if (content.Length < 2)
            {
                return content;
            }

            IsValid = true;
            return content.Substring(1, content.Length - 2);
        }

        private void DecomposeArgs(string args)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(args))
                {
                    var argsSplit = args.Split('-');
                    Args = new int[argsSplit.Length];
                    for (var i = 0; i < argsSplit.Length; ++i)
                    {
                        Args[i] = Convert.ToInt32(argsSplit[i]);
                    }                    
                }
                else
                {
                    Args = new int[0];
                }
            }
            catch (Exception ex)
            {
                IsArgsError = true;
                Logger.Write(ex.Message);
            }
        }

        private void DecomposeOrder(string argsOrder)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(argsOrder))
                {
                    var argsSplit = argsOrder.Split('-');

                    ArgsOrder = new int[argsSplit.Length];
                    for (var i = 0; i < argsSplit.Length; ++i)
                    {
                        ArgsOrder[i] = Convert.ToInt32(argsSplit[i]);
                    }

                    IsDefaultOrder = false;
                }
                else
                {
                    ArgsOrder = new int[0];
                }

                CheckOrder();
            }
            catch (Exception ex)
            {
                IsOrderError = true;
                Logger.Write(ex.Message);
            }
        }

        private void CheckOrder()
        {
            if (ArgsOrder.Length > Args.Length)
            {
                IsExtraOrder = true;
            }

            var maxOrder = Args.Length;
            foreach (var ord in ArgsOrder)
            {
                if (ord > maxOrder)
                {
                    IsOrderError = true;
                    break;
                }
            }
        }
    }
}
