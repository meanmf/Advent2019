using CH.Combinatorics;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Advent2019
{
    [TestFixture]
    public class Day7
    {
        #region input
        const string _input = "3,8,1001,8,10,8,105,1,0,0,21,38,63,80,105,118,199,280,361,442,99999,3,9,102,5,9,9,1001,9,3,9,1002,9,2,9,4,9,99,3,9,1001,9,4,9,102,4,9,9,101,4,9,9,102,2,9,9,101,2,9,9,4,9,99,3,9,1001,9,5,9,102,4,9,9,1001,9,4,9,4,9,99,3,9,101,3,9,9,1002,9,5,9,101,3,9,9,102,5,9,9,101,3,9,9,4,9,99,3,9,1002,9,2,9,1001,9,4,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,99,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,99";
        #endregion

        [Test]
        public async Task Silver()
        {
            var inputs = new[] { 0, 1, 2, 3, 4 };
            var tasks = inputs.Permute().Select(async phases =>
            {
                long input = 0;
                for (int i = 0; i < phases.Count(); i++)
                {
                    var intcode = new IntCode(_input);
                    intcode.AddInput(phases.ElementAt(i));
                    intcode.AddInput(input);

                    var output = await intcode.RunAsync();
                    input = output.Single();
                }

                return input;
            });

            var results = await Task.WhenAll(tasks);
            Assert.AreEqual(118936, results.Max());
        }

        [Test]
        public async Task Gold()
        {
            var inputs = new[] { 5, 6, 7, 8, 9 };
            var tasks = inputs.Permute().Select(async phases =>
            {
                var amps = new IntCode[phases.Count()];
                for (int i = 0; i < amps.Length; i++)
                {
                    amps[i] = new IntCode(_input);
                }

                for (int i = 0; i < amps.Length; i++)
                {
                    amps[i].AddInput(phases.ElementAt(i));
                    amps[i].PipeTo = amps[(i + 1) % 5];
                }

                amps[0].AddInput(0);

                var tasks = amps.Select(a => a.RunAsync()).ToArray();
                var output = await tasks.Last();
                return output.Last();
            });

            var results = await Task.WhenAll(tasks);
            Assert.AreEqual(57660948, results.Max());
        }
    }
}
