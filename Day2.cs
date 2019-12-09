using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2019
{
    [TestFixture]
    class Day2
    {
        readonly string _input = FileHelpers.GetSingle("Inputs\\Day2.txt");

        [Test]
        public async Task Silver()
        {
            var intcode = new IntCode(_input);
            intcode.Set(1, 12);
            intcode.Set(2, 2);
            await intcode.RunAsync();

            Assert.AreEqual(5290681, intcode.Get(0));
        }

        [Test]
        public async Task Gold()
        {
            const int target = 19690720;

            for (int noun = 0; noun < 100; noun++)
            {
                for (int verb = 0; verb < 100; verb++)
                {
                    var intcode = new IntCode(_input);
                    intcode.Set(1, noun);
                    intcode.Set(2, verb);
                    await intcode.RunAsync();

                    if (intcode.Get(0) == target)
                    {
                        Assert.AreEqual(5741, 100 * noun + verb);
                        return;
                    }
                }
            }
        }
    }
}
