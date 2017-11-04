// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace shared
{
    public class Inserter
    {
        public Inserter(SQLiteCommand command, string cmdText, bool bin, int clientDatId)
        {
            _clientDatId = clientDatId;
            _isBin = bin;
            _cmdText = cmdText;
            _cmd = command;

            Amount = bin ? 999 / 2 : 999 / 4;
        }

        private readonly SQLiteCommand _cmd;
        private readonly string _cmdText;
        private readonly int _clientDatId;

        public int Amount { get; }

        public uint Size { get; set; } = 10 * 1024 * 1024;

        private readonly bool _isBin;
        private int _currentAmount;
        private int _currentSize;
        private string _currentCmdText = string.Empty;

        private readonly List<TextParams> _textParams = new List<TextParams>();
        private readonly List<BinParams> _binParams = new List<BinParams>();

        private static readonly object PushObj = new object();

        public void Push(InserterParam iparam)
        {
            lock (PushObj)
            {
                var incomingSize = iparam.TotalSize;
                var expectedSize = _currentSize + incomingSize;

                if (expectedSize > Size)
                {
                    Flush();
                }

                _currentSize = expectedSize;
            
                AddParam(iparam);

                if (_currentAmount == Amount)
                {
                    Flush();
                }                
            }
        }

        public void Flush()
        {
            if (_isBin && _binParams.Count == 0)
            {
                return;
            }

            if (!_isBin && _textParams.Count == 0)
            {
                return;
            }

            _cmd.CommandText = _currentCmdText + ";";

            _cmd.Parameters.Clear();

            if (_isBin)
            {
                for (var i = 0; i < _binParams.Count; ++i)
                {
                    var bparam = _binParams[i];

                    _cmd.Parameters.AddWithValue($"@id_{i}", bparam.Param0);
                    _cmd.Parameters.Add($"@data_{i}", DbType.Binary, bparam.Param1.Length).Value = bparam.Param1;
                }
            }
            else
            {
                foreach (var tparam in _textParams)
                {
                    _cmd.Parameters.Add(new SQLiteParameter { Value = tparam.Param0 });
                    _cmd.Parameters.Add(new SQLiteParameter { Value = tparam.Param1 });
                    _cmd.Parameters.Add(new SQLiteParameter { Value = tparam.Param2 });
                    _cmd.Parameters.Add(new SQLiteParameter { Value = tparam.Param3 });
                }                
            }

            _cmd.ExecuteNonQuery();

            _currentSize = 0;
            _currentAmount = 0;
            _currentCmdText = string.Empty;
            _textParams.Clear();
            _binParams.Clear();
        }

        private void AddParam(InserterParam iparam)
        {
            var textParams = iparam as TextParams;
            if (textParams != null)
            {
                _textParams.Add(textParams);

                _currentCmdText += _currentAmount == 0 ? _cmdText : ",";
                _currentCmdText += $" (?, ?, ?, ?, {_clientDatId})";

                _currentAmount++;
                return;
            }

            var binParams = iparam as BinParams;
            if (binParams != null)
            {
                _binParams.Add(binParams);

                _currentCmdText += _currentAmount == 0 ? _cmdText : ",";
                _currentCmdText += $" (@id_{_currentAmount}, @data_{_currentAmount}, {_clientDatId})";

                _currentAmount++;
            }          
        }
    }

    public class TextParams : InserterParam
    {
        public TextParams(uint p0, long p1, string p2, string p3)
        {
            Param0 = p0;
            Param1 = p1;
            Param2 = p2;
            Param3 = p3;
        }

        public override int TotalSize => UintSize + LongSize + Param2.Length * CharSize + Param3.Length * CharSize;

        public override uint Param0 { get; }

        public long Param1 { get; }

        public string Param2 { get; }

        public string Param3 { get; }
    }

    public class BinParams : InserterParam
    {
        public BinParams(uint p0, byte[] p1)
        {
            Param0 = p0;
            Param1 = p1;
        }

        public override uint Param0 { get; }

        public byte[] Param1 { get; }

        public override int TotalSize => UintSize + Param1.Length;
    }

    public abstract class InserterParam
    {
        protected const int UintSize = sizeof(uint);
        protected const int LongSize = sizeof(long);
        protected const int CharSize = sizeof(char);

        public abstract int TotalSize { get; }

        public abstract uint Param0 { get; }
    }
}
