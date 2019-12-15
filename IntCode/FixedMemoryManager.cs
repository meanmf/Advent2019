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

        FixedMemoryManager(long[] parent)
        {
            _mem = (long[])parent.Clone();
        }

        public long this[long address]
        {
            get { return _mem[address]; }
            set { _mem[address] = value; }
        }

        public IMemoryManager Fork()
        {
            var copy = new FixedMemoryManager(_mem);
            return copy;
        }
    }
}
