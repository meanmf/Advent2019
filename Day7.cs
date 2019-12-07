using CH.Combinatorics;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Advent2019
{
    [TestFixture]
    public class Day7
    {
        #region input
        const string _input = "3,8,1001,8,10,8,105,1,0,0,21,38,63,80,105,118,199,280,361,442,99999,3,9,102,5,9,9,1001,9,3,9,1002,9,2,9,4,9,99,3,9,1001,9,4,9,102,4,9,9,101,4,9,9,102,2,9,9,101,2,9,9,4,9,99,3,9,1001,9,5,9,102,4,9,9,1001,9,4,9,4,9,99,3,9,101,3,9,9,1002,9,5,9,101,3,9,9,102,5,9,9,101,3,9,9,4,9,99,3,9,1002,9,2,9,1001,9,4,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,99,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,99";
        #endregion

        [Test]
        public async Task Silver()
        {
            var inputs = Enumerable.Range(0, 5);
            var tasks = inputs.Permute().AsParallel().Select(async phases =>
            {
                int input = 0;
                for (int i = 0; i < phases.Count(); i++)
                {
                    var intcode = new IntCode(_input);
                    intcode.AddInput(phases.ElementAt(i));
                    intcode.AddInput(input);

                    var output = await intcode.RunAsync();
                    input = output.Single();
                }

                return input;
            });

            var results = await Task.WhenAll(tasks);
            Assert.AreEqual(118936, results.Max());
        }

        [Test]
        public async Task Gold()
        {
            var inputs = Enumerable.Range(5, 5);
            var tasks = inputs.Permute().AsParallel().Select(async phases =>
            {
                var amps = new IntCode[phases.Count()];
                for (int i = 0; i < amps.Length; i++)
                {
                    amps[i] = new IntCode(_input);
                    amps[i].AddInput(phases.ElementAt(i));
                }

                for (int i = 0; i < amps.Length; i++)
                {
                    amps[i].PipeTo = amps[(i + 1) % 5];
                }

                amps[0].AddInput(0);

                var tasks = amps.Select(a => a.RunAsync()).ToArray();
                var output = await tasks.Last();
                return output.Last();
            });

            var results = await Task.WhenAll(tasks);
            Assert.AreEqual(57660948, results.Max());
        }

        class IntCode
        {
            const bool _debug = false;

            readonly int[] _ops;
            readonly BufferBlock<int> _inputs = new BufferBlock<int>();

            readonly List<int> _outputs = new List<int>();
            public IntCode PipeTo { get; set; }

            public IntCode(string program)
            {
                _ops = program.Split(",").Select(int.Parse).ToArray();
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
                    return $"[{_ops[position + 1]}:{_ops[_ops[position + 1]]}]";
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

            public void AddInput(int input)
            {
                _inputs.Post(input);
            }

            Task<int> GetInputAsync()
            {
                return _inputs.ReceiveAsync();
            }

            void Output(int value)
            {
                _outputs.Add(value);
                if (PipeTo != null)
                {
                    PipeTo.AddInput(value);
                }
            }

            void Log(string message)
            {
                if (_debug)
                {
                    Console.WriteLine(message);
                }
            }

            public async Task<IReadOnlyList<int>> RunAsync()
            {
                int ip = 0;
                bool done = false;

                while (!done)
                {
                    switch (GetOpCode(ip))
                    {
                        case 1: // Add
                            Log($"{ip}: [{_ops[ip + 3]}] = {DebugParam1(ip)} + {DebugParam2(ip)}");
                            _ops[_ops[ip + 3]] = GetParam1(ip) + GetParam2(ip);
                            ip += 4;
                            break;
                        case 2: // Mult
                            Log($"{ip}: [{_ops[ip + 3]}] = {DebugParam1(ip)} * {DebugParam2(ip)}");
                            _ops[_ops[ip + 3]] = GetParam1(ip) * GetParam2(ip);
                            ip += 4;
                            break;
                        case 3: // Input
                            var input = await GetInputAsync();
                            Log($"{ip}: [{_ops[ip + 1]}] = Input({input})");
                            _ops[_ops[ip + 1]] = input;
                            ip += 2;
                            break;
                        case 4: // Output
                            Log($"{ip}: Output {DebugParam1(ip)}");
                            Output(GetParam1(ip));
                            ip += 2;
                            break;
                        case 5: // Jump if true
                            Log($"{ip}: if {DebugParam1(ip)} != 0 jmp {DebugParam2(ip)}");
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
                            Log($"{ip}: if {DebugParam1(ip)} == 0 jmp {DebugParam2(ip)}");
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
                            Log($"{ip}: [{_ops[ip + 3]}] = ({DebugParam1(ip)} < {DebugParam2(ip)})");
                            _ops[_ops[ip + 3]] = (GetParam1(ip) < GetParam2(ip) ? 1 : 0);
                            ip += 4;
                            break;
                        case 8: // Equals
                            Log($"{ip}: [{_ops[ip + 3]}] = ({DebugParam1(ip)} == {DebugParam2(ip)})");
                            _ops[_ops[ip + 3]] = (GetParam1(ip) == GetParam2(ip) ? 1 : 0);
                            ip += 4;
                            break;
                        case 99: // End
                            Log($"{ip}: End");
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
}
