using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Advent2019
{
    [TestFixture]
    public class Day1
    {
        [Test]
        public void Silver()
        {
            var totalFuel = FileHelpers.ReadAllLines("Day1.txt").Sum(line => CalcFuel(int.Parse(line)));

            Assert.AreEqual(3297626, totalFuel);
        }

        [Test]
        public void Gold()
        {
            var totalFuel = FileHelpers.ReadAllLines("Day1.txt").Select(int.Parse).Sum(mass =>
            {
                int addedMass = 0;
                for (; ; )
                {
                    mass = CalcFuel(mass);
                    if (mass <= 0) break;

                    addedMass += mass;
                }

                return addedMass;
            });

            Assert.AreEqual(4943578, totalFuel);
        }

        static int CalcFuel(int mass)
        {
            return (int)Math.Floor((decimal)mass / 3) - 2;
        }
    }
}
