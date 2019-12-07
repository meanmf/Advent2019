using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent2019
{
    [TestFixture]
    class Day2
    {
        #region input
        static readonly string _input = "1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,6,1,19,1,5,19,23,1,13,23,27,1,6,27,31,2,31,13,35,1,9,35,39,2,39,13,43,1,43,10,47,1,47,13,51,2,13,51,55,1,55,9,59,1,59,5,63,1,6,63,67,1,13,67,71,2,71,10,75,1,6,75,79,1,79,10,83,1,5,83,87,2,10,87,91,1,6,91,95,1,9,95,99,1,99,9,103,2,103,10,107,1,5,107,111,1,9,111,115,2,13,115,119,1,119,10,123,1,123,10,127,2,127,10,131,1,5,131,135,1,10,135,139,1,139,2,143,1,6,143,0,99,2,14,0,0";
        #endregion

        [Test]
        public void Silver()
        {
            var result = Run(12, 2);

            Assert.AreEqual(5290681, result);
        }

        [Test]
        public void Gold()
        {
            const int target = 19690720;

            for (int noun = 0; noun < 100; noun++)
            {
                for (int verb = 0; verb < 100; verb++)
                {
                    var result = Run(noun, verb);
                    if (result == target)
                    {
                        Assert.AreEqual(5741, 100 * noun + verb);
                        return;
                    }
                }
            }
        }

        int Run(int noun, int verb)
        {
            var ops = _input.Split(",").Select(int.Parse).ToArray();
            int ip = 0;
            bool done = false;

            ops[1] = noun;
            ops[2] = verb;

            while (!done)
            {
                switch (ops[ip])
                {
                    case 1: // Add
                        ops[ops[ip + 3]] = ops[ops[ip + 1]] + ops[ops[ip + 2]];
                        ip += 4;
                        break;
                    case 2: // Mult
                        ops[ops[ip + 3]] = ops[ops[ip + 1]] * ops[ops[ip + 2]];
                        ip += 4;
                        break;
                    case 99: // End
                        done = true;
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid opcode @ {ip}: {ops[ip]}");
                }
            }

            return ops[0];
        }
    }
}
