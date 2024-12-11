//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2020 Aaron Bockover.
// Licensed under the MIT License.

using System.Collections.Generic;

using Mono.Options;

namespace Idgen
{
    public sealed class XcodeIdGenerator : IIdGenerator
    {
        public string Command { get; } = "xcode";
        public string CommandDescription { get; }
            = "Generate an identifier for use in Xcode storyboards/XIBs.";
        public string UsageArguments { get; }
        public OptionSet Options { get; }

        public IEnumerable<string> Generate(IEnumerable<string> args)
        {
            yield return NanoidDotNet
                .Nanoid
                .Generate(
                    alphabet: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
                    size: 8)
                .Insert(3, "-")
                .Insert(6, "-");
        }
    }
}
