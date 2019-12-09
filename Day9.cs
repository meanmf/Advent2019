using NUnit.Framework;
using System;
using System.Linq;

namespace Advent2019
{
    [TestFixture]
    public class Day9
    {
        readonly string _input = FileHelpers.GetSingle("Inputs\\Day9.txt");

        [Test]
        public void TestCase1()
        {
            var intcode = new IntCode("1102,34915192,34915192,7,4,7,99,0");
            var output = intcode.RunAsync().Result;

            Assert.AreEqual(34915192L * 34915192L, output.Single());
        }

        [Test]
        public void TestCase2()
        {
            var intcode = new IntCode("104,1125899906842624,99");
            var output = intcode.RunAsync().Result;

            Assert.AreEqual(1125899906842624, output.Single());
        }

        [Test]
        public void TestCase3()
        {
            const string input = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
            var intcode = new IntCode(input);

            var expected = input.Split(",").Select(long.Parse);
            var output = intcode.RunAsync().Result;

            Assert.IsTrue(expected.SequenceEqual(output));
        }

        [Test]
        public void Silver()
        {
            var comp = new IntCode(_input, new BoundedVirtualMemoryManager(10000));
            comp.AddInput(1);

            var output = comp.RunAsync().Result;
            Assert.AreEqual(3429606717, output.Single());
        }

        [Test]
        public void Gold()
        {
            var comp = new IntCode(_input, new BoundedVirtualMemoryManager(10000));
            comp.AddInput(2);

            var output = comp.RunAsync().Result;
            Assert.AreEqual(33679, output.Single());
        }        
    }
}
