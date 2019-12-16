using System;

namespace Advent2019
{
    class BoundedVirtualMemoryManager : IMemoryManager
    {
        const int _pageBits = 16;
        const long _pageSize = 1 << _pageBits;
        readonly long[][] _pages;

        public BoundedVirtualMemoryManager(long capacity)
        {
            var pageCount = (long)Math.Ceiling((decimal)capacity / _pageSize);
            _pages = new long[pageCount][];
        }

        BoundedVirtualMemoryManager(BoundedVirtualMemoryManager original)
        {
            _pages = new long[original._pages.Length][];
            for (int i=0;i<_pages.Length; i++)
            {
                _pages[i] = (long[])original._pages[i].Clone();
            }
        }

        (long pageNumber, long offset) GetVirtualAddress(long address)
        {
            var pageNumber = address >> _pageBits;
            var offset = (int)(address % _pageSize);

            return (pageNumber, offset);
        }

        public IMemoryManager Fork()
        {
            return new BoundedVirtualMemoryManager(this);
        }

        public long this[long address]
        {
            get
            {
                var (pageNumber, offset) = GetVirtualAddress(address);

                if (_pages[pageNumber] == null)
                {
                    return 0L;
                }

                return _pages[pageNumber][offset];
            }

            set
            {
                var (pageNumber, offset) = GetVirtualAddress(address);

                if (_pages[pageNumber] == null)
                {
                    _pages[pageNumber] = new long[_pageSize];
                }

                _pages[pageNumber][offset] = value;
            }
        }
    }
}
