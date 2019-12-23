using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent2019
{
    [TestFixture]
    public class Day3
    {
        static readonly IReadOnlyDictionary<char, (int x, int y)> _deltas = new Dictionary<char, (int, int)>
        {
            ['U'] = (0, -1),
            ['D'] = (0, 1),
            ['L'] = (-1, 0),
            ['R'] = (1, 0)
        };

        [Test]
        public void Silver()
        {
            var wires = FileHelpers.EnumerateLines("Inputs\\Day3.txt").Select(MakeWire).ToArray();

            var intersections = wires[0].Keys.Intersect(wires[1].Keys);
            var minDistance = intersections.Min(intersection => Math.Abs(intersection.x) + Math.Abs(intersection.y));

            Assert.AreEqual(1674, minDistance);
        }

        [Test]
        public void Gold()
        {
            var wires = FileHelpers.EnumerateLines("Inputs\\Day3.txt").Select(MakeWire).ToArray();

            var intersections = wires[0].Keys.Intersect(wires[1].Keys);
            var minSteps = intersections.Min(intersection => wires[0][intersection] + wires[1][intersection]);

            Assert.AreEqual(14012, minSteps);
        }

        static IReadOnlyDictionary<(int x, int y), int> MakeWire(string instructions)
        {
            var wire = new Dictionary<(int, int), int>();

            int stepNumber = 0;
            var currentLocation = (x: 0, y: 0);
            foreach (var segment in instructions.Split(","))
            {
                var distance = int.Parse(segment.Substring(1));
                var (deltaX, deltaY) = _deltas[segment[0]];

                for (int i = 0; i < distance; i++)
                {
                    currentLocation.x += deltaX;
                    currentLocation.y += deltaY;
                    stepNumber++;

                    wire.TryAdd(currentLocation, stepNumber);
                }
            }

            return wire;
        }
    }
}
