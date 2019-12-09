using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Advent2019
{
    class IntCode
    {
        readonly bool debugOutput = false;
        readonly long[] _ops = new long[10000];
        readonly BufferBlock<long> _inputs = new BufferBlock<long>();
        readonly List<long> _outputs = new List<long>();

        long _relativeBase = 0;
        long _ip = 0;

        public IntCode PipeTo { get; set; }

        public IntCode(string input, bool debug = false)
        {
            debugOutput = debug;

            var program = input.Split(",").Select(long.Parse).ToArray();
            Array.Copy(program, 0, _ops, 0, program.Length);
        }

        long GetOpCode()
        {
            return _ops[_ip] % 100;
        }

        long GetParam(int paramNum)
        {
            var modeBase = (int)(10 * Math.Pow(10, paramNum));
            var mode = (_ops[_ip] / modeBase) % 10;
            if (mode == 0)
            {
                return _ops[_ip + paramNum];
            }
            else if (mode == 1)
            {
                return _ip + paramNum;
            }

            return _relativeBase + _ops[_ip + paramNum];
        }

        string DebugParam(int paramNum)
        {
            var modeBase = (int)(10 * Math.Pow(10, paramNum));
            var mode = (_ops[_ip] / modeBase) % 10;
            if (mode == 0)
            {
                return $"[{_ops[_ip + paramNum]}:{_ops[_ops[_ip + paramNum]]}]";
            }
            else if (mode == 1)
            {
                return _ops[_ip + paramNum].ToString();
            }

            return $"R[{_relativeBase} + {_ops[_ip + paramNum]}:{_ops[_relativeBase + _ops[_ip + paramNum]]}]";
        }

        public void AddInput(long input)
        {
            _inputs.Post(input);
        }

        public void Set(long position, long value)
        {
            _ops[position] = value;
        }

        public long Get(long position)
        {
            return _ops[position];
        }

        Task<long> GetInputAsync()
        {
            return _inputs.ReceiveAsync();
        }

        void Output(long value)
        {
            _outputs.Add(value);
            if (PipeTo != null)
            {
                PipeTo.AddInput(value);
            }
        }

        void Log(string message)
        {
            if (debugOutput)
            {
                Console.WriteLine(message);
            }
        }

        public async Task<IReadOnlyList<long>> RunAsync()
        {
            bool done = false;

            while (!done)
            {
                switch (GetOpCode())
                {
                    case 1: // Add
                        Log($"{_ip}: {DebugParam(3)} = {DebugParam(1)} + {DebugParam(2)}");
                        _ops[GetParam(3)] = _ops[GetParam(1)] + _ops[GetParam(2)];
                        _ip += 4;
                        break;
                    case 2: // Mult
                        Log($"{_ip}: {DebugParam(3)} = {DebugParam(1)} * {DebugParam(2)}");
                        _ops[GetParam(3)] = _ops[GetParam(1)] * _ops[GetParam(2)];
                        _ip += 4;
                        break;
                    case 3: // Input
                        var input = await GetInputAsync();
                        Log($"{_ip}: {DebugParam(1)} = Input({input})");
                        _ops[GetParam(1)] = input;
                        _ip += 2;
                        break;
                    case 4: // Output
                        Log($"{_ip}: Output {DebugParam(1)}");
                        Output(_ops[GetParam(1)]);
                        _ip += 2;
                        break;
                    case 5: // Jump if true
                        Log($"{_ip}: if {DebugParam(1)} != 0 jmp {DebugParam(2)}");
                        if (_ops[GetParam(1)] != 0)
                        {
                            _ip = _ops[GetParam(2)];
                        }
                        else
                        {
                            _ip += 3;
                        }
                        break;
                    case 6: // Jump if false
                        Log($"{_ip}: if {DebugParam(1)} == 0 jmp {DebugParam(2)}");
                        if (_ops[GetParam(1)] == 0)
                        {
                            _ip = _ops[GetParam(2)];
                        }
                        else
                        {
                            _ip += 3;
                        }
                        break;
                    case 7: // Less than
                        Log($"{_ip}: {DebugParam(3)} = ({DebugParam(1)} < {DebugParam(2)})");
                        _ops[GetParam(3)] = (_ops[GetParam(1)] < _ops[GetParam(2)] ? 1 : 0);
                        _ip += 4;
                        break;
                    case 8: // Equals
                        Log($"{_ip}: {DebugParam(3)} = ({DebugParam(1)} == {DebugParam(2)})");
                        _ops[GetParam(3)] = (_ops[GetParam(1)] == _ops[GetParam(2)] ? 1 : 0);
                        _ip += 4;
                        break;
                    case 9: // Relative Base
                        var param1 = DebugParam(1);
                        _relativeBase += _ops[GetParam(1)];
                        Log($"{_ip}: RB += {param1} => {_relativeBase}");
                        _ip += 2;
                        break;
                    case 99: // End
                        Log($"{_ip}: End");
                        done = true;
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid opcode @ {_ip}: {GetOpCode()}");
                }
            }

            return _outputs;
        }
    }
}
