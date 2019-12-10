// #define INTCODE_TRACE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Advent2019
{
    class IntCode
    {
        readonly BufferBlock<long> _inputs = new BufferBlock<long>();
        readonly List<long> _outputs = new List<long>();
        readonly IMemoryManager _mem;

        long _relativeBase = 0;
        long _ip = 0;

        public IntCode PipeTo { get; set; }

        public IntCode(string input, IMemoryManager memoryManager = null)
        {
            _mem = memoryManager ?? new FixedMemoryManager(1000);

            var program = input.Split(",").Select(long.Parse).ToArray();
            for (int i = 0; i < program.Length; i++)
            {
                _mem[i] = program[i];
            }
        }

        long GetOpCode()
        {
            return _mem[_ip] % 100;
        }

        long GetParam(int paramNum)
        {
            var modeBase = (int)(10 * Math.Pow(10, paramNum));
            var mode = (_mem[_ip] / modeBase) % 10;
            if (mode == 0)
            {
                return _mem[_ip + paramNum];
            }
            else if (mode == 1)
            {
                return _ip + paramNum;
            }

            return _relativeBase + _mem[_ip + paramNum];
        }

        string DebugParam(int paramNum, bool showCurrentValue = true)
        {
            var modeBase = (int)(10 * Math.Pow(10, paramNum));
            var mode = (_mem[_ip] / modeBase) % 10;

            if (showCurrentValue)
            {
                if (mode == 0)
                {
                    return $"{_mem[_mem[_ip + paramNum]]} @{_mem[_ip + paramNum]}";
                }
                else if (mode == 1)
                {
                    return _mem[_ip + paramNum].ToString();
                }

                return $"{_mem[_relativeBase + _mem[_ip + paramNum]]} @{_relativeBase + _mem[_ip + paramNum]}";
            }
            else
            {
                if (mode == 0)
                {
                    return $"@{_mem[_ip + paramNum]}";
                }
                else if (mode == 1)
                {
                    return "ERR";
                }

                return $"@{_relativeBase + _mem[_ip + paramNum]}";
            }
        }

        public void AddInput(long input)
        {
            _inputs.Post(input);
        }

        public void Set(long position, long value)
        {
            _mem[position] = value;
        }

        public long Get(long position)
        {
            return _mem[position];
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

#if (INTCODE_TRACE)
        void Log(string message)
        {
            Console.WriteLine($"{_ip}: {message}");
        }
#endif

        public async Task<IReadOnlyList<long>> RunAsync()
        {
            bool done = false;

            while (!done)
            {
                switch (GetOpCode())
                {
                    case 1: // Add
#if (INTCODE_TRACE)
                        Log($"{DebugParam(3, false)} = {DebugParam(1)} + {DebugParam(2)}");
#endif
                        _mem[GetParam(3)] = _mem[GetParam(1)] + _mem[GetParam(2)];
                        _ip += 4;
                        break;
                    case 2: // Mult
#if (INTCODE_TRACE)
                        Log($"{DebugParam(3, false)} = {DebugParam(1)} * {DebugParam(2)}");
#endif
                        _mem[GetParam(3)] = _mem[GetParam(1)] * _mem[GetParam(2)];
                        _ip += 4;
                        break;
                    case 3: // Input
                        var input = await GetInputAsync();
#if (INTCODE_TRACE)
                        Log($"{DebugParam(1, false)} = Input({input})");
#endif
                        _mem[GetParam(1)] = input;
                        _ip += 2;
                        break;
                    case 4: // Output
#if (INTCODE_TRACE)
                        Log($"Output {DebugParam(1)}");
#endif
                        Output(_mem[GetParam(1)]);
                        _ip += 2;
                        break;
                    case 5: // Jump if true
#if (INTCODE_TRACE)
                        Log($"if {DebugParam(1)} != 0 jmp {DebugParam(2)}");
#endif
                        if (_mem[GetParam(1)] != 0)
                        {
                            _ip = _mem[GetParam(2)];
                        }
                        else
                        {
                            _ip += 3;
                        }
                        break;
                    case 6: // Jump if false
#if (INTCODE_TRACE)
                        Log($"if {DebugParam(1)} == 0 jmp {DebugParam(2)}");
#endif
                        if (_mem[GetParam(1)] == 0)
                        {
                            _ip = _mem[GetParam(2)];
                        }
                        else
                        {
                            _ip += 3;
                        }
                        break;
                    case 7: // Less than
#if (INTCODE_TRACE)
                        Log($"{DebugParam(3, false)} = ({DebugParam(1)} < {DebugParam(2)})");
#endif
                        _mem[GetParam(3)] = (_mem[GetParam(1)] < _mem[GetParam(2)] ? 1 : 0);
                        _ip += 4;
                        break;
                    case 8: // Equals
#if (INTCODE_TRACE)
                        Log($"{DebugParam(3, false)} = ({DebugParam(1)} == {DebugParam(2)})");
#endif
                        _mem[GetParam(3)] = (_mem[GetParam(1)] == _mem[GetParam(2)] ? 1 : 0);
                        _ip += 4;
                        break;
                    case 9: // Relative Base
#if (INTCODE_TRACE)
                        Log($"RB += {DebugParam(1)} => {_relativeBase + _mem[GetParam(1)]}");
#endif
                        _relativeBase += _mem[GetParam(1)];
                        _ip += 2;
                        break;
                    case 99: // End
#if (INTCODE_TRACE)
                        Log($"End");
#endif
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
