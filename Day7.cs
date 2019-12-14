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
                    var inputProvider = new PipeInputProvider();
                    var intcode = new IntCode(_input, inputProvider: inputProvider);
                    inputProvider.Post(phases.ElementAt(i));
                    inputProvider.Post(input);

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
                var inputProviders = new PipeInputProvider[phases.Count()];
                var amps = new IntCode[phases.Count()];
                for (int i = 0; i < amps.Length; i++)
                {
                    inputProviders[i] = new PipeInputProvider();
                    amps[i] = new IntCode(_input, inputProvider: inputProviders[i]);
                }

                for (int i = 0; i < amps.Length; i++)
                {
                    inputProviders[i].Post(phases.ElementAt(i));
                    amps[i].PipeTo = inputProviders[(i + 1) % 5];
                }

                inputProviders[0].Post(0);

                var tasks = amps.Select(a => a.RunAsync()).ToArray();
                var output = await tasks.Last();
                return output.Last();
            });

            var results = await Task.WhenAll(tasks);
            Assert.AreEqual(57660948, results.Max());
        }
    }
}
