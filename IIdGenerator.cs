//
// Author:
//   Aaron Bockover <aaron.bockover@gmail.com>
//
// Copyright 2018 Aaron Bockover.
// Licensed under the MIT License.

using System.Collections.Generic;

using Mono.Options;

namespace Idgen
{
    interface IIdGenerator
    {
        string Command { get; }
        string CommandDescription { get; }
        string UsageArguments { get; }
        OptionSet Options { get; }
        string Generate (IEnumerable<string> args);
    }
}