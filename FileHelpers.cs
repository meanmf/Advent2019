using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Advent2019
{
    static class FileHelpers
    {
        public static IEnumerable<string> ReadAllLines(string filename)
        {
            using var inputFile = new StreamReader(File.OpenRead(filename));
            while (!inputFile.EndOfStream)
            {
                yield return inputFile.ReadLine();
            }
        }

        public static string GetSingle(string filename)
        {
            using var inputFile = new StreamReader(File.OpenRead(filename));
            return inputFile.ReadLine();
        }

        public static IEnumerable<string> ReadAllLinesFromString(string input)
        {
            using var inputStream = new StringReader(input);

            for (; ; )
            {
                var line = inputStream.ReadLine();
                if (line == null) yield break;

                yield return line;
            }
        }
    }
}
