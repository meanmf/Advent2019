using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Advent2019
{
    [TestFixture]
    public class Day14
    {
        #region TestInputs
        const string _testInput1 = @"157 ORE => 5 NZVS
165 ORE => 6 DCFZ
44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL
12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ
179 ORE => 7 PSHF
177 ORE => 5 HKGWZ
7 DCFZ, 7 PSHF => 2 XJWVT
165 ORE => 2 GPVTF
3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT";

        const string _testInput2 = @"2 VPVL, 7 FWMGM, 2 CXFTF, 11 MNCFX => 1 STKFG
17 NVRVD, 3 JNWZP => 8 VPVL
53 STKFG, 6 MNCFX, 46 VJHF, 81 HVMC, 68 CXFTF, 25 GNMV => 1 FUEL
22 VJHF, 37 MNCFX => 5 FWMGM
139 ORE => 4 NVRVD
144 ORE => 7 JNWZP
5 MNCFX, 7 RFSQX, 2 FWMGM, 2 VPVL, 19 CXFTF => 3 HVMC
5 VJHF, 7 MNCFX, 9 VPVL, 37 CXFTF => 6 GNMV
145 ORE => 6 MNCFX
1 NVRVD => 8 CXFTF
1 VJHF, 6 MNCFX => 4 RFSQX
176 ORE => 6 VJHF";

        const string _testInput3 = @"171 ORE => 8 CNZTR
7 ZLQW, 3 BMBT, 9 XCVML, 26 XMNCP, 1 WPTQ, 2 MZWV, 1 RJRHP => 4 PLWSL
114 ORE => 4 BHXH
14 VRPVC => 6 BMBT
6 BHXH, 18 KTJDG, 12 WPTQ, 7 PLWSL, 31 FHTLT, 37 ZDVW => 1 FUEL
6 WPTQ, 2 BMBT, 8 ZLQW, 18 KTJDG, 1 XMNCP, 6 MZWV, 1 RJRHP => 6 FHTLT
15 XDBXC, 2 LTCX, 1 VRPVC => 6 ZLQW
13 WPTQ, 10 LTCX, 3 RJRHP, 14 XMNCP, 2 MZWV, 1 ZLQW => 1 ZDVW
5 BMBT => 4 WPTQ
189 ORE => 9 KTJDG
1 MZWV, 17 XDBXC, 3 XCVML => 2 XMNCP
12 VRPVC, 27 CNZTR => 2 XDBXC
15 KTJDG, 12 BHXH => 5 XCVML
3 BHXH, 2 VRPVC => 7 MZWV
121 ORE => 7 VRPVC
7 XCVML => 6 RJRHP
5 BHXH, 4 VRPVC => 5 LTCX";
        #endregion

        [Test, Sequential]
        public void Part1(
            [Values(_testInput1, _testInput2, _testInput3)] string input,
            [Values(13312, 180697, 2210736)] long answer)
        {
            var inputBytes = new MemoryStream(Encoding.ASCII.GetBytes(input));
            var reactor = new Reactor(new StreamReader(inputBytes));
            var oreUsed = reactor.Run(1);
            Assert.AreEqual(answer, oreUsed);
        }

        [Test, Sequential]
        public void Part2(
            [Values(_testInput1, _testInput2, _testInput3)] string input,
            [Values(82892753L, 5586022L, 460664L)] long answer)
        {
            var inputBytes = new MemoryStream(Encoding.ASCII.GetBytes(input));
            var reactor = new Reactor(new StreamReader(inputBytes));
            var maxFuel = reactor.FindMaxFuel();
            Assert.AreEqual(answer, maxFuel);
        }

        [Test]
        public void Silver()
        {
            using var input = new StreamReader(@"Inputs\Day14.txt");
            var reactor = new Reactor(input);

            var oreUsed = reactor.Run(1);
            Assert.AreEqual(654909, oreUsed);
        }

        [Test]
        public void Gold()
        {
            using var input = new StreamReader(@"Inputs\Day14.txt");
            var reactor = new Reactor(input);

            var maxFuel = reactor.FindMaxFuel();
            Assert.AreEqual(2876992, maxFuel);
        }
    }

    class Reactor
    {
        readonly IReadOnlyDictionary<string, Reaction> _reactions;

        public Reactor(StreamReader inputStream)
        {
            var reactions = new Dictionary<string, Reaction>();

            while (!inputStream.EndOfStream)
            {
                var line = inputStream.ReadLine();

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

        public long FindMaxFuel()
        {
            long availableOre = 1_000_000_000_000L;
            long fuelLowerBound = 0;
            long fuelUpperBound = 100_000_000;
            long solution = 0;

            while (fuelLowerBound < fuelUpperBound - 1)
            {
                long fuelOut = fuelLowerBound + (fuelUpperBound - fuelLowerBound) / 2;
                var oreUsed = Run(fuelOut);

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

            return solution;
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
