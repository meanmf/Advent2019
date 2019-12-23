using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Advent2019
{
    [TestFixture]
    public class Day18
    {
        public static void Main()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            new Day18().Gold();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void Silver()
        {
            var maze = new Maze(FileHelpers.EnumerateLines(@"Inputs\Day18.txt"));
            Assert.AreEqual(5402, maze.Run());
        }

        [Test]
        public void Gold()
        {
            var maze = new Maze(FileHelpers.EnumerateLines(@"Inputs\Day18b.txt"));
            Assert.AreEqual(2138, maze.Run());
        }

        [Test]
        public void Part1Example1()
        {
            const string input = "########################\n#...............b.C.D.f#\n#.######################\n#.....@.a.B.c.d.A.e.F.g#\n########################\n";
            var maze = new Maze(FileHelpers.ReadAllLinesFromString(input));
            Assert.AreEqual(132, maze.Run());
        }

        [Test]
        public void Part1Example2()
        {
            const string input = "#################\n#i.G..c...e..H.p#\n########.########\n#j.A..b...f..D.o#\n########@########\n#k.E..a...g..B.n#\n########.########\n#l.F..d...h..C.m#\n#################\n";
            var maze = new Maze(FileHelpers.ReadAllLinesFromString(input));
            Assert.AreEqual(136, maze.Run());
        }

        [Test]
        public void Part1Example3()
        {
            const string input = "########################\n#@..............ac.GI.b#\n###d#e#f################\n###A#B#C################\n###g#h#i################\n########################\n";
            var maze = new Maze(FileHelpers.ReadAllLinesFromString(input));
            Assert.AreEqual(81, maze.Run());
        }

        [Test]
        public void Part2Example1()
        {
            const string input = "###############\n#d.ABC.#.....a#\n######@#@######\n###############\n######@#@######\n#b.....#.....c#\n###############\n";
            var maze = new Maze(FileHelpers.ReadAllLinesFromString(input));
            Assert.AreEqual(24, maze.Run());
        }

        [Test]
        public void Part2Example2()
        {
            const string input = "#############\n#DcBa.#.GhKl#\n#.###@#@#I###\n#e#d#####j#k#\n###C#@#@###J#\n#fEbA.#.FgHi#\n#############\n";
            var maze = new Maze(FileHelpers.ReadAllLinesFromString(input));
            Assert.AreEqual(32, maze.Run());
        }

        [Test]
        public void Part2Example3()
        {
            const string input = "#############\n#g#f.D#..h#l#\n#F###e#E###.#\n#dCba@#@BcIJ#\n#############\n#nK.L@#@G...#\n#M###N#H###.#\n#o#m..#i#jk.#\n#############\n";
            var maze = new Maze(FileHelpers.ReadAllLinesFromString(input));
            Assert.AreEqual(72, maze.Run());
        }

        class Maze
        {
            const int xSize = 81;
            const int ySize = 81;

            readonly char[,] _map = new char[xSize, ySize];
            readonly int _keyCount;
            readonly IReadOnlyDictionary<char, (char key, int distance, string requiredKeys)[]> _distances;
            readonly Dictionary<string, int> _cache = new Dictionary<string, int>();
            readonly string _bots;

            public Maze(IEnumerable<string> input)
            {
                var bots = string.Empty;
                var keys = new Dictionary<char, (int x, int y)>();

                char nextBot = '1';

                int y = 0;
                foreach (var line in input)
                {
                    int x = 0;
                    foreach (var c in line)
                    {
                        _map[x, y] = c;

                        if (IsBot(c))
                        {
                            bots += nextBot;
                            keys[nextBot++] = (x, y);
                            _map[x, y] = '.';
                        }
                        else if (IsKey(c))
                        {
                            keys[c] = (x, y);
                            _keyCount++;
                        }

                        x++;
                    }

                    y++;
                }
                _distances = keys.ToDictionary(k => k.Key, k => FindAllKeys(k.Value.x, k.Value.y, 0));
                _bots = bots;
            }

            public int Run()
            {
                _cache.Clear();
                return RunGold(_bots, string.Empty, 0, int.MaxValue);
            }

            int RunGold(string currentPositions, string haveKeys, int distance, int minSolution)
            {
                foreach (var thisKey in currentPositions)
                {
                    foreach (var (nextKey, nextDistance, requiredKeys) in _distances[thisKey])
                    {
                        if (haveKeys.Contains(nextKey)) continue;

                        int newDistance = distance + nextDistance;
                        if (newDistance >= minSolution) continue;

                        if (!HaveRequiredKeys(requiredKeys, haveKeys)) continue;

                        if (haveKeys.Length + 1 == _keyCount)
                        {
                            minSolution = Math.Min(minSolution, newDistance);
                        }
                        else
                        {
                            var newPositionArray = currentPositions.Replace(thisKey, nextKey).ToCharArray();
                            Array.Sort(newPositionArray);
                            var newPositions = new string(newPositionArray);

                            var newKeyArray = (haveKeys + nextKey).ToCharArray();
                            Array.Sort(newKeyArray);
                            var newKeys = new string(newKeyArray);

                            var hashKey = newPositions + newKeys;
                            if (!_cache.TryGetValue(hashKey, out var cacheDistance) ||
                                cacheDistance > newDistance)
                            {
                                _cache[hashKey] = newDistance;
                                minSolution = RunGold(newPositions, newKeys, newDistance, minSolution);
                            }
                        }
                    }
                }

                return minSolution;
            }

            (char key, int moves, string requiredKeys)[] FindAllKeys(int botX, int botY, int moves)
            {
                var nextKeys = new Dictionary<char, (int moves, string requiredKeys)>();
                var visited = new bool[_map.GetLength(0), _map.GetLength(1)];

                var nextMoves = new List<(int x, int y, string requiredKeys)>
                {
                    (botX, botY, string.Empty)
                };

                while (nextMoves.Any())
                {
                    var thisMoves = nextMoves;
                    nextMoves = new List<(int x, int y, string doors)>();
                    moves++;

                    foreach (var (x, y, requiredKeys) in thisMoves)
                    {
                        visited[x, y] = true;

                        CheckNext(x - 1, y, requiredKeys);
                        CheckNext(x + 1, y, requiredKeys);
                        CheckNext(x, y - 1, requiredKeys);
                        CheckNext(x, y + 1, requiredKeys);
                    }
                }

                return nextKeys.Select(k => (k.Key, k.Value.moves, k.Value.requiredKeys)).ToArray();

                void CheckNext(int x, int y, string doors)
                {
                    if (!visited[x, y])
                    {
                        if (IsDoor(_map[x, y]))
                        {
                            nextMoves.Add((x, y, doors + char.ToLower(_map[x, y])));
                        }
                        else if (!IsWall(_map[x, y]))
                        {
                            nextMoves.Add((x, y, doors));
                        }

                        if (IsKey(_map[x, y]))
                        {
                            if (!nextKeys.TryGetValue(_map[x, y], out var path) || path.moves > moves)
                            {
                                nextKeys[_map[x, y]] = (moves, doors);
                            }
                        }
                    }
                }
            }

            static bool HaveRequiredKeys(string requiredKeys, string haveKeys)
            {
                if (requiredKeys.Length > haveKeys.Length) return false;

                for (int i = 0; i < requiredKeys.Length; i++)
                {
                    if (!haveKeys.Contains(requiredKeys[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            static bool IsDoor(char c)
            {
                return (c >= 'A' && c <= 'Z');
            }

            static bool IsKey(char c)
            {
                return (c >= 'a' && c <= 'z');
            }

            static bool IsWall(char c)
            {
                return c == '#';
            }

            static bool IsBot(char c)
            {
                return c == '@';
            }
        }
    }
}
