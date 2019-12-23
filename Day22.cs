using NUnit.Framework;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent2019
{
    [TestFixture]
    public class Day22
    {
        [Test]
        public void Silver()
        {
            var cards = Shuffle(10007, FileHelpers.EnumerateLines(@"Inputs\Day22.txt"));
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] == 2019)
                {
                    Assert.AreEqual(2322, i);
                    break;
                }
            }
        }

        [Test]
        public void Gold()
        {
            var commands = File.ReadAllLines(@"Inputs\Day22.txt");

            var deckSize = BigInteger.ValueOf(119315717514047);
            var targetCard = BigInteger.ValueOf(2020);
            var iterations = BigInteger.ValueOf(101741582076661);

            var positionZero = RunReverse(deckSize.LongValue, 0, commands);
            var positionOne = RunReverse(deckSize.LongValue, 1, commands);

            var offset = BigInteger.ValueOf(positionZero);
            var multiplier = BigInteger.ValueOf(deckSize.LongValue + positionOne - positionZero).Mod(deckSize);

            var solution = multiplier.ModPow(iterations, deckSize).Multiply(targetCard)
                .Add(offset.Multiply(
                    multiplier
                        .ModPow(iterations, deckSize)
                        .Subtract(BigInteger.One)
                        .Multiply(
                            multiplier
                                .Subtract(BigInteger.One)
                                .ModPow(deckSize.Subtract(BigInteger.Two), deckSize)
                ))).Add(deckSize).Mod(deckSize);

            Assert.AreEqual(49283089762689L, solution.LongValue);
        }

        static long[] Shuffle(long cardCount, IEnumerable<string> commands)
        {
            var cards = MakeDeck(cardCount);
            var newDeck = MakeDeck(cardCount);

            foreach (var line in commands)
            {
                if (line.StartsWith("deal with increment"))
                {
                    var increment = int.Parse(line.Substring(19));

                    for (int m = 0; m < cards.Length; m++)
                    {
                        newDeck[(m * increment) % cards.Length] = cards[m];
                    }

                }
                else if (line.StartsWith("deal into new stack"))
                {
                    for (int m = 0; m < cards.Length; m++)
                    {
                        newDeck[m] = cards[cards.Length - m - 1];
                    }
                }
                else if (line.StartsWith("cut"))
                {
                    int cut = int.Parse(line.Substring(4));
                    for (int m = 0; m < cards.Length; m++)
                    {
                        newDeck[m] = cards[(cards.Length + m + cut) % cards.Length];
                    }
                }
                else
                {
                    throw new InvalidOperationException("Unknown command: " + line);
                }

                (cards, newDeck) = (newDeck, cards);
            }

            return cards;
        }        

        static long RunReverse(long cardCount, long targetCard, IEnumerable<string> commands)
        {
            var bigCardCount = BigInteger.ValueOf(cardCount);

            foreach (var command in commands.Reverse())
            {
                if (command.StartsWith("deal with increment"))
                {
                    var increment = new BigInteger(command.Substring(20));
                    var inverse = increment.ModInverse(bigCardCount);

                    targetCard = BigInteger.ValueOf(targetCard)
                                    .Multiply(inverse)
                                    .Mod(bigCardCount)
                                    .LongValue;
                }
                else if (command.StartsWith("deal into new stack"))
                {
                    targetCard = cardCount - targetCard - 1;
                }
                else if (command.StartsWith("cut"))
                {
                    var cut = long.Parse(command.Substring(4));
                    targetCard = ((targetCard + cut) + cardCount) % cardCount;
                }
                else
                {
                    throw new InvalidOperationException("Unknown command: " + command);
                }
            }

            return targetCard;
        }
        
        static long[] MakeDeck(long cardCount)
        {
            var cards = new long[cardCount];
            for (long i = 0; i < cards.Length; i++)
            {
                cards[i] = i;
            }

            return cards;
        }
    }
}
