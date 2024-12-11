//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2024 Aaron Bockover.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Mono.Options;

namespace Idgen;

public static class Program
{
    static readonly string s_version = typeof(Program)
        .Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        .InformationalVersion;

    static readonly string s_copyright = typeof(Program)
        .Assembly
        .GetCustomAttribute<AssemblyCopyrightAttribute>()
        .Copyright;

    public static int Main(string[] args)
    {
        bool showVersion = false;
        int numberOfIds = 1;

        var firstArg = args.FirstOrDefault();
        if (string.IsNullOrEmpty(firstArg) || firstArg[0] == '-' || firstArg[0] == '/')
        {
            switch (firstArg?[1..].ToLowerInvariant())
            {
                case "h":
                case "?":
                case "help":
                case "-help":
                    break;
                default:
                    args = ["v4", .. args];
                    break;
            }
        }

        var suite = new CommandSet("idgen")
        {
            { "Usage: idgen COMMAND [OPTIONS]+" },
            { "" },
            { $"  idgen v{s_version}" },
            { $"  https://github.com/abock/idgen"},
            { $"  {s_copyright}"},
            { "" },
            { "OPTIONS:" },
            { "" },
            {
                "h|?|help",
                "Show this help.",
                v => { }
            },
            {
                "V|version",
                "Show the idgen version.",
                v => showVersion = true
            },
            {
                "n=",
                "Generate {NUMBER} of identifiers", v =>
                {
                    if (!NumberParse.TryParse(v, out numberOfIds) || numberOfIds < 0)
                        throw new Exception(
                            "NUMBER must be a positive integer, or zero, for the -number option.");
                }
            },
            { "" },
            { "COMMANDS:" },
            { "" }
        };

        var generators = new IIdGenerator[]
        {
            new GuidGenerator.V4(),
            new GuidGenerator.V5(),
            new GuidGenerator.V3(),
            new NanoidGenerator(),
            new HashidsGenerator(),
            new XcodeIdGenerator(),
            new PhoneGenerator()
        };

        foreach (var generator in generators)
        {
            var hasOptions = generator.Options?.Any(o => !string.IsNullOrEmpty(o.Prototype)) ?? false;

            var usageLine = hasOptions ? "[OPTIONS]+" : null;

            if (!string.IsNullOrEmpty(generator.UsageArguments))
            {
                if (usageLine != null)
                    usageLine += " ";
                usageLine += generator.UsageArguments;
            }

            if (usageLine != null)
                usageLine = " " + usageLine;

            var optionSet = new OptionSet
            {
                { $"Usage: {suite.Suite} {generator.Command}{usageLine}" },
            };

            if (hasOptions)
            {
                optionSet.Add("");
                optionSet.Add("OPTIONS:");
                optionSet.Add("");

                foreach (Option option in generator.Options)
                    optionSet.Add(option);
            }

            suite.Add(new Command(generator.Command, generator.CommandDescription)
            {
                Options = optionSet,
                Run = commandArgs => RunCommand(generator, commandArgs)
            });
        }

        void RunCommand(IIdGenerator generator, IEnumerable<string> commandArgs)
        {
            if (showVersion)
            {
                Console.WriteLine(s_version);
                return;
            }

            for (int i = 0; i < numberOfIds; i++)
            {
                foreach (var id in generator.Generate(commandArgs))
                {
                    if (id != null)
                        Console.WriteLine(id);
                }
            }
        }

        suite.Add(
            "\n" +
            "NOTE: any argument that expects a number my be specified in decimal, " +
            "binary (0b1001), or hex (0xabcd and ab123h) notation. Numbers may " +
            "also contain digit separators (_ and ,) and arbitrary whitespace.");

        try
        {
            suite.Run(args);
        }
        catch (Exception e)
        {
            Error(e.Message);
            return 2;
        }

        return 0;
    }

    static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"Error: {message}");
        Console.ResetColor();
    }
}
