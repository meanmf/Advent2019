using NUnit.Framework;
using Oc6.Lib.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent2019
{
    class Axis
    {
        public int Position;
        public int Velocity;
    }

    class Planet
    {
        public IReadOnlyDictionary<char, Axis> Axes;

        public Planet(string[] tokens)
        {
            var axes = new Dictionary<char, Axis>();
            for (int i = 0; i < tokens.Length; i += 2)
            {
                axes.Add(tokens[i].ToUpper()[0], new Axis { Position = int.Parse(tokens[i + 1]) });
            }

            Axes = axes;
        }

        public void Step()
        {
            foreach (var axis in Axes.Values)
            {
                axis.Position += axis.Velocity;
            }
        }

        public int KineticEnergy => Axes.Sum(a => Math.Abs(a.Value.Velocity));
        public int PotentialEnergy => Axes.Sum(a => Math.Abs(a.Value.Position));
    }

    class PlanetarySystem
    {
        readonly IReadOnlyList<Planet> _planets;

        public PlanetarySystem(string filename)
        {
            var planets = new List<Planet>();
            foreach (var line in FileHelpers.EnumerateLines(filename))
            {
                var tokens = line.Split(new[] { '<', '=', ' ', '>', ',' }, StringSplitOptions.RemoveEmptyEntries);
                var planet = new Planet(tokens);
                planets.Add(planet);
            }

            _planets = planets;
        }

        public IEnumerable<int> Snapshot(char axis)
        {
            return _planets.Select(p => p.Axes[axis].Position).Union(
                _planets.Select(p => p.Axes[axis].Velocity));
        }

        public IEnumerable<char> Axes
        {
            get { return _planets.First().Axes.Keys; }
        }

        public long Energy
        {
            get { return _planets.Sum(p => p.KineticEnergy * p.PotentialEnergy); }
        }

        public void Step()
        {
            for (int i = 0; i < _planets.Count; i++)
            {
                for (int j = i + 1; j < _planets.Count; j++)
                {
                    foreach (var axis in _planets[i].Axes.Keys)
                    {
                        int deltaV = Math.Sign(_planets[i].Axes[axis].Position - _planets[j].Axes[axis].Position);

                        _planets[i].Axes[axis].Velocity -= deltaV;
                        _planets[j].Axes[axis].Velocity += deltaV;
                    }
                }
            }

            foreach (var planet in _planets)
            {
                planet.Step();
            }
        }
    }

    [TestFixture]
    public class Day12
    {
        [Test]
        public void Silver()
        {
            var system = new PlanetarySystem("Inputs\\Day12.txt");

            for (int i = 0; i < 1000; i++)
            {
                system.Step();
            }

            Assert.AreEqual(9958, system.Energy);
        }

        [Test]
        public void Gold()
        {
            var system = new PlanetarySystem("Inputs\\Day12.txt");

            var initialStates = system.Axes.ToDictionary(axis => axis, axis => system.Snapshot(axis).ToArray());
            var periods = system.Axes.ToDictionary(axis => axis, axis => 0L);

            long turn = 0;
            while (periods.Any(p => p.Value == 0))
            {
                turn++;
                system.Step();

                foreach (var axis in system.Axes)
                {
                    if (periods[axis] == 0 && initialStates[axis].SequenceEqual(system.Snapshot(axis)))
                    {
                        periods[axis] = turn;
                    }
                }
            }

            Assert.AreEqual(318382803780324L, DivisorFunctions.LeastCommonMultiple(periods.Values.ToArray()));
        }
    }
}
