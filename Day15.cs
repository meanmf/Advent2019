using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NUnit.Framework;

namespace Advent2019
{
    [TestFixture]
    public class Day15
    {
        static bool _output = false;

        public static Task Main(string[] args)
        {
            _output = true;
            return new Day15().Solve();
        }

        readonly string _input = FileHelpers.GetSingle(@"Inputs\Day15.txt");

        void DumpBoard(int[,] board)
        {
            for (int y = 0; y < board.GetLength(1) - 1; y++)
            {
                for (int x = 0; x < board.GetLength(0) - 1; x++)
                {
                    DumpBoardPosition(board, x, y);
                }
            }
        }

        void DumpBoardPosition(int[,] board, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            switch (board[x, y])
            {
                case -4:
                    Console.Write("O");
                    break;
                case -3:
                    Console.Write("B");
                    break;
                case -2:
                    Console.Write("#");
                    break;
                case -1:
                    Console.Write("?");
                    break;
                default:
                    Console.Write(".");
                    break;
            }
        }

        [Test]
        public async Task Solve()
        {
            if (_output) Console.Clear();

            const int size = 42;
            var board = NewBoard(size);
            var mem = new FixedMemoryManager(2048);

            var intcode = new IntCode(_input, mem);
            var seeker = new Seeker { X = size / 2, Y = size / 2, Intcode = intcode };
            board[seeker.X, seeker.Y] = -3;

            if (_output) DumpBoard(board);

            var intcodeTask = intcode.RunAsync();

            var (silverSolution, goldSolution) = await RunAsync(seeker, board, _output);
            await intcodeTask;

            Assert.AreEqual(234, silverSolution);
            Assert.AreEqual(292, goldSolution);

            if (_output) Console.SetCursorPosition(0, size);
        }

        async Task<(int,int)> RunAsync(Seeker rootSeeker, int[,] board, bool output)
        {
            int silverSolution = 0;
            int maxDepth = 0;
            var seekers = new List<Seeker> { rootSeeker };
            var tasks = new List<Task>();
            Task<(int,int)> goldTask = null;
            int goldSolution = 0;

            while (seekers.Any())
            {
                if (output) await Task.Delay(3);
                var remainingSeekers = new List<Seeker>();
                foreach (var seeker in seekers)
                {
                    bool seekerReused = false;
                    for (int direction = 1; direction <= 4; direction++)
                    {
                        if (board[seeker.X + DeltaX(direction), seeker.Y + DeltaY(direction)] == -1)
                        {
                            Seeker newSeeker;
                            if (!seekerReused)
                            {
                                seekerReused = true;
                                newSeeker = seeker;
                            }
                            else
                            {
                                newSeeker = seeker.Clone();
                                tasks.Add(newSeeker.Intcode.RunAsync());
                            }

                            newSeeker.Direction = direction;
                            remainingSeekers.Add(newSeeker);
                        }
                    }

                    if (!seekerReused)
                    {
                        seeker.Intcode.Terminate();
                        board[seeker.X, seeker.Y] = seeker.Depth;
                        if (output) DumpBoardPosition(board, seeker.X, seeker.Y);
                    }
                }

                seekers = remainingSeekers;
                var nextSeekers = new List<Seeker>();
                foreach (var seeker in seekers)
                {
                    seeker.Intcode.InputBlock.Post(seeker.Direction);
                    var response = await seeker.Intcode.OutputBlock.ReceiveAsync();

                    board[seeker.X, seeker.Y] = seeker.Depth;
                    if (output) DumpBoardPosition(board, seeker.X, seeker.Y);

                    if (response == 0)
                    {
                        board[seeker.X + DeltaX(seeker.Direction), seeker.Y + DeltaY(seeker.Direction)] = -2;
                        seeker.Intcode.Terminate();
                        if (output) DumpBoardPosition(board, seeker.X + DeltaX(seeker.Direction), seeker.Y + DeltaY(seeker.Direction));
                    }
                    else
                    {
                        seeker.Move();
                        maxDepth = Math.Max(maxDepth, seeker.Depth);
                        if (response == 1)
                        {
                            board[seeker.X, seeker.Y] = -3;
                            nextSeekers.Add(seeker);
                        }
                        else
                        { 
                            silverSolution = seeker.Depth;
                            board[seeker.X, seeker.Y] = -4;
                            if (output) DumpBoardPosition(board, seeker.X, seeker.Y);
                            seeker.Intcode.Terminate();

                            var goldBoard = NewBoard(board.GetLength(0));
                            goldBoard[seeker.X, seeker.Y] = 0;
                            var goldSeeker = seeker.Clone();
                            goldSeeker.Depth = 0;
                            tasks.Add(goldSeeker.Intcode.RunAsync());
                            goldTask = RunAsync(goldSeeker, goldBoard, false);
                        }

                        if (output) DumpBoardPosition(board, seeker.X, seeker.Y);
                    }

                    seekers = nextSeekers;
                }
            }

            await Task.WhenAll(tasks);
            if (goldTask != null)
            {
                (_, goldSolution) = await goldTask;
            }

            return (silverSolution, goldSolution > 0 ? goldSolution : maxDepth);
        }

        int[,] NewBoard(int size)
        {
            var board = new int[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    board[x, y] = -1;
                }
            }

            return board;
        }


        static int DeltaX(int direction)
        {
            if (direction == 3) return -1;
            if (direction == 4) return 1;
            return 0;
        }

        static int DeltaY(int direction)
        {
            if (direction == 1) return -1;
            if (direction == 2) return 1;
            return 0;
        }

        class Seeker
        {
            public int X;
            public int Y;
            public int Direction;
            public IntCode Intcode;
            public int Depth;

            public Seeker Clone()
            {
                return new Seeker
                {
                    X = X,
                    Y = Y,
                    Depth = Depth,
                    Intcode = Intcode.Fork(),
                };
            }

            public void Move()
            {
                X += DeltaX(Direction);
                Y += DeltaY(Direction);
                Depth++;
            }
        }
    }
}
