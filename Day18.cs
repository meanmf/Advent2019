using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent2019
{
    [TestFixture]
    public class Day18
    {
        class Board
        {
            const int xSize = 81;
            const int ySize = 81;

            readonly char[,] _map = new char[xSize, ySize];
            readonly IReadOnlyDictionary<char, (int x, int y)> _keys;
            readonly IReadOnlyDictionary<char, IReadOnlyDictionary<char, (int distance, string doors)>> _distances;
            readonly Dictionary<string, int> _cache = new Dictionary<string, int>();
            readonly string _bots;

            public Board(string filename)
            {
                var bots = string.Empty;
                var keys = new Dictionary<char, (int x, int y)>();

                int y = 0;
                char nextBot = '1';
                foreach (var line in FileHelpers.ReadAllLines(filename))
                {
                    int x = 0;
                    foreach (var c in line)
                    {
                        _map[x, y] = c;

                        if (c == '@')
                        {
                            bots += nextBot;
                            keys[nextBot++] = (x, y);
                            _map[x, y] = '.';
                        }
                        else if (c != '.' && c != '#')
                        {
                            if (char.IsLower(c))
                            {
                                keys[char.ToUpper(c)] = (x, y);
                            }
                        }

                        x++;
                    }

                    y++;
                }

                _distances = keys.ToDictionary(k => k.Key, k => FindAllKeys(k.Value.x, k.Value.y, 0));
                _bots = bots;
                _keys = keys;
            }

            public int Run()
            {
                return RunGold(_bots, string.Empty, 0, int.MaxValue);
            }

            int RunGold(string currentPositions, string haveKeys, int distance, int minSolution)
            {
                int botCount = currentPositions.Length;

                foreach (var thisKey in currentPositions)
                {
                    foreach (var (nextKey, nextRoute) in _distances[thisKey])
                    {
                        if (haveKeys.Contains(nextKey)) continue;
                        if (distance + nextRoute.distance >= minSolution) continue;

                        if (nextRoute.doors.All(d => haveKeys.Contains(d)))
                        {
                            if (haveKeys.Length + 1 == _keys.Count - botCount)
                            {
                                minSolution = Math.Min(minSolution, distance + nextRoute.distance);
                            }
                            else
                            {
                                var newPositionArray = currentPositions.Replace(thisKey, nextKey).ToCharArray();
                                Array.Sort(newPositionArray);
                                var newPositions = new string(newPositionArray);

                                var newKeyArray = haveKeys.ToCharArray();
                                Array.Sort(newKeyArray);
                                var newKeys = new string(newKeyArray) + nextKey;

                                var hashKey = newPositions + "|" + newKeys;
                                if (!_cache.TryGetValue(hashKey, out var cacheDistance) ||
                                    cacheDistance > distance + nextRoute.distance)
                                {
                                    _cache[hashKey] = distance + nextRoute.distance;
                                    minSolution = RunGold(newPositions, newKeys, distance + nextRoute.distance, minSolution);
                                }
                            }
                        }
                    }
                }

                return minSolution;
            }

            bool IsDoor(char c)
            {
                return (c >= 'A' && c <= 'Z');
            }

            IReadOnlyDictionary<char, (int moves, string doors)> FindAllKeys(int botX, int botY, int moves)
            {
                var nextKeys = new Dictionary<char, (int moves, string doors)>();
                var visited = new bool[_map.GetLength(0), _map.GetLength(1)];

                var nextMoves = new List<(int x, int y, string doors)>
                {
                    (botX, botY, string.Empty)
                };

                while (nextMoves.Any())
                {
                    var thisMoves = nextMoves;
                    nextMoves = new List<(int x, int y, string doors)>();
                    moves++;

                    foreach (var (xx, yy, doors) in thisMoves)
                    {
                        visited[xx, yy] = true;
                        if (!visited[xx - 1, yy])
                        {
                            if (_map[xx - 1, yy] != '#')
                            {
                                if (IsDoor(_map[xx - 1, yy]))
                                {
                                    nextMoves.Add((xx - 1, yy, doors + _map[xx - 1, yy]));
                                }
                                else
                                {
                                    nextMoves.Add((xx - 1, yy, doors));
                                }
                            }
                            if (_map[xx - 1, yy] >= 'a' && _map[xx - 1, yy] <= 'z')
                            {
                                if (nextKeys.TryGetValue(char.ToUpper(_map[xx - 1, yy]), out var path))
                                {
                                    if (path.moves > moves)
                                    {
                                        nextKeys[char.ToUpper(_map[xx - 1, yy])] = (moves, doors);
                                    }
                                }
                                else
                                {
                                    nextKeys.Add(char.ToUpper(_map[xx - 1, yy]), (moves, doors));
                                }
                            }
                        }

                        if (!visited[xx + 1, yy])
                        {
                            if (_map[xx + 1, yy] != '#')
                            {
                                if (IsDoor(_map[xx + 1, yy]))
                                {
                                    nextMoves.Add((xx + 1, yy, doors + _map[xx + 1, yy]));
                                }
                                else
                                {
                                    nextMoves.Add((xx + 1, yy, doors));
                                }
                            }
                            if (_map[xx + 1, yy] >= 'a' && _map[xx + 1, yy] <= 'z')
                            {
                                if (nextKeys.TryGetValue(char.ToUpper(_map[xx + 1, yy]), out var path))
                                {
                                    if (path.moves > moves)
                                    {
                                        nextKeys[char.ToUpper(_map[xx + 1, yy])] = (moves, doors);
                                    }
                                }
                                else
                                {
                                    nextKeys.Add(char.ToUpper(_map[xx + 1, yy]), (moves, doors));
                                }
                            }
                        }

                        if (!visited[xx, yy - 1])
                        {
                            if (_map[xx, yy - 1] != '#')
                            {
                                if (IsDoor(_map[xx, yy - 1]))
                                {
                                    nextMoves.Add((xx, yy - 1, doors + _map[xx, yy - 1]));
                                }
                                else
                                {
                                    nextMoves.Add((xx, yy - 1, doors));
                                }
                            }
                            if (_map[xx, yy - 1] >= 'a' && _map[xx, yy - 1] <= 'z')
                            {
                                if (nextKeys.TryGetValue(char.ToUpper(_map[xx, yy - 1]), out var path))
                                {
                                    if (path.moves > moves)
                                    {
                                        nextKeys[char.ToUpper(_map[xx, yy - 1])] = (moves, doors);
                                    }
                                }
                                else
                                {
                                    nextKeys.Add(char.ToUpper(_map[xx, yy - 1]), (moves, doors));
                                }
                            }
                        }

                        if (!visited[xx, yy + 1])
                        {
                            if (_map[xx, yy + 1] != '#')
                            {
                                if (IsDoor(_map[xx, yy + 1]))
                                {
                                    nextMoves.Add((xx, yy + 1, doors + _map[xx, yy + 1]));
                                }
                                else
                                {
                                    nextMoves.Add((xx, yy + 1, doors));
                                }
                            }
                            if (_map[xx, yy + 1] >= 'a' && _map[xx, yy + 1] <= 'z')
                            {
                                if (nextKeys.TryGetValue(char.ToUpper(_map[xx, yy + 1]), out var path))
                                {
                                    if (path.moves > moves)
                                    {
                                        nextKeys[char.ToUpper(_map[xx, yy + 1])] = (moves, doors);
                                    }
                                }
                                else
                                {
                                    nextKeys.Add(char.ToUpper(_map[xx, yy + 1]), (moves, doors));
                                }
                            }
                        }
                    }
                }

                return nextKeys;
            }
        }

        [Test]
        public void Gold()
        {
            var board = new Board(@"Inputs\Day18b.txt");
            Assert.AreEqual(2138, board.Run());
        }

        [Test]
        public void Silver()
        {
            var board = new Board(@"Inputs\Day18.txt");
            Assert.AreEqual(5402, board.Run());
        }
    }
}
