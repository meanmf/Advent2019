using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Advent2019
{
    [TestFixture]
    public class Day9
    {
        readonly string _input = FileHelpers.GetSingle("Inputs\\Day9.txt");

        [Test]
        public async Task TestCase1()
        {
            var intcode = new IntCode("1102,34915192,34915192,7,4,7,99,0");
            var output = await intcode.RunAsync();

            Assert.AreEqual(34915192L * 34915192L, output.Single());
        }

        [Test]
        public async Task TestCase2()
        {
            var intcode = new IntCode("104,1125899906842624,99");
            var output = await intcode.RunAsync();

            Assert.AreEqual(1125899906842624, output.Single());
        }

        [Test]
        public async Task TestCase3()
        {
            const string input = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
            var intcode = new IntCode(input);

            var expected = input.Split(",").Select(long.Parse);
            var output = await intcode.RunAsync();

            Assert.IsTrue(expected.SequenceEqual(output));
        }

        [Test]
        public async Task Silver()
        {
            var intcode = new IntCode(_input, new FixedMemoryManager(10000));
            intcode.InputBlock.Post(1);

            var output = await intcode.RunAsync();
            Assert.AreEqual(3429606717, output.Single());
        }

        [Test]
        public async Task Gold()
        {
            var intcode = new IntCode(_input, new FixedMemoryManager(10000));
            intcode.InputBlock.Post(2);

            var output = await intcode.RunAsync();
            Assert.AreEqual(33679, output.Single());
        }        
    }
}
