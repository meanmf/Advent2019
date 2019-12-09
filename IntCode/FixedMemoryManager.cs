using System;
using System.Collections.Generic;
using System.Text;

namespace Advent2019
{
    class FixedMemoryManager : IMemoryManager
    {
        readonly long[] _mem;

        public FixedMemoryManager(int size)
        {
            _mem = new long[size];
        }

        public long this[long address]
        {
            get { return _mem[address]; }
            set { _mem[address] = value; }
        }
    }
}
