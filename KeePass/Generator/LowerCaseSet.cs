﻿using System;
using System.Linq;

namespace KeePass.Generator
{
    internal class LowerCaseSet : ICharacterSet
    {
        public char[] Characters
        {
            get
            {
                return Enumerable
                    .Range('a', 'z' - 'a')
                    .Select(x => (char)x)
                    .ToArray();
            }
        }

        public string Name
        {
            get { return Resources.Lower; }
        }

        public int Strength
        {
            get { return 26; }
        }
    }
}