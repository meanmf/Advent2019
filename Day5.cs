using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2019
{
    [TestFixture]
    public class Day5
    {
        readonly string _program = FileHelpers.GetSingle("Inputs\\Day5.txt");

        [Test]
        public async Task Silver()
        {
            var inputProvider = new PipeInputProvider();
            var intcode = new IntCode(_program, inputProvider: inputProvider);
            inputProvider.Post(1);
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
            var inputProvider = new PipeInputProvider();
            var intcode = new IntCode(_program, inputProvider: inputProvider);
            inputProvider.Post(5);
            var outputs = await intcode.RunAsync();

            Assert.AreEqual(513116, outputs.Single());
        }
    }
}
