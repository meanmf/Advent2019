using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Advent2019
{
    [TestFixture]
    public class Day21
    {
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

            var consoleWriter = new ConsoleWriter();
            intcode.OutputBlock.AsObservable().Subscribe(consoleWriter);
            var output = await intcode.RunAsync();
            await consoleWriter.Completion.Task;
            Assert.AreEqual(19355436, output.Last());
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

            var consoleWriter = new ConsoleWriter();
            intcode.OutputBlock.AsObservable().Subscribe(consoleWriter);
            var output = await intcode.RunAsync();
            await consoleWriter.Completion.Task;
            Assert.AreEqual(1142618405, output.Last());
        }
    }
}
