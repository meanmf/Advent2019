using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advent2019
{
    class TreeNode
    {
        public string Name { get; }
        public TreeNode Parent;

        public TreeNode(string name)
        {
            Name = name;
        }

        public IEnumerable<TreeNode> GetPathToRoot()
        {
            var node = Parent;
            while (node != null)
            {
                yield return node;
                node = node.Parent;
            }
        }
    }

    [TestFixture]
    public class Day6
    {
        [Test]
        public void Silver()
        {
            var treeNodes = ReadTree();

            var sum = treeNodes.Values.Sum(node => node.GetPathToRoot().Count());
            Assert.AreEqual(140608, sum);
        }

        [Test]
        public void Gold()
        {
            var treeNodes = ReadTree();

            var youParents = treeNodes["YOU"].GetPathToRoot().Reverse().ToArray();
            var santaParents = treeNodes["SAN"].GetPathToRoot().Reverse().ToArray();

            int i = 0;
            while (youParents[i] == santaParents[i])
            {
                i++;
            }

            var distance = youParents.Length - i + santaParents.Length - i;
            Assert.AreEqual(337, distance);
        }

        IReadOnlyDictionary<string, TreeNode> ReadTree()
        {
            var treeNodes = new ConcurrentDictionary<string, TreeNode>();

            Parallel.ForEach(FileHelpers.ReadAllLines("Inputs\\Day6.txt"), line =>
            {
                var tokens = line.Split(")");

                var parentNode = treeNodes.GetOrAdd(tokens[0], v => new TreeNode(v));
                var childNode = treeNodes.GetOrAdd(tokens[1], v => new TreeNode(v));

                childNode.Parent = parentNode;
            });

            return treeNodes;
        }
    }
}
