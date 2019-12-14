using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2019
{
    [TestFixture]
    public class Day14
    {
        [Test]
        public void Silver()
        {
            var reactor = new Reactor("Inputs\\Day14.txt");

            var oreUsed = reactor.Run(1);
            Assert.AreEqual(654909, oreUsed);
        }

        [Test]
        public void Gold()
        {
            var reactor = new Reactor("Inputs\\Day14.txt");

            long availableOre = 1_000_000_000_000L;
            long fuelLowerBound = 0;
            long fuelUpperBound = 100_000_000;
            long solution = 0;

            while (fuelLowerBound < fuelUpperBound-1)
            {
                long fuelOut = fuelLowerBound + (fuelUpperBound - fuelLowerBound) / 2;
                var oreUsed = reactor.Run(fuelOut);

                if (oreUsed < availableOre)
                {
                    solution = fuelOut;
                    fuelLowerBound = fuelOut;
                }
                else if (oreUsed > availableOre)
                {
                    fuelUpperBound = fuelOut;
                }
                else
                {
                    solution = fuelOut;
                    break;
                }
            }

            Assert.AreEqual(2876992, solution);
        }
    }

    class Reactor
    {
        readonly IReadOnlyDictionary<string, Reaction> _reactions;

        public Reactor(string filename)
        {
            var reactions = new Dictionary<string, Reaction>();

            foreach (var line in FileHelpers.ReadAllLines(filename))
            {
                var tokens = line.Split("=>");
                var inputs = tokens[0].Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                var outputs = tokens[1].Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                var reaction = new Reaction();
                for (int i = 0; i < inputs.Length; i += 2)
                {
                    var chem = new Chem(inputs[i + 1], long.Parse(inputs[i]));
                    reaction.Inputs.Add(chem);
                }

                reaction.Output = new Chem(outputs[1], long.Parse(outputs[0]));
                reactions.Add(reaction.Output.Name, reaction);
            }

            _reactions = reactions;
        }

        public long Run(long fuelAmount)
        {
            var stockpile = _reactions.ToDictionary(r => r.Key, r => 0L);
            var neededChems = new Queue<Chem>();
            neededChems.Enqueue(new Chem("FUEL", fuelAmount));

            long oreUsed = 0;
            while (neededChems.Any())
            {
                var neededChem = neededChems.Dequeue();
                var reaction = _reactions[neededChem.Name];

                var usedFromStockpile = Math.Min(stockpile[neededChem.Name], neededChem.Amount);
                var neededOutputAmount = neededChem.Amount - usedFromStockpile;
                stockpile[neededChem.Name] -= usedFromStockpile;

                var reactionTimes = (long)Math.Ceiling((decimal)neededOutputAmount / reaction.Output.Amount);
                var amountGenerated = reactionTimes * reaction.Output.Amount;
                stockpile[neededChem.Name] += amountGenerated - neededOutputAmount;

                foreach (var input in reaction.Inputs)
                {
                    var inputNeeded = input.Amount * reactionTimes;
                    if (input.Name == "ORE")
                    {
                        oreUsed += inputNeeded;
                    }
                    else
                    {
                        neededChems.Enqueue(new Chem(input.Name, inputNeeded));
                    }
                }
            }

            return oreUsed;
        }

        class Chem
        {
            public string Name { get; }
            public long Amount { get; }

            public Chem(string name, long amount)
            {
                Name = name;
                Amount = amount;
            }

            public override string ToString()
            {
                return $"{Amount} {Name}";
            }
        }

        class Reaction
        {
            public List<Chem> Inputs { get; } = new List<Chem>();
            public Chem Output { get; set; }

            public override string ToString()
            {
                return string.Join(", ", Inputs) + " => " + Output;
            }
        }
    }
}
