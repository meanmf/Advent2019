using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent2019
{
    class Image
    {
        readonly int _width;
        readonly int _height;
        readonly string _data;

        public Image(int width, int height, string input)
        {
            _width = width;
            _height = height;
            _data = input;
        }

        public char Get(int x, int y)
        {
            return _data[y * _width + x];
        }

        public int Count(char target)
        {
            return _data.Count(c => c == target);
        }

        public override string ToString()
        {
            var output = new StringBuilder();
            for (int y = 0; y < _height; y++)
            {
                output.AppendLine(
                    _data.Substring(y * _width, _width)
                        .Replace('0', ' ')
                        .Replace('1', 'X')
                        .Replace('2', '.'));
            }

            return output.ToString();
        }

        public static Image Merge(Image image1, Image image2)
        {
            var output = new StringBuilder();
            for (int i = 0; i < image1._data.Length; i++)
            {
                if (image1._data[i] == '2')
                {
                    output.Append(image2._data[i]);
                }
                else
                {
                    output.Append(image1._data[i]);
                }
            }

            return new Image(image1._width, image1._height, output.ToString());
        }
    }

    [TestFixture]
    public class Day8
    {
        readonly string _input = FileHelpers.GetSingle("Inputs\\Day8.txt");
        const int _width = 25;
        const int _height = 6;

        [Test]
        public void Silver()
        {
            int size = _width * _height;

            var images = new List<Image>();
            for (int i = 0; i < _input.Length; i += size)
            {
                var image = new Image(_width, _height, _input.Substring(i, size));
                images.Add(image);
            }

            Image leastZeroesImage = null;
            int leastZeroesCount = int.MaxValue;
            foreach (var image in images)
            {
                var zeroCount = image.Count('0');
                if (zeroCount < leastZeroesCount)
                {
                    leastZeroesImage = image;
                    leastZeroesCount = zeroCount;
                }
            }

            var onesCount = leastZeroesImage.Count('1');
            var twosCount = leastZeroesImage.Count('2');

            Assert.AreEqual(2080, onesCount * twosCount);
        }

        [Test]
        public void Gold()
        {
            string expected = " XX  X  X XXX   XX  X   X" + Environment.NewLine +
                              "X  X X  X X  X X  X X   X" + Environment.NewLine +
                              "X  X X  X X  X X     X X " + Environment.NewLine +
                              "XXXX X  X XXX  X      X  " + Environment.NewLine +
                              "X  X X  X X X  X  X   X  " + Environment.NewLine +
                              "X  X  XX  X  X  XX    X  " + Environment.NewLine;

            int size = _width * _height;

            var output = new Image(_width, _height, _input.Substring(0, size));
            for (int i = size; i < _input.Length; i += size)
            {
                var nextImage = new Image(_width, _height, _input.Substring(i, size));
                output = Image.Merge(output, nextImage);
            }

            Assert.AreEqual(expected, output.ToString());
        }
    }
}
