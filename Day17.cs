using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Advent2019
{
    [TestFixture]
    public class Day17
    {
        readonly string _input = FileHelpers.GetSingle(@"Inputs\Day17.txt");

        [Test]
        public async Task Silver()
        {
            var intcode = new IntCode(_input, new FixedMemoryManager(4096));
            var outputs = await intcode.RunAsync();

            const int xSize = 45;
            const int ySize = 35;
            var board = new char[xSize, ySize];

            var outputIterator = outputs.GetEnumerator();
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    outputIterator.MoveNext();
                    board[x, y] = (char)outputIterator.Current;
                }

                outputIterator.MoveNext();
            }

            int total = 0;
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    if (board[x, y] == '#')
                    {
                        int vert = Vert(x, y);
                        int horiz = Horiz(x, y);

                        if (vert == 2 && horiz == 2)
                        {
                            board[x, y] = 'O';
                            total += x * y;
                        }
                    }
                }
            }

            Assert.AreEqual(3448, total);

            int Horiz(int x, int y)
            {
                int count = 0;
                if (x > 0 && board[x - 1, y] == '#') count++;
                if (x < xSize - 1 && board[x + 1, y] == '#') count++;

                return count;
            }

            int Vert(int x, int y)
            {
                int count = 0;
                if (y > 0 && board[x, y - 1] == '#') count++;
                if (y < ySize - 1 && board[x, y + 1] == '#') count++;

                return count;
            }
        }

        [Test]
        public async Task Gold()
        {
            var mem = new FixedMemoryManager(4096);
            var intcode = new IntCode(_input, mem);

            mem[0] = 2;

            var inputs = new[] {
                "A,A,B,C,C,A,C,B,C,B",
                "L,4,L,4,L,6,R,10,L,6",
                "L,12,L,6,R,10,L,6",
                "R,8,R,10,L,6",
                "n" };

            foreach (var input in inputs)
            {
                foreach (var c in input)
                {
                    intcode.InputBlock.Post(c);
                }

                intcode.InputBlock.Post('\n');
            }

            var outputs = await intcode.RunAsync();
            Assert.AreEqual(762405, outputs.Last());
        }
    }
}
