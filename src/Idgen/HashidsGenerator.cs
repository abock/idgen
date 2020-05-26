//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2020 Aaron Bockover.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Options;

using HashidsNet;

namespace Idgen
{
    sealed class HashidsGenerator : IIdGenerator
    {
        int minHashLength = 0;
        string alphabet = Hashids.DEFAULT_ALPHABET;
        string seps = Hashids.DEFAULT_SEPS;
        string salt = "";

        public string Command { get; } = "hashid";
        public string CommandDescription { get; }
            = "Generate a Hashid from a set of positive integers. See https://hashids.org.";
        public string UsageArguments { get; } = "NUMBERS+";
        public OptionSet Options { get; }

        public HashidsGenerator()
        {
            Options = new OptionSet
            {
                {
                    "salt=",
                    $"Set the salt for the hashid.",
                    v => salt = v
                },
                {
                    "s|size=",
                    "The ID should be at least {SIZE} characters in length.",
                    v =>
                    {
                        if (v != null && (!NumberParse.TryParse (v, out minHashLength) || minHashLength <= 0))
                            throw new Exception (
                                "SIZE must be a positive integer.");
                    }
                },
                {
                    "a|alphabet=",
                    $"Set the alphabet for the hashid (default is {alphabet})",
                    v => alphabet = v
                },
                {
                    "seps=",
                    $"Set the separator characters for the hashid -(default is {seps})",
                    v => seps = v
                }
            };
        }

        public string Generate(IEnumerable<string> args)
        {
            var numberStrings = args.ToArray();
            if (numberStrings == null || numberStrings.Length == 0)
                throw new Exception("At least one NUMBER is required to generate a hashid.");

            return new Hashids(salt, minHashLength, alphabet, seps)
                .Encode(numberStrings.Select(a =>
                {
                    if (!NumberParse.TryParse(a, out var part) || part < 0)
                        throw new Exception($"NUMBER '{a}'must be a positive integer or zero.");
                    return (int)part;
                }));
        }
    }
}
