using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent2019
{
    [TestFixture]
    public class Day24
    {
        [Test]
        public void Silver()
        {
            var board = new Board(@"Inputs\Day24.txt");
            var positions = new HashSet<int>();

            int Run()
            {
                for (; ; )
                {
                    var hash = board.GetHashCode();
                    if (positions.Contains(hash))
                    {
                        return hash;
                    }

                    positions.Add(hash);

                    board.StepGold();
                    board.Swap();
                }
            }

            var solution = Run();
            Assert.AreEqual(32523825, solution);
        }

        [Test]
        public void Gold()
        {
            const int moveCount = 200;
            var boards = new Board[(moveCount + 1) * 2];

            for (int i = 0; i < boards.Length; i++)
            {
                boards[i] = new Board();
            }

            boards[boards.Length / 2] = new Board(@"Inputs\Day24.txt");

            for (int i = 1; i < boards.Length; i++)
            {
                boards[i].Up = boards[i - 1];
            }

            for (int i = 0; i < boards.Length - 1; i++)
            {
                boards[i].Down = boards[i + 1];
            }

            for (int move = 0; move < moveCount; move++)
            {
                for (int i = 0; i < boards.Length; i++)
                {
                    boards[i].StepGold();
                }

                for (int i = 0; i < boards.Length; i++)
                {
                    boards[i].Swap();
                }
            }

            var total = boards.Sum(board => board.BugCount);
            Assert.AreEqual(2052, total);
        }

        class Board
        {
            bool[,] _board = new bool[5, 5];
            bool[,] _nextBoard = new bool[5, 5];

            public Board Down { get; set; }
            public Board Up { get; set; }

            public Board()
            {
            }
            
            public Board(string filename)
            {
                int y = 0;

                foreach (var line in FileHelpers.EnumerateLines(filename))
                {
                    int x = 0;
                    foreach (var c in line)
                    {
                        if (c == '#')
                        {
                           _board[x, y] = true;
                        }
                        x++;
                    }
                    y++;
                }
            }

            public void Swap()
            {
                (_nextBoard, _board) = (_board, _nextBoard);
            }

            public int LeftOuter => Enumerable.Range(0, 5).Sum(y => _board[0, y] ? 1 : 0);
            public int LeftInner => _board[1, 2] ? 1 : 0;

            public int RightOuter => Enumerable.Range(0, 5).Sum(y => _board[4, y] ? 1 : 0);
            public int RightInner => _board[3, 2] ? 1 : 0;

            public int TopOuter => Enumerable.Range(0, 5).Sum(x => _board[x, 0] ? 1 : 0);
            public int TopInner => _board[2, 1] ? 1 : 0;

            public int BottomOuter => Enumerable.Range(0, 5).Sum(x => _board[x, 4] ? 1 : 0);
            public int BottomInner => _board[2, 3] ? 1 : 0;

            public void StepGold()
            {
                for (int y = 0; y < 5; y++)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        int count = 0;
                        if (x == 2 && y == 2 && (Up != null || Down != null))
                        {
                            continue;
                        }

                        if (x > 0 && _board[x - 1, y]) count++;
                        if (x < 4 && _board[x + 1, y]) count++;
                        if (y > 0 && _board[x, y - 1]) count++;
                        if (y < 4 && _board[x, y + 1]) count++;

                        if (Down != null)
                        {
                            if (x == 2 && y == 1)
                            {
                                count += Down.TopOuter;
                            }
                            else if (x == 2 && y == 3)
                            {
                                count += Down.BottomOuter;
                            }
                            else if (x == 1 && y == 2)
                            {
                                count += Down.LeftOuter;
                            }
                            else if (x == 3 && y == 2)
                            {
                                count += Down.RightOuter;
                            }
                        }

                        if (Up != null)
                        {
                            if (x == 0)
                            {
                                count += Up.LeftInner;
                            }
                            else if (x == 4)
                            {
                                count += Up.RightInner;
                            }

                            if (y == 0)
                            {
                                count += Up.TopInner;
                            }
                            else if (y == 4)
                            {
                                count += Up.BottomInner;
                            }
                        }

                        if (_board[x, y])
                        {
                            if (count == 1)
                            {
                                _nextBoard[x, y] = true;
                            }
                            else
                            {
                                _nextBoard[x, y] = false;
                            }
                        }
                        else
                        {
                            if (count == 1 || count == 2)
                            {
                                _nextBoard[x, y] = true;
                            }
                            else
                            {
                                _nextBoard[x, y] = false;
                            }
                        }
                    }
                }
            }

            public override int GetHashCode()
            {
                int hash = 0;
                for (int y = 4; y >= 0; y--)
                {
                    for (int x = 4; x >= 0; x--)
                    {
                        hash <<= 1;
                        if (_board[x, y])
                        {
                            hash++;
                        }
                    }
                }

                return hash;
            }

            public int BugCount
            {
                get
                {
                    int count = 0;
                    for (int y = 0; y < 5; y++)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            if (y == 2 && x == 2) continue;
                            count += _board[x, y] ? 1 : 0;
                        }
                    }

                    return count;
                }
            }
        }
    }
}
