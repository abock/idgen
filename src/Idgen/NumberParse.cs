﻿//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2020 Aaron Bockover.
// Licensed under the MIT License.

using System;
using System.Text.RegularExpressions;

namespace Idgen
{
    public static class NumberParse
    {
        static readonly Regex strip = new Regex(@"[\s_,]*");

        public static bool TryParse(string str, out int value)
        {
            value = int.MinValue;

            if (str is null)
                return false;
            
            str = str.Trim();

            var negate = false;
            if (str.Length > 0 && str[0] == '-')
            {
                negate = true;
                str = str.Substring(1);
            }

            str = strip.Replace(str, string.Empty);

            int @base = 10;

            if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                @base = 16;
                str = str.Substring(2);
            }
            else if (str.EndsWith("h", StringComparison.OrdinalIgnoreCase))
            {
                @base = 16;
                str = str.Substring(0, str.Length - 1);
            }
            else if (str.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
            {
                @base = 2;
                str = str.Substring(2);
            }

            try
            {
                value = Convert.ToInt32(str, @base);
                if (negate)
                    value = -value;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
