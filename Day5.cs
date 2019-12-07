using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent2019
{
    [TestFixture]
    public class Day5
    {
        static readonly string _program = "3,225,1,225,6,6,1100,1,238,225,104,0,1101,81,30,225,1102,9,63,225,1001,92,45,224,101,-83,224,224,4,224,102,8,223,223,101,2,224,224,1,224,223,223,1102,41,38,225,1002,165,73,224,101,-2920,224,224,4,224,102,8,223,223,101,4,224,224,1,223,224,223,1101,18,14,224,1001,224,-32,224,4,224,1002,223,8,223,101,3,224,224,1,224,223,223,1101,67,38,225,1102,54,62,224,1001,224,-3348,224,4,224,1002,223,8,223,1001,224,1,224,1,224,223,223,1,161,169,224,101,-62,224,224,4,224,1002,223,8,223,101,1,224,224,1,223,224,223,2,14,18,224,1001,224,-1890,224,4,224,1002,223,8,223,101,3,224,224,1,223,224,223,1101,20,25,225,1102,40,11,225,1102,42,58,225,101,76,217,224,101,-153,224,224,4,224,102,8,223,223,1001,224,5,224,1,224,223,223,102,11,43,224,1001,224,-451,224,4,224,1002,223,8,223,101,6,224,224,1,223,224,223,1102,77,23,225,4,223,99,0,0,0,677,0,0,0,0,0,0,0,0,0,0,0,1105,0,99999,1105,227,247,1105,1,99999,1005,227,99999,1005,0,256,1105,1,99999,1106,227,99999,1106,0,265,1105,1,99999,1006,0,99999,1006,227,274,1105,1,99999,1105,1,280,1105,1,99999,1,225,225,225,1101,294,0,0,105,1,0,1105,1,99999,1106,0,300,1105,1,99999,1,225,225,225,1101,314,0,0,106,0,0,1105,1,99999,8,226,677,224,1002,223,2,223,1006,224,329,1001,223,1,223,7,226,226,224,102,2,223,223,1006,224,344,101,1,223,223,108,677,677,224,1002,223,2,223,1006,224,359,101,1,223,223,1107,226,677,224,1002,223,2,223,1005,224,374,101,1,223,223,1008,677,226,224,1002,223,2,223,1005,224,389,101,1,223,223,1007,677,226,224,1002,223,2,223,1005,224,404,1001,223,1,223,1107,677,226,224,1002,223,2,223,1005,224,419,1001,223,1,223,108,677,226,224,102,2,223,223,1006,224,434,1001,223,1,223,7,226,677,224,102,2,223,223,1005,224,449,1001,223,1,223,107,226,226,224,102,2,223,223,1006,224,464,101,1,223,223,107,677,226,224,102,2,223,223,1006,224,479,101,1,223,223,1007,677,677,224,1002,223,2,223,1006,224,494,1001,223,1,223,1008,226,226,224,1002,223,2,223,1006,224,509,101,1,223,223,7,677,226,224,1002,223,2,223,1006,224,524,1001,223,1,223,1007,226,226,224,102,2,223,223,1006,224,539,101,1,223,223,8,677,226,224,1002,223,2,223,1006,224,554,101,1,223,223,1008,677,677,224,102,2,223,223,1006,224,569,101,1,223,223,1108,677,226,224,102,2,223,223,1005,224,584,101,1,223,223,107,677,677,224,102,2,223,223,1006,224,599,1001,223,1,223,1108,677,677,224,1002,223,2,223,1006,224,614,1001,223,1,223,1107,677,677,224,1002,223,2,223,1005,224,629,1001,223,1,223,108,226,226,224,1002,223,2,223,1005,224,644,101,1,223,223,8,226,226,224,1002,223,2,223,1005,224,659,101,1,223,223,1108,226,677,224,1002,223,2,223,1006,224,674,101,1,223,223,4,223,99,226";

        [Test]
        public void Silver()
        {
            var intcode = new IntCode(_program, 1);
            var outputs = intcode.Run();

            foreach (var output in outputs.SkipLast(1))
            {
                Assert.AreEqual(0, output);
            }

            Assert.AreEqual(5346030, outputs.Last());
        }

        [Test]
        public void Gold()
        {
            var intcode = new IntCode(_program, 5);
            var outputs = intcode.Run();

            Assert.AreEqual(513116, outputs.Single());
        }
    }

    class IntCode
    {
        readonly int[] _ops;
        readonly int _inputValue;

        readonly List<int> _outputs = new List<int>();

        public IntCode(string program, int inputValue)
        {
            _ops = program.Split(",").Select(int.Parse).ToArray();
            _inputValue = inputValue;
        }

        int GetOpCode(int position)
        {
            return _ops[position] % 100;
        }

        int GetParam1(int position)
        {
            var mode = (_ops[position] / 100) % 10;
            if (mode == 0)
            {
                return _ops[_ops[position + 1]];
            }

            return _ops[position + 1];
        }

        string DebugParam1(int position)
        {
            var mode = (_ops[position] / 100) % 10;
            if (mode == 0)
            {
                return $"[{_ops[position + 1]}:{_ops[_ops[position+1]]}]";
            }

            return _ops[position + 1].ToString();
        }

        int GetParam2(int position)
        {
            var mode = (_ops[position] / 1000) % 10;
            if (mode == 0)
            {
                return _ops[_ops[position + 2]];
            }

            return _ops[position + 2];
        }

        string DebugParam2(int position)
        {
            var mode = (_ops[position] / 1000) % 10;
            if (mode == 0)
            {
                return $"[{_ops[position + 2]}:{_ops[_ops[position + 2]]}]";
            }

            return _ops[position + 2].ToString();
        }

        int GetInput()
        {
            return _inputValue;
        }

        void Output(int value)
        {
            _outputs.Add(value);
        }

        public IReadOnlyList<int> Run()
        {
            int ip = 0;
            bool done = false;

            while (!done)
            {
                switch (GetOpCode(ip))
                {
                    case 1: // Add
                        Console.WriteLine($"{ip}: [{_ops[ip+3]}] = {DebugParam1(ip)} + {DebugParam2(ip)}");
                        _ops[_ops[ip + 3]] = GetParam1(ip) + GetParam2(ip);
                        ip += 4;
                        break;
                    case 2: // Mult
                        Console.WriteLine($"{ip}: [{_ops[ip + 3]}] = {DebugParam1(ip)} * {DebugParam2(ip)}");
                        _ops[_ops[ip + 3]] = GetParam1(ip) * GetParam2(ip);
                        ip += 4;
                        break;
                    case 3: // Input
                        var input = GetInput();
                        Console.WriteLine($"{ip}: [{_ops[ip + 1]}] = Input({input})");
                        _ops[_ops[ip + 1]] = input;
                        ip += 2;
                        break;
                    case 4: // Output
                        Console.WriteLine($"{ip}: Output {DebugParam1(ip)}");
                        Output(GetParam1(ip));
                        ip += 2;
                        break;
                    case 5: // Jump if true
                        Console.WriteLine($"{ip}: if {DebugParam1(ip)} != 0 jmp {DebugParam2(ip)}");
                        if (GetParam1(ip) != 0)
                        {
                            ip = GetParam2(ip);
                        }
                        else
                        {
                            ip += 3;
                        }
                        break;
                    case 6: // Jump if false
                        Console.WriteLine($"{ip}: if {DebugParam1(ip)} == 0 jmp {DebugParam2(ip)}");
                        if (GetParam1(ip) == 0)
                        {
                            ip = GetParam2(ip);
                        }
                        else
                        {
                            ip += 3;
                        }
                        break;
                    case 7: // Less than
                        Console.WriteLine($"{ip}: [{_ops[ip + 3]}] = ({DebugParam1(ip)} < {DebugParam2(ip)})");
                        _ops[_ops[ip + 3]] = (GetParam1(ip) < GetParam2(ip) ? 1 : 0);
                        ip += 4;
                        break;
                    case 8: // Equals
                        Console.WriteLine($"{ip}: [{_ops[ip + 3]}] = ({DebugParam1(ip)} == {DebugParam2(ip)})");
                        _ops[_ops[ip + 3]] = (GetParam1(ip) == GetParam2(ip) ? 1 : 0);
                        ip += 4;
                        break;
                    case 99: // End
                        Console.WriteLine($"{ip}: End");
                        done = true;
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid opcode @ {ip}: {GetOpCode(ip)}");
                }
            }

            return _outputs;
        }
    }
}
