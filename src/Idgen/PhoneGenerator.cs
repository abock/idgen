//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2024 Aaron Bockover.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text;

using Mono.Options;

namespace Idgen;

public sealed class PhoneGenerator : IIdGenerator
{
    public string Command { get; } = "phone";

    public string CommandDescription { get; }
        = "Encodes a phrase suitable for input into a phone (0-9 * keys). See https://en.wikipedia.org/wiki/Telephone_keypad.";

    public string UsageArguments { get; } = "PHRASE";

    public OptionSet Options { get; }

    public IEnumerable<string> Generate(IEnumerable<string> args)
    {
        var builder = new StringBuilder();

        foreach (var arg in args)
        {
            foreach (var c in arg)
                builder.Append(Translate(c));

            builder.Append('#');

            yield return builder.ToString();
            builder.Clear();
        }
    }

    static char Translate(char c)
    {
        c = char.ToLowerInvariant(c);
        return c switch
        {
            'a' or 'b' or 'c' => '2',
            'd' or 'e' or 'f' => '3',
            'g' or 'h' or 'i' => '4',
            'j' or 'k' or 'l' => '5',
            'm' or 'n' or 'o' => '6',
            'p' or 'q' or 'r' or 's' => '7',
            't' or 'u' or 'v' => '8',
            'w' or 'x' or 'y' or 'z' => '9',
            _ => char.IsDigit(c) ? c : '*',
        };
    }
}
