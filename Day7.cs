using CH.Combinatorics;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Advent2019
{
    [TestFixture]
    public class Day7
    {
        readonly string _input = FileHelpers.GetSingle("Inputs\\Day7.txt");

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
