//
// Author:
//   Aaron Bockover <aaron.bockover@gmail.com>
//
// Copyright 2018 Aaron Bockover.
// Licensed under the MIT License.

using System;
using System.Reflection;

using Mono.Options;

using static Xamarin.GuidHelpers;

namespace Idgen
{
    static class Program
    {
        static readonly string version = typeof (Program)
            .Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute> ()
            .InformationalVersion;

        static readonly string copyright = typeof (Program)
            .Assembly
            .GetCustomAttribute<AssemblyCopyrightAttribute> ()
            .Copyright;

        static readonly Random random = new Random ();

        enum IdKind
        {
            GuidV4,
            GuidV5,
            GuidV3,
            Xcode,
            Nanoid
        }

        enum GuidFormat
        {
            D,
            N,
            B,
            P,
            X,
            Base64
        }

        [Flags]
        enum Filter
        {
            None = 0,
            Uppercase = 1 << 0
        }

        static int Main (string[] args)
        {
            bool showHelp = false;
            bool showVersion = false;
            uint numberOfIds = 1;
            int nanoidSize = 21;
            string nanoidAlphabet = "_~0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            IdKind kind = default;
            GuidFormat guidFormat = default;
            Filter filter = default;
            Guid guidNamespace = GuidNamespace.URL;
            string guidName = null;

            void ToggleFilter (string v, Filter f)
            {
                if (v == null)
                    filter &= ~f;
                else
                    filter |= f;
            }

            var options = new OptionSet {
                { "Usage: idgen [OPTIONS]+" },
                { "" },
                { $"  idgen v{version}" },
                { $"  https://github.com/abock/idgen"},
                { $"  {copyright}"},
                { "" },
                { "OPTIONS:" },
                { "" },
                {
                    "h|?|help",
                    "Show this help.",
                    v => showHelp = true
                },
                {
                    "V|version",
                    "Show the idgen version.",
                    v => showVersion = true
                },
                {
                    "n=",
                    "Generate {NUMBER} of identifiers", v => {
                        if (!uint.TryParse (v, out numberOfIds))
                            throw new Exception (
                                "NUMBER must be a positive integer, or zero, for the -number option.");
                    }
                },
                {
                    "u|upper",
                    "Output the identifier in upper-case",
                    v => ToggleFilter (v, Filter.Uppercase)
                },
                {
                    "f|format=",
                    "{FORMAT} to use (see FORMATS) for GUID identifiers.",
                    v => {
                        if (!Enum.TryParse<GuidFormat> (v, true, out guidFormat))
                            throw new Exception (
                                "Invalid FORMAT for the -format option.");
                    }
                },
                {
                    "guidv4",
                    "Generate a random version 4 GUID (default ID type if no other type options are specified).",
                    v => kind = IdKind.GuidV4
                },
                {
                    "guidv5=",
                    "Generate a version 5 GUID based on a SHA-1 hash of {VALUE} and a namespace provided " +
                    "by the -namespace option. The URL namespace will be used if one is not specified.",
                    v => {
                        kind = IdKind.GuidV5;
                        guidName = v;
                    }
                },
                {
                    "guidv3=",
                    "Generate a version 3 GUID based on a MD5 hash of {VALUE} and a namespace provided " +
                    "by the -namespace option. The URL namespace will be used if one is not specified.",
                    v => {
                        kind = IdKind.GuidV3;
                        guidName = v;
                    }
                },
                {
                    "namespace=",
                    "For -guidv5 and -guidv3 options, specify the {NAMESPACE_GUID} that should be used. " +
                    "The namespace must itself be a GUID.",
                    v => {
                        if (!Guid.TryParse (v, out guidNamespace))
                            throw new Exception ("NAMESPACE_GUID is not a valid GUID.");
                    }
                },
                {
                    "nanoid:",
                    "Generate a Nano ID of {SIZE} characters (default: 21). See https://zelark.github.io/nano-id-cc/.",
                    v => {
                        if (v != null && (!int.TryParse (v, out nanoidSize) || nanoidSize <= 0))
                            throw new Exception (
                                "SIZE must be a positive integer.");

                        kind = IdKind.Nanoid;
                    }
                },
                {
                    "nanoid-alphabet=",
                    $"Set the alphabet for -nanoid (default is {nanoidAlphabet})",
                    v => nanoidAlphabet = v
                },
                {
                    "xcode",
                    "Generate an identifier for use in Xcode storyboards/XIBs.",
                    v => kind = IdKind.Xcode
                },
                { "" },
                { "FORMATS:" },
                { "" },
                { "  Base64   The binary representation of the GUID encoded in base 64." } ,
                { "           This format ignores the -uppercase option if specified." },
                { "" },
                { "  N        32 digits:" },
                { "           00000000000000000000000000000000" },
                { "" },
                { "  D        32 digits separated by hyphens:" },
                { "           00000000-0000-0000-0000-000000000000" },
                { "" },
                { "  B        32 digits separated by hyphens, enclosed in braces:" },
                { "           {{00000000-0000-0000-0000-000000000000}}" },
                { "" },
                { "  P        32 digits separated by hyphens, enclosed in parentheses:" },
                { "           (00000000-0000-0000-0000-000000000000)" },
                { "" },
                { "  X        Four hexadecimal values enclosed in braces, where the" },
                { "           fourth value is a subset of eight hexadecimal values" },
                { "           that is also enclosed in braces:" },
            };

            try {
                options.Parse (args);
            } catch (Exception e) {
                Error (e.Message);
                return 2;
            }

            if (showHelp) {
                options.WriteOptionDescriptions (Console.Out);
                // Mono.Options throws 'InvalidOperationException: Invalid option description'
                // if the following is provided directly in the OptionSet because of the {} braces.
                Console.WriteLine ("           {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}");
                return 1;
            }

            if (showVersion) {
                Console.WriteLine (version);
                return 0;
            }

            string FormatGuid (Guid guid)
            {
                switch (guidFormat) {
                case GuidFormat.N:
                    return guid.ToString ("N");
                case GuidFormat.B:
                    return guid.ToString ("B");
                case GuidFormat.P:
                    return guid.ToString ("P");
                case GuidFormat.X:
                    return guid.ToString ("X");
                case GuidFormat.Base64:
                    return Convert.ToBase64String (guid.ToByteArray ());
                case GuidFormat.D:
                default:
                    return guid.ToString ("D");
                }
            }

            for (uint i = 0; i < numberOfIds; i++) {
                string id = null;

                switch (kind) {
                case IdKind.GuidV4:
                    id = FormatGuid (Guid.NewGuid ());
                    break;
                case IdKind.GuidV3:
                    id = FormatGuid (GuidV3 (guidNamespace, guidName));
                    break;
                case IdKind.GuidV5:
                    id = FormatGuid (GuidV5 (guidNamespace, guidName));
                    break;
                case IdKind.Xcode:
                    id = XcodeId ();
                    break;
                case IdKind.Nanoid:
                    id = Nanoid.Nanoid.Generate (nanoidAlphabet, nanoidSize);
                    break;
                }

                if (filter.HasFlag (Filter.Uppercase) && guidFormat != GuidFormat.Base64)
                    id = id.ToUpperInvariant ();

                Console.WriteLine (id);
            }

            return 0;
        }

        static void Error (string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine ($"Error: {message}");
            Console.ResetColor ();
        }

        static string XcodeId ()
            => Nanoid
                .Nanoid
                .Generate (
                    alphabet: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
                    size: 8)
                .Insert(3, "-")
                .Insert(6, "-");
    }
}