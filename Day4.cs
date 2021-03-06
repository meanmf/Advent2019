﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Advent2019
{
    [TestFixture]
    public class Day4
    {
        readonly IEnumerable<int> _range;

        public Day4()
        {
            var input = FileHelpers.GetSingle("Inputs\\Day4.txt");
            var tokens = input.Split(",");
            var low = int.Parse(tokens[0]);
            var high = int.Parse(tokens[1]);
            _range = Enumerable.Range(low, high - low + 1);
        }
        
        [Test]
        public void Silver()
        {
            Assert.AreEqual(966, _range.Count(IsMatchSilver));
        }

        [Test]
        public void Gold()
        {
            Assert.AreEqual(628, _range.Count(IsMatchGold));
        }

        static bool IsMatchSilver(int number)
        {
            var str = number.ToString();

            bool isMatch = false;
            for (int i = 1; i < str.Length; i++)
            {
                if (str[i] == str[i-1])
                {
                    isMatch = true;
                }

                if (str[i] < str[i-1])
                {
                    return false;
                }
            }

            return isMatch;
        }

        static bool IsMatchGold(int number)
        {
            var str = number.ToString();

            int len = 1;
            bool isMatch = false;
            for (int i = 1; i < str.Length; i++)
            {
                if (str[i] != str[i-1])
                {
                    if (len == 2)
                    {
                        isMatch = true;
                    }

                    len = 1;
                }
                else
                {
                    len++;
                }

                if (str[i] < str[i-1])
                {
                    return false;
                }
            }

            if (len == 2)
            {
                isMatch = true;
            }

            return isMatch;
        }
    }
}
