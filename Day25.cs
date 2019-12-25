using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Advent2019
{
    public class Day25
    {
        static readonly string _input = FileHelpers.GetSingle(@"Inputs\Day25.txt");

        public static async Task Main()
        {
            var mem = new FixedMemoryManager(8192);
            var intcode = new IntCode(_input, mem);

            var obs = new ConsoleWriter();
            intcode.OutputBlock.AsObservable().Subscribe(obs);
            var intCodeTask = intcode.RunAsync();

            for (; ;)
            {
                var cmd = Console.ReadLine();
                if (cmd == "quit") break;
                intcode.Write(cmd + '\n');
            }

            intcode.Terminate();
            await intCodeTask;
        }
    }
}
