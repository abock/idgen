//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2024 Aaron Bockover.
// Licensed under the MIT License.

using System;
using System.Text.RegularExpressions;

namespace Idgen;

static class NumberParse
{
    static readonly Regex s_strip = new(@"[\s_,]*");

    static (string str, int @base, bool negate) Configure(string str)
    {
        str = str.Trim();

        var negate = false;
        if (str.Length > 0 && str[0] == '-')
        {
            negate = true;
            str = str[1..];
        }

        str = s_strip.Replace(str, string.Empty);

        int @base = 10;

        if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            @base = 16;
            str = str[2..];
        }
        else if (str.EndsWith("h", StringComparison.OrdinalIgnoreCase))
        {
            @base = 16;
            str = str[..^1];
        }
        else if (str.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
        {
            @base = 2;
            str = str[2..];
        }

        return (str, @base, negate);
    }

    public static bool TryParse(string str, out int value)
    {
        value = int.MinValue;

        if (str is null)
            return false;

        var (numStr, @base, negate) = Configure(str);

        try
        {
            value = Convert.ToInt32(numStr, @base);
            if (negate)
                value = -value;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryParse64(string str, out long value)
    {
        value = long.MinValue;

        if (str is null)
            return false;

        var (numStr, @base, negate) = Configure(str);

        try
        {
            value = Convert.ToInt64(numStr, @base);
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
