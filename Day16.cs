using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent2019
{
    [TestFixture]
    public class Day16
    {
        readonly string _input = FileHelpers.GetSingle(@"Inputs\\Day16.txt");

        [Test]
        public void Silver()
        {
            string inputText = _input;
            var inputs = new byte[inputText.Length];
            for (int i = 0; i < inputText.Length; i++)
            {
                inputs[i] = byte.Parse(inputText.Substring(i, 1));
            }

            for (int phase = 1; phase <= 100; phase++)
            {
                var outputs = new byte[inputs.Length];
                for (int outputDigit = 0; outputDigit < inputs.Length; outputDigit++)
                {
                    var pattern = Pattern(outputDigit + 1).Skip(1).GetEnumerator();
                    int total = 0;
                    for (int inputDigit = 0; inputDigit < inputs.Length; inputDigit++)
                    {
                        pattern.MoveNext();
                        total += inputs[inputDigit] * pattern.Current;
                    }
                    outputs[outputDigit] = (byte)Math.Abs(total % 10);
                }
                inputs = outputs;
            }

            var solution = int.Parse(string.Join("", inputs.Take(8)));
            Assert.AreEqual(68317988, solution);
        }

        [Test]
        public void Gold()
        {
            string inputText = _input;
            const int repeat = 10000;
            int offset = int.Parse(inputText.Substring(0, 7));

            var inputs = new byte[inputText.Length * repeat];
            for (int i = 0; i < inputText.Length; i++)
            {
                var digit = byte.Parse(inputText.Substring(i, 1));
                for (int a = 0; a < repeat; a++)
                {
                    inputs[(inputText.Length * a) + i] = digit;
                }
            }

            for (int loop = 0; loop < 100; loop++)
            {
                for (int outputDigit = inputs.Length - 2; outputDigit >= offset; outputDigit--)
                {
                    inputs[outputDigit] = (byte)((inputs[outputDigit] + inputs[outputDigit + 1]) % 10);
                }
            }

            var solution = int.Parse(string.Join("", inputs.Skip(offset).Take(8)));
            Assert.AreEqual(53850800, solution);
        }

        IEnumerable<int> Pattern(int phase)
        {
            var basePattern = new[] { 0, 1, 0, -1 };

            for (; ; )
            {
                foreach (var item in basePattern)
                {
                    for (int i = 0; i < phase; i++)
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
