using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Advent2019
{
    [TestFixture]
    public class Day20
    {
        [Test]
        public void Silver()
        {
            var moveCount = Run(0);
            Assert.AreEqual(570, moveCount);
        }

        [Test]
        public void Gold()
        {
            var moveCount = Run(1);
            Assert.AreEqual(7056, moveCount);
        }

        int Run(int levelDelta)
        { 
            var board = new char[150, 150];

            int ySize = -1;
            int xSize = -1;

            int yy = 0;
            foreach (var line in FileHelpers.ReadAllLines(@"Inputs\Day20.txt"))
            {
                int xx = 0;
                foreach (char c in line)
                {
                    if (c == '.' || c == '#' || c == 'o')
                    {
                        board[xx, yy] = c;
                    }
                    else
                    {
                        board[xx, yy] = '#';
                    }
                    if (++xx > xSize) { xSize = xx; }
                }
                if (++yy > ySize) { ySize = yy; }
            }

            var portals = new Dictionary<(int x, int y), (int x, int y)>
            {
                [(39, 0)] = (88, 83), // JG
                [(88, 83)] = (39, 0),
                [(47, 0)] = (88, 47), // LV
                [(88, 47)] = (47, 0),
                [(51, 0)] = (88, 37), // PM
                [(88, 37)] = (51, 0),
                [(63, 0)] = (30, 57), // PI
                [(30, 57)] = (63, 0),
                [(67, 0)] = (35, 90), // VO
                [(35, 90)] = (67, 0),
                [(69, 0)] = (61, 90), // TR
                [(61, 90)] = (69, 0),
                [(77, 0)] = (30, 53), // XC
                [(30, 53)] = (77, 0),
                [(37, 30)] = (63, 120), // FZ
                [(63, 120)] = (37, 30),
                [(47, 30)] = (37, 120), // OG
                [(37, 120)] = (47, 30),
                [(57, 30)] = (118, 55), // OD
                [(118, 55)] = (57, 30),
                [(59, 30)] = (59, 120), // XL
                [(59, 120)] = (59, 30),
                [(69, 30)] = (51, 120), // NB
                [(51, 120)] = (69, 30),
                [(71, 30)] = (0, 73), // UY
                [(0, 73)] = (71, 30),
                [(79, 30)] = (79, 120), // WW
                [(79, 120)] = (79, 30),
                [(0, 35)] = (88, 73), // PL
                [(88, 73)] = (0, 35),
                [(30, 37)] = (0, 69), // EL
                [(0, 69)] = (30, 37),
                [(118, 39)] = (73, 90), // BR
                [(73, 90)] = (118, 39),
                [(0, 41)] = (53, 90), // YP
                [(53, 90)] = (0, 41),
                [(30, 45)] = (118, 81), // TH
                [(118, 81)] = (30, 45),
                [(118, 47)] = (88, 59), // UN
                [(88, 59)] = (118, 47),
                [(0, 47)] = (30, 85), // EV
                [(30, 85)] = (0, 47),
                [(0, 55)] = (30, 69), // WV
                [(30, 69)] = (0, 55),
                [(30, 73)] = (118, 69), // XP
                [(118, 69)] = (30, 73),
                [(118, 61)] = (81, 90), // OA
                [(81, 90)] = (118, 61),
                [(67, 90)] = (43, 120), // CK
                [(43, 120)] = (67, 90),
                [(73, 120)] = (88, 67), // SL
                [(88, 67)] = (73, 120),
                [(0, 85)] = (41, 90), // JH
                [(41, 90)] = (0, 85)
            };

            foreach (var (k, v) in portals)
            {
                Assert.IsTrue(board[k.x, k.y] == 'o');
                Assert.IsTrue(board[v.x, v.y] == 'o');
                Assert.IsTrue(portals.ContainsKey(v));
                Assert.IsTrue(portals[v] == k);

                Assert.IsTrue(k.x == 0 || k.x == xSize - 1 || k.y == 0 || k.y == ySize - 1 ||
                    k.x == 30 || k.x == 88 || k.y == 30 || k.y == 90);
            }

            var nextMoves = new List<(int x, int y, int z)>
            {
                (0, 59, 0)
            };

            var target = (x: 0, y: 83, z: 0);

            int moveCount = 0;
            var visited = new HashSet<(int x, int y, int z)>();

            while (nextMoves.Any())
            {
                var thisMoves = nextMoves;
                nextMoves = new List<(int x, int y, int z)>();
                foreach (var move in thisMoves)
                {
                    visited.Add(move);

                    if (move == target)
                    {
                        return moveCount;
                    }

                    var (x, y, z) = move;

                    TryAddMove(x - 1, y, z);
                    TryAddMove(x + 1, y, z);
                    TryAddMove(x, y - 1, z);
                    TryAddMove(x, y + 1, z);

                    if (portals.TryGetValue((x, y), out var portalTarget))
                    {
                        int newZ = IsInner(x, y) ? z + levelDelta : z - levelDelta;
                        TryAddMove(portalTarget.x, portalTarget.y, newZ);
                    }
                }

                moveCount++;
            }

            return -1;

            void TryAddMove(int x, int y, int z)
            {
                if (x >= 0 && y >= 0 && x < xSize && y < ySize && z >= 0 && IsFloor(x, y) && !visited.Contains((x, y, z)))
                {
                    nextMoves.Add((x, y, z));
                }
            }

            bool IsFloor(int x, int y)
            {
                return board[x, y] == '.' || board[x, y] == 'o';
            }

            bool IsInner(int x, int y)
            {
                if (x == 0 || x == xSize - 1)
                {
                    return false;
                }
                if (y == 0 || y == ySize - 1)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
