//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2020 Aaron Bockover.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

using Mono.Options;

namespace Idgen
{
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
            switch (c)
            {
                case 'a':
                case 'b':
                case 'c':
                    return '2';
                case 'd':
                case 'e':
                case 'f':
                    return '3';
                case 'g':
                case 'h':
                case 'i':
                    return '4';
                case 'j':
                case 'k':
                case 'l':
                    return '5';
                case 'm':
                case 'n':
                case 'o':
                    return '6';
                case 'p':
                case 'q':
                case 'r':
                case 's':
                    return '7';
                case 't':
                case 'u':
                case 'v':
                    return '8';
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    return '9';
                default:
                    return char.IsDigit(c) ? c : '*';
            }
        }
    }
}
