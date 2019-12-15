using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Advent2019
{
    class PainterBot
    {
        const int gridSize = 250;

        readonly string _input = FileHelpers.GetSingle("Inputs\\Day11.txt");
        readonly int _startValue;

        readonly int[,] _grid = new int[gridSize, gridSize];
        readonly HashSet<(int x, int y)> _paintedSpaces = new HashSet<(int, int)>();

        int _minX = int.MaxValue;
        int _maxX = int.MinValue;
        int _minY = int.MaxValue;
        int _maxY = int.MinValue;

        public PainterBot(int startValue)
        {
            _startValue = startValue;
        }

        public int PaintedSpaceCount
        {
            get => _paintedSpaces.Count();
        }

        public string DumpOutput()
        {
            var output = new StringBuilder();

            for (int y = _minY; y <= _maxY; y++)
            {
                for (int xx = _minX; xx <= _maxX; xx++)
                {
                    output.Append(_grid[xx, y] == 1 ? 'X' : ' ');
                }

                output.AppendLine();
            }

            return output.ToString();
        }

        public async Task RunAsync()
        { 
            int x = gridSize / 2;
            int y = gridSize / 2;
            int direction = 0;

            var cancel = new CancellationTokenSource();
            var intcode = new IntCode(_input, new FixedMemoryManager(2048));
            var intcodeTask = intcode.RunAsync();

            try
            {
                _grid[x, y] = _startValue;
                while (!intcode.OutputBlock.Completion.IsCompleted)
                {
                    intcode.InputBlock.Post(_grid[x, y]);

                    var paint = await intcode.OutputBlock.ReceiveAsync();
                    if (paint == 1)
                    {
                        if (x < _minX) _minX = x;
                        if (y < _minY) _minY = y;
                        if (x > _maxX) _maxX = x;
                        if (y > _maxY) _maxY = y;
                        _paintedSpaces.Add((x, y));
                    }

                    _grid[x, y] = (int)paint;

                    var turn = await intcode.OutputBlock.ReceiveAsync(cancel.Token);
                    switch (turn)
                    {
                        case 0:
                            direction = (direction + 270) % 360;
                            break;
                        case 1:
                            direction = (direction + 90) % 360;
                            break;
                        default:
                            throw new InvalidOperationException("Invalid output: " + turn);
                    }

                    switch (direction)
                    {
                        case 0:
                            y--;
                            break;
                        case 90:
                            x++;
                            break;
                        case 180:
                            y++;
                            break;
                        case 270:
                            x--;
                            break;
                    }
                }
            }
            catch (InvalidOperationException)
            {
            }

            await intcodeTask;
        }
    }

    [TestFixture]
    public class Day11
    {

        [Test]
        public async Task Silver()
        {
            var bot = new PainterBot(0);
            await bot.RunAsync();

            Assert.AreEqual(1863, bot.PaintedSpaceCount);
        }

        [Test]
        public async Task Gold()
        {
            var bot = new PainterBot(1);
            await bot.RunAsync();

            var expected = @"XXX  X    X  X X    XXXX   XX X    XXXX
X  X X    X  X X       X    X X       X
XXX  X    X  X X      X     X X      X 
X  X X    X  X X     X      X X     X  
X  X X    X  X X    X    X  X X    X   
XXX  XXXX  XX  XXXX XXXX  XX  XXXX XXXX
";

            Assert.AreEqual(expected, bot.DumpOutput());
        }
    }
}
