using System;
using System.Collections.Generic;
using System.Text;

namespace Advent2019
{
    interface IMemoryManager
    {
        long this[long address] { get; set; }

        IMemoryManager Fork();
    }
}
