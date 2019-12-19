using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Advent2019
{
    [TestFixture]
    public class Day19
    {
        readonly IntCode _baseIntcode = new IntCode(
            FileHelpers.GetSingle(@"Inputs\Day19.txt"), new FixedMemoryManager(512));

        [Test]
        public async Task Silver()
        {
            int total = 0;
            for (int y = 0; y < 50; y++)
            {
                for (int x = 0; x < 50; x++)
                {
                    total += await GetPosAsync(x, y);
                }
            }

            Assert.AreEqual(173, total);
        }

        [Test]
        public async Task Gold()
        {
            const int shipSize = 100;

            int firstCol = 0;
            int answer = 0;

            for (int y = 10; answer == 0; y++)
            {
                int x = firstCol;

                while (await GetPosAsync(x, y) == 0)
                {
                    x++;
                }

                firstCol = x;

                while (await GetPosAsync(x + shipSize - 1, y) == 1)
                {
                    if (await GetPosAsync(x, y + shipSize - 1) == 1)
                    {
                        answer = x * 10000 + y;
                    }

                    x++;
                }
            }

            Assert.AreEqual(6671097, answer);
        }

        async Task<int> GetPosAsync(int x, int y)
        {
            var intcode = _baseIntcode.Fork();
            intcode.InputBlock.Post(x);
            intcode.InputBlock.Post(y);

            var output = await intcode.RunAsync();
            return (int)output.Single();
        }
    }
}
