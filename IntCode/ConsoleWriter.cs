using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Advent2019
{
    class ConsoleWriter : IObserver<long>
    {
        public TaskCompletionSource<bool> Completion = new TaskCompletionSource<bool>();

        public void OnCompleted()
        {
            Completion.SetResult(true);
        }

        public void OnError(Exception error)
        {
            Console.WriteLine(error);
        }

        public void OnNext(long value)
        {
            if (value <= 255)
            {
                Console.Write((char)value);
            }
            else
            {
                Console.Write(value);
            }
        }
    }

}
