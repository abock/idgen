//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2020 Aaron Bockover.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using Mono.Options;

namespace Idgen
{
    public sealed class NanoidGenerator : IIdGenerator
    {
        int size = 21;
        string alphabet = "_~0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public string Command { get; } = "nanoid";
        public string CommandDescription { get; }
            = "Generate a Nano ID. See https://github.com/ai/nanoid";
        public string UsageArguments { get; }
        public OptionSet Options { get; }

        public NanoidGenerator()
        {
            Options = new OptionSet
            {
                {
                    "s|size=",
                    "The ID will be {SIZE} characters in length (21 if unspecified).",
                    v => {
                        if (v != null && (!NumberParse.TryParse (v, out size) || size <= 0))
                            throw new Exception (
                                "SIZE must be a positive integer.");
                    }
                },
                {
                    "a|alphabet=",
                    $"Set the alphabet for -nanoid (default is {alphabet})",
                    v => alphabet = v
                }
            };
        }

        public IEnumerable<string> Generate(IEnumerable<string> args)
        {
            yield return Nanoid.Nanoid.Generate(alphabet, size);
        }
    }
}
