using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Advent2019
{
    [TestFixture]
    public class Day23
    {
        const int _botCount = 50;

        readonly string _input = FileHelpers.GetSingle(@"Inputs\Day23.txt");

        [Test]
        public async Task Silver()
        {
            var nat = new SilverNAT();
            var bots = MakeBots(nat);
            var natTask = nat.RunAsync(bots);
            var tasks = bots.Select(c => c.RunAsync()).ToArray();

            await Task.WhenAll(tasks);

            var answer = await natTask;
            Assert.AreEqual(22134, answer);
        }

        [Test]
        public async Task Gold()
        {
            var nat = new GoldNAT();
            var bots = MakeBots(nat);
            var natTask = nat.RunAsync(bots);
            var tasks = bots.Select(c => c.RunAsync()).ToArray();
            
            await Task.WhenAll(tasks);

            var answer = await natTask;
            Assert.AreEqual(16084, answer);
        }

        IntCode[] MakeBots(INat nat)
        {
            var bots = new IntCode[_botCount];
            for (int i = 0; i < bots.Length; i++)
            {
                var mem = new FixedMemoryManager(4096);
                var intcode = new IntCode(_input, mem, i, nonBlocking: true);
                intcode.InputBlock.Post(i);
                bots[i] = intcode;
                var observer = new Observer(i, bots, nat);
                intcode.OutputBlock.AsObservable().Subscribe(observer);
            }

            return bots;
        }

        class Observer : IObserver<long>
        {
            readonly INat _nat;
            readonly int _id;
            readonly IntCode[] _bots;
            readonly Queue<long> _queue = new Queue<long>();

            public Observer(int id, IntCode[] others, INat nat)
            {
                _id = id;
                _bots = others;
                _nat = nat;
            }

            public void OnNext(long value)
            {
                _queue.Enqueue(value);
                if (_queue.Count == 3)
                {
                    var target = _queue.Dequeue();
                    var x = _queue.Dequeue();
                    var y = _queue.Dequeue();

                    if (target == 255)
                    {
                        _nat.Set(x, y);
                        Console.WriteLine($"{_id}: NAT->{x},{y}");
                    }
                    else
                    {
                        var targetBot = _bots[target];
                        targetBot.InputBlock.Post(x);
                        targetBot.InputBlock.Post(y);
                        Console.WriteLine($"{_id}: @{target} {x},{y}");
                    }
                }
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }
        }

        interface INat
        {
            void Set(long x, long y);
        }

        class SilverNAT : INat
        {
            readonly TaskCompletionSource<long>  _answerWaiter = new TaskCompletionSource<long>();

            public async Task<long> RunAsync(IntCode[] bots)
            {
                var answer = await _answerWaiter.Task;
                foreach (var bot in bots)
                {
                    bot.Terminate();
                }
                return answer;
            }

            public void Set(long x, long y)
            {
                _answerWaiter.SetResult(y);
            }
        }

        class GoldNAT : INat
        {
            long _x;
            long _y;
            long _lastY = int.MinValue;

            public void Set(long x, long y)
            {
                _x = x;
                _y = y;
            }

            public async Task<long> RunAsync(IntCode[] bots)
            {
                await Task.Delay(500);
                for (; ;)
                {
                    if (bots.All(c => c.IsPolling))
                    {
                        if (_y == _lastY)
                        {
                            foreach (var bot in bots)
                            {
                                bot.Terminate();
                            }
                            return _y;
                        }
                        _lastY = _y;
                        bots[0].InputBlock.Post(_x);
                        bots[0].InputBlock.Post(_y);
                        await Task.Delay(500);
                    }
                    else
                    {
                        await Task.Delay(10);
                    }
                }
            }
        }
    }
}
