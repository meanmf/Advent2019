using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Advent2019
{
    [TestFixture]
    public class Day10
    {
        HashSet<(int x, int y)> ReadMap()
        {
            var map = new HashSet<(int x, int y)>();

            using var inputFile = new StreamReader(File.OpenRead("Inputs\\Day10.txt"));

            int y = 0;
            while (!inputFile.EndOfStream)
            {
                var line = inputFile.ReadLine();
                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        map.Add((x, y));
                    }
                }

                y++;
            }

            return map;
        }

        [Test]
        public void Silver()
        {
            var map = ReadMap();

            int max = int.MinValue;
            foreach (var point in map)
            {
                var tans = new HashSet<decimal>();
                foreach (var otherPoint in map)
                {
                    if (point == otherPoint) continue;

                    var slope = (decimal)Math.Atan2(point.y - otherPoint.y, point.x - otherPoint.x);
                    tans.Add(slope);
                }

                max = Math.Max(max, tans.Count());
            }

            Assert.AreEqual(314, max);
        }

        class PointData
        {
            public int X { get; }
            public int Y { get; }
            public double Tan { get; }
            public double Distance { get; }

            public PointData(int x, int y, double tan, double distance)
            {
                X = x;
                Y = y;
                Tan = tan;
                Distance = distance;
            }
        }

        [Test]
        public void Gold()
        {
            var laserBase = (x: 27, y: 19);

            var map = ReadMap();
            map.Remove(laserBase);

            var data = new List<PointData>();
            foreach (var (x, y) in map)
            {
                var tan = Math.Atan2(laserBase.y - y, laserBase.x - x);
                var distance = Math.Sqrt(Math.Pow(laserBase.y - y, 2) + Math.Pow(laserBase.x - x, 2));
                data.Add(new PointData(x, y, tan, distance));
            }

            var sortedPoints = data.OrderBy(d => d.Tan).ThenBy(d => d.Distance).ToList();

            int index = 0;
            while (sortedPoints[index].Tan < Math.PI / 2)
            {
                index++;
            }

            for (int count = 0; count < 199; count++)
            {
                var nextIndex = index;
                
                do
                {
                    nextIndex = (nextIndex + 1) % sortedPoints.Count;
                }
                while (sortedPoints[nextIndex] == null || sortedPoints[nextIndex].Tan == sortedPoints[index].Tan);

                sortedPoints[index] = null;
                index = nextIndex;
            }

            Assert.AreEqual(1513, sortedPoints[index].X * 100 + sortedPoints[index].Y);
        }
    }
}
