//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2024 Aaron Bockover.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Mono.Options;

using HashidsNet;

namespace Idgen;

public sealed class HashidsGenerator : IIdGenerator
{
    int _minHashLength = 0;
    string _alphabet = Hashids.DEFAULT_ALPHABET;
    string _seps = Hashids.DEFAULT_SEPS;
    string _salt = "";

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
                v => _salt = v
            },
            {
                "s|size=",
                "The ID should be at least {SIZE} characters in length.",
                v =>
                {
                    if (v != null && (!NumberParse.TryParse(v, out _minHashLength) || _minHashLength <= 0))
                        throw new Exception (
                            "SIZE must be a positive integer.");
                }
            },
            {
                "a|alphabet=",
                $"Set the alphabet for the hashid (default is {_alphabet})",
                v => _alphabet = v
            },
            {
                "seps=",
                $"Set the separator characters for the hashid -(default is {_seps})",
                v => _seps = v
            }
        };
    }

    static readonly Regex s_spaceRegex = new(@"\s+");

    public IEnumerable<string> Generate(IEnumerable<string> args)
    {
        var numberStrings = args.ToArray();
        if (numberStrings.Length > 0)
        {
            yield return GenerateSingle(numberStrings);
            yield break;
        }

        string line;
        while ((line = Console.In.ReadLine()) != null)
        {
            yield return GenerateSingle(s_spaceRegex
                .Split(line)
                .Where(a => !string.IsNullOrEmpty(a))
                .ToArray());
        }
    }

    string GenerateSingle(string[] numberStrings)
        => new Hashids(_salt, _minHashLength, _alphabet, _seps)
            .EncodeLong(numberStrings.Select(a =>
            {
                if (!NumberParse.TryParse64(a, out var part) || part < 0)
                    throw new Exception($"NUMBER '{a}' must be a positive integer or zero.");
                return part;
            }));
}
