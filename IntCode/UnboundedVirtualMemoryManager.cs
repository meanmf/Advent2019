using System;
using System.Collections.Generic;
using System.Text;

namespace Advent2019
{
    class UnboundedVirtualMemoryManager : IMemoryManager
    {
        const int _pageBits = 10;
        const long _pageSize = 2 << _pageBits;
        readonly IDictionary<long, long[]> _pages = new Dictionary<long, long[]>();

        (long pageNumber, long offset) GetVirtualAddress(long address)
        {
            var pageNumber = address >> _pageBits;
            var offset = (int)(address % _pageSize);

            return (pageNumber, offset);
        }

        public long this[long address]
        {
            get
            {
                var (pageNumber, offset) = GetVirtualAddress(address);

                if (!_pages.TryGetValue(pageNumber, out var page))
                {
                    return 0L;
                }

                return page[offset];
            }

            set
            {
                var (pageNumber, offset) = GetVirtualAddress(address);

                if (!_pages.TryGetValue(pageNumber, out var page))
                {
                    page = new long[_pageSize];
                    _pages[pageNumber] = page;
                }

                page[offset] = value;
            }
        }
    }

}
