﻿using System;
using System.Linq;

namespace KeePass.Generator
{
    internal class DigitsSet : ICharacterSet
    {
        public char[] Characters
        {
            get
            {
                return Enumerable
                    .Range('0', 10)
                    .Select(x => (char)x)
                    .ToArray();
            }
        }

        public string Name
        {
            get { return Resources.Digits; }
        }

        public int Strength
        {
            get { return 10; }
        }
    }
}