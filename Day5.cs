using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Advent2019
{
    [TestFixture]
    public class Day5
    {
        readonly string _program = FileHelpers.GetSingle("Inputs\\Day5.txt");

        [Test]
        public async Task Silver()
        {
            var intcode = new IntCode(_program);
            intcode.InputBlock.Post(1);
            var outputs = await intcode.RunAsync();

            foreach (var output in outputs.SkipLast(1))
            {
                Assert.AreEqual(0, output);
            }

            Assert.AreEqual(5346030, outputs.Last());
        }

        [Test]
        public async Task Gold()
        {
            var intcode = new IntCode(_program);
            intcode.InputBlock.Post(5);
            var outputs = await intcode.RunAsync();

            Assert.AreEqual(513116, outputs.Single());
        }
    }
}
