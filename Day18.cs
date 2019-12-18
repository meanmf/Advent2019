using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent2019
{
    [TestFixture]
    public class Day18
    {
        public static void Main()
        {
            new Day18().Gold();
        }

        readonly IDictionary<char, (int x, int y)> _keys = new Dictionary<char, (int, int)>();

        int _minSolution = int.MaxValue;

        readonly IDictionary<char, IReadOnlyDictionary<char, (int distance, string doors)>> _distances =
            new Dictionary<char, IReadOnlyDictionary<char, (int distance, string doors)>>();

        readonly Dictionary<string, int> _cache = new Dictionary<string, int>();

        [Test]
        public void Gold()
        {
            const int xSize = 81;
            const int ySize = 81;
            var map = new char[xSize, ySize];

            int y = 0;
            foreach (var line in FileHelpers.ReadAllLines(@"Inputs\Day18b.txt"))
            {
                int x = 0;
                foreach (var c in line)
                {
                    map[x, y] = c;

                    if (c >= '1' && c <= '4')
                    {
                        _keys[c] = (x, y);
                        map[x, y] = '.';
                    }
                    else if (c != '.' && c != '#')
                    {
                        if (char.IsLower(c))
                        {
                            _keys[char.ToUpper(c)] = (x, y);
                        }
                    }

                    x++;
                }

                y++;
            }

            foreach (var key in _keys)
            {
                var toKeys = FindAllKeys(map, key.Value.x, key.Value.y, 0);
                _distances[key.Key] = toKeys;
            }

            RunGold("1234", "", 0);
            Console.WriteLine(_minSolution);
        }

        void RunGold(string currentPositions, string haveKeys, int distance)
        {
            if (distance > _minSolution) return;

            foreach (var thisKey in currentPositions)
            {
                foreach (var nextKey in _distances[thisKey])
                {
                    if (haveKeys.Contains(nextKey.Key)) continue;
                    if (distance + nextKey.Value.distance >= _minSolution) continue;

                    if (nextKey.Value.doors.All(d => haveKeys.Contains(d)))
                    {
                        if (haveKeys.Length == _keys.Count - 5)
                        {
                            _minSolution = Math.Min(_minSolution, distance + nextKey.Value.distance);
                            Console.WriteLine($"Solution={distance + nextKey.Value.distance}");
                        }
                        else
                        {
                            var newPositionArray = new char[4];
                            for (int i = 0; i < 4; i++)
                            {
                                if (currentPositions[i] == thisKey)
                                {
                                    newPositionArray[i] = nextKey.Key;
                                }
                                else
                                {
                                    newPositionArray[i] = currentPositions[i];
                                }
                            }

                            Array.Sort(newPositionArray);
                            var newPositions = new string(newPositionArray);

                            var newKeys = new string(haveKeys.OrderBy(c => c).Append(nextKey.Key).ToArray());
                            var hashKey = newPositions + "|" + newKeys;
                            if (_cache.TryGetValue(hashKey, out var cacheDistance))
                            {
                                if (cacheDistance <= distance + nextKey.Value.distance) continue;
                            }
                            _cache[hashKey] = distance + nextKey.Value.distance;

                            RunGold(newPositions, newKeys, distance + nextKey.Value.distance);
                        }
                    }
                }
            }
        }

        [Test]
        public void Silver()
        {
            const int xSize = 81;
            const int ySize = 81;
            var map = new char[xSize, ySize];

            int y = 0;
            foreach (var line in FileHelpers.ReadAllLines(@"Inputs\Day18.txt"))
            {
                int x = 0;
                foreach (var c in line)
                {
                    map[x, y] = c;

                    if (c == '@')
                    {
                        _keys['@'] = (x, y);
                        map[x, y] = '.';
                    }
                    else if (c != '.' && c != '#')
                    {
                        if (char.IsLower(c))
                        {
                            _keys[char.ToUpper(c)] = (x, y);
                        }
                    }

                    x++;
                }

                y++;
            }

            foreach (var key in _keys)
            {
                var toKeys = FindAllKeys(map, key.Value.x, key.Value.y, 0);
                _distances[key.Key] = toKeys;
            }

            RunSilver('@', string.Empty, 0);
            Console.WriteLine(_minSolution);
        }
    
        void RunSilver(char thisKey, string haveKeys, int distance)
        {
            if (distance > _minSolution) return;
            var newKeys = new string(haveKeys.OrderBy(c => c).Append(thisKey).ToArray());
            if (_cache.TryGetValue(newKeys, out var cacheDistance))
            {
                if (cacheDistance <= distance) return;
            }
            _cache[newKeys] = distance;

            foreach (var nextKey in _distances[thisKey])
            {
                if (newKeys.Contains(nextKey.Key)) continue;
                if (distance + nextKey.Value.distance >= _minSolution) continue;

                if (nextKey.Value.doors.All(d => newKeys.Contains(d)))
                {
                    if (newKeys.Length == _keys.Count - 1)
                    {
                        _minSolution = Math.Min(_minSolution, distance + nextKey.Value.distance);
                        Console.WriteLine($"Solution={distance + nextKey.Value.distance}");
                    }
                    else
                    {
                        RunSilver(nextKey.Key, newKeys, distance + nextKey.Value.distance);
                    }
                }
            }
        }

        bool IsDoor(char c)
        {
            return (c >= 'A' && c <= 'Z');
        }

        IReadOnlyDictionary<char, (int moves, string doors)> FindAllKeys(char[,] map, int botX, int botY, int moves)
        {
            var nextKeys = new Dictionary<char, (int moves, string doors)>();
            var visited = new int[map.GetLength(0), map.GetLength(1)];

            var nextMoves = new List<(int x, int y, string doors)>
            {
                (botX, botY, string.Empty)
            };

            while (nextMoves.Any())
            {
                var thisMoves = nextMoves;
                nextMoves = new List<(int x, int y, string doors)>();
                moves++;
                if (moves > _minSolution) break;

                foreach (var (xx, yy, doors) in thisMoves)
                {
                    visited[xx, yy] = moves;

                    if (visited[xx - 1, yy] == 0)
                    {
                        if (map[xx - 1, yy] != '#')
                        {
                            if (IsDoor(map[xx - 1, yy]))
                            {
                                nextMoves.Add((xx - 1, yy, doors + map[xx - 1, yy]));
                            }
                            else
                            {
                                nextMoves.Add((xx - 1, yy, doors));
                            }
                        }
                        if (map[xx - 1, yy] >= 'a' && map[xx - 1, yy] <= 'z')
                        {
                            if (nextKeys.TryGetValue(char.ToUpper(map[xx - 1, yy]), out var path))
                            {
                                if (path.moves > moves)
                                {
                                    nextKeys[char.ToUpper(map[xx - 1, yy])] = (moves, doors);
                                }
                            }
                            else
                            {
                                nextKeys.Add(char.ToUpper(map[xx - 1, yy]), (moves, doors));
                            }
                        }
                    }

                    if (visited[xx + 1, yy] == 0)
                    {
                        if (map[xx + 1, yy] != '#')
                        {
                            if (IsDoor(map[xx + 1, yy]))
                            {
                                nextMoves.Add((xx + 1, yy, doors + map[xx + 1, yy]));
                            }
                            else
                            {
                                nextMoves.Add((xx + 1, yy, doors));
                            }
                        }
                        if (map[xx + 1, yy] >= 'a' && map[xx + 1, yy] <= 'z')
                        {
                            if (nextKeys.TryGetValue(char.ToUpper(map[xx + 1, yy]), out var path))
                            {
                                if (path.moves > moves)
                                {
                                    nextKeys[char.ToUpper(map[xx + 1, yy])] = (moves, doors);
                                }
                            }
                            else
                            {
                                nextKeys.Add(char.ToUpper(map[xx + 1, yy]), (moves, doors));
                            }
                        }
                    }

                    if (visited[xx, yy - 1] == 0)
                    {
                        if (map[xx, yy - 1] != '#')
                        {
                            if (IsDoor(map[xx, yy - 1]))
                            {
                                nextMoves.Add((xx, yy - 1, doors + map[xx, yy - 1]));
                            }
                            else
                            {
                                nextMoves.Add((xx, yy - 1, doors));
                            }
                        }
                        if (map[xx, yy - 1] >= 'a' && map[xx, yy - 1] <= 'z')
                        {
                            if (nextKeys.TryGetValue(char.ToUpper(map[xx, yy - 1]), out var path))
                            {
                                if (path.moves > moves)
                                {
                                    nextKeys[char.ToUpper(map[xx, yy - 1])] = (moves, doors);
                                }
                            }
                            else
                            {
                                nextKeys.Add(char.ToUpper(map[xx, yy - 1]), (moves, doors));
                            }
                        }
                    }

                    if (visited[xx, yy + 1] == 0)
                    {
                        if (map[xx, yy + 1] != '#')
                        {
                            if (IsDoor(map[xx, yy + 1]))
                            {
                                nextMoves.Add((xx, yy + 1, doors + map[xx, yy + 1]));
                            }
                            else
                            {
                                nextMoves.Add((xx, yy + 1, doors));
                            }
                        }
                        if (map[xx, yy + 1] >= 'a' && map[xx, yy + 1] <= 'z')
                        {
                            if (nextKeys.TryGetValue(char.ToUpper(map[xx, yy + 1]), out var path))
                            {
                                if (path.moves > moves)
                                {
                                    nextKeys[char.ToUpper(map[xx, yy + 1])] = (moves, doors);
                                }
                            }
                            else
                            {
                                nextKeys.Add(char.ToUpper(map[xx, yy + 1]), (moves, doors));
                            }
                        }
                    }
                }
            }

            return nextKeys;
        }
    }
}
