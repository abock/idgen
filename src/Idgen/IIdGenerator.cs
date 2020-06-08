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
    public interface IIdGenerator
    {
        string Command { get; }
        string CommandDescription { get; }
        string UsageArguments { get; }
        OptionSet Options { get; }
        IEnumerable<string> Generate(IEnumerable<string> args);
    }
}
