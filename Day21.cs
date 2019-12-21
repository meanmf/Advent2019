using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Advent2019
{
    [TestFixture]
    public class Day21
    {
        public static Task Main()
        {
            return new Day21().Gold();
        }

        readonly string _input = FileHelpers.GetSingle(@"Inputs\Day21.txt");

        [Test]
        public async Task Silver()
        {
            var mem = new FixedMemoryManager(8192);
            var intcode = new IntCode(_input, mem);

            var program =   "OR B T\n" +
                            "AND C T\n" +
                            "NOT T J\n" +
                            "AND D J\n" +
                            "NOT A T\n" +
                            "OR T J\n" +
                "WALK\n";

            intcode.Write(program);

            var outputs = await intcode.RunAsync();

            foreach (var c in outputs)
            {
                if (c <= 255)
                {
                    Console.Write((char)c);
                }
                else
                {
                    Console.Write(c);
                }
            }
        }

        [Test]
        public async Task Gold()
        {
            var mem = new FixedMemoryManager(8192);
            var intcode = new IntCode(_input, mem);

            var program = "OR B T\n" +
                            "AND C T\n" +
                            "NOT T J\n" +
                            "AND D J\n" +
                            "AND H J\n" +
                            "NOT A T\n" +
                            "OR T J\n" +
                            "RUN\n";

            intcode.Write(program);

            var outputs = await intcode.RunAsync();

            foreach (var c in outputs)
            {
                if (c <= 255)
                {
                    Console.Write((char)c);
                }
                else
                {
                    Console.Write(c);
                }
            }
        }
    }
}
