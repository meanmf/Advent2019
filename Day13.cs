using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Advent2019
{
    [TestFixture]
    public class Day13
    {
        static readonly string _input = FileHelpers.GetSingle("Inputs\\Day13.txt");

        [Test]
        public async Task Silver()
        {
            var points = new Dictionary<(long x, long y), int>();

            var intcode = new IntCode(_input, new FixedMemoryManager(4096));
            var output = await intcode.RunAsync();

            for (int i = 0; i < output.Count(); i += 3)
            {
                points.Add((output[i], output[i + 1]), (int)output[i + 2]);
            }

            Assert.AreEqual(296, points.Count(p => p.Value == 2));
        }

        static int _delay = 0;
        static bool _output = false;
        public static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                _delay = int.Parse(args[0]);
            }
            _output = true;
            await new Day13().Gold();
        }

        [Test]
        public async Task Gold()
        {
            var chars = new[] { ' ', 'X', 'O', '-', '.' };

            var mem = new FixedMemoryManager(4096);
            var inputProvider = new PipeInputProvider();
            var intcode = new IntCode(_input, mem, inputProvider);
            mem[0] = 2;

            var intcodeTask = intcode.RunAsync();

            int score = -1;
            if (_output) { Console.Clear(); }
            try
            {
                int ballX;
                int paddleX = -1;
                while (!intcode.OutputBlock.Completion.IsCompleted)
                {
                    var x = (int)await intcode.OutputBlock.ReceiveAsync();
                    var y = (int)await intcode.OutputBlock.ReceiveAsync();
                    var tile = (int)await intcode.OutputBlock.ReceiveAsync();

                    if (x == -1 && y == 0)
                    {
                        score = tile;
                        if (_output)
                        {
                            Console.SetCursorPosition(0, 26);
                            Console.WriteLine($"Score={score}");
                        }
                    }
                    else
                    {
                        if (_output)
                        {
                            Console.SetCursorPosition(x, y);
                            Console.Write(chars[tile]);
                        }

                        if (tile == 3) // paddle
                        {
                            paddleX = x;
                        }
                        else if (tile == 4) // ball
                        {
                            ballX = x;
                            if (_delay > 0)
                            {
                                await Task.Delay(_delay);
                            }

                            inputProvider.Post(Math.Sign(ballX - paddleX));
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
            }

            await intcodeTask;

            if (_output)
            {
                Console.SetCursorPosition(0, 26);
                Console.WriteLine($"Score={score}");
            }
            Assert.AreEqual(13824, score);
        }
    }
}
