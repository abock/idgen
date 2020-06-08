//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2020 Aaron Bockover.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mono.Options;

using static Xamarin.GuidHelpers;

namespace Idgen
{
    public enum GuidEncoding
    {
        BigEndian,
        MixedEndian
    }

    public enum GuidFormat
    {
        D,
        N,
        B,
        P,
        X,
        Base64,
        Short
    }

    public abstract class GuidGenerator : IIdGenerator
    {
        GuidEncoding encoding;
        GuidFormat format;
        bool uppercase;

        public string Command { get; }
        public string CommandDescription { get; }
        public virtual string UsageArguments { get; }
        public OptionSet Options { get; }

        protected GuidGenerator(
            string command,
            string commandDescription)
        {
            Command = command;
            CommandDescription = commandDescription;
            Options = new OptionSet
            {
                {
                    "f|format=",
                    "{FORMAT} to use (see GUID FORMATS).",
                    v =>
                    {
                        if (!Enum.TryParse<GuidFormat> (v, true, out format))
                            throw new Exception ($"Invalid FORMAT '{v}'for the -format option.");
                    }
                },
                {
                    "u|upper",
                    "Output the GUID in upper-case (except for when -format is 'Base64' or 'Short').",
                    v => uppercase = v != null
                },
                {
                    "me|mixed-endian",
                    "Use mixed endian encoding whereby the first three components are " +
                    "little-endian and the last two are big-endian. " +
                    "See https://en.wikipedia.org/wiki/Universally_unique_identifier#Encoding",
                    v => encoding = v is null
                        ? GuidEncoding.BigEndian
                        : GuidEncoding.MixedEndian
                },
                { "" },
                { "GUID FORMATS:" },
                { "" },
                { "  Base64   The binary representation of the GUID encoded in base 64." } ,
                { "           This format ignores the -upper option if specified." },
                { "" },
                { "  Short    Like Base64, but with padding (==) stripped," },
                { "           '/' changed to '_', and '+' changed to '-'." },
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
                { "           {{0x00000000,0x0000,0x0000,{{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}" }
            };
        }

        static Guid ToMixedEndian(Guid guid)
        {
            var bytes = guid.ToByteArray();
            Array.Reverse(bytes, 0, 4);
            Array.Reverse(bytes, 4, 2);
            Array.Reverse(bytes, 6, 2);
            return new Guid(bytes);
        }

        static string FormatGuid(GuidFormat format, Guid guid)
        {
            switch (format)
            {
                case GuidFormat.N:
                    return guid.ToString("N");
                case GuidFormat.B:
                    return guid.ToString("B");
                case GuidFormat.P:
                    return guid.ToString("P");
                case GuidFormat.X:
                    return guid.ToString("X");
                case GuidFormat.Base64:
                    return Convert.ToBase64String(guid.ToByteArray());
                case GuidFormat.Short:
                    return Convert.ToBase64String(guid.ToByteArray())
                        .Replace("/", "_")
                        .Replace("+", "-")
                        .Substring(0, 22);
                case GuidFormat.D:
                default:
                    return guid.ToString("D");
            }
        }

        protected abstract Guid GenerateGuid(IEnumerable<string> args);

        public IEnumerable<string> Generate(IEnumerable<string> args = null)
            => Generate(encoding, format, args);

        public IEnumerable<string> Generate(
            GuidEncoding encoding,
            GuidFormat format,
            IEnumerable<string> args = null)
        {
            var guid = GenerateGuid(args ?? Array.Empty<string>());

            if (encoding == GuidEncoding.MixedEndian)
                guid = ToMixedEndian(guid);

            var guidString = FormatGuid(format, guid);

            switch (format)
            {
                case GuidFormat.Base64:
                case GuidFormat.Short:
                    yield return guidString;
                    break;
                default:
                    if (uppercase)
                        yield return guidString.ToUpperInvariant();
                    else
                        yield return guidString;
                    break;
            }
        }

        public sealed class V4 : GuidGenerator
        {
            public V4() : base(
                "v4",
                "Generate a random version 4 GUID (default ID type if no other type options are specified).")
            {
            }

            protected override Guid GenerateGuid(IEnumerable<string> args)
                => Guid.NewGuid();
        }

        public abstract class NamespaceNameGuidGenerator : GuidGenerator
        {
            readonly Func<Guid, string, Guid> generator;

            public override string UsageArguments { get; } = "NAME [NAMESPACE]";

            protected NamespaceNameGuidGenerator(
                string command,
                string commandDescription,
                Func<Guid, string, Guid> generator) : base(
                    command,
                    commandDescription)
                => this.generator = generator;

            protected sealed override Guid GenerateGuid(IEnumerable<string> args)
            {
                var name = args.FirstOrDefault();
                if (string.IsNullOrEmpty(name))
                    throw new Exception("Must specify NAME as the first positional argument.");

                if (name == "-")
                    name = Console.In.ReadToEnd();

                var @namespace = args.Skip(1).FirstOrDefault();
                Guid namespaceGuid = default;

                if (!string.IsNullOrEmpty(@namespace))
                {
                    switch (@namespace.ToLowerInvariant())
                    {
                        case "url":
                            namespaceGuid = GuidNamespace.URL;
                            break;
                        case "dns":
                            namespaceGuid = GuidNamespace.DNS;
                            break;
                        case "oid":
                            namespaceGuid = GuidNamespace.OID;
                            break;
                        default:
                            if (!Guid.TryParse(@namespace, out namespaceGuid))
                                throw new Exception("Invalid namespace GUID");
                            break;
                    }
                }

                if (namespaceGuid == default)
                    namespaceGuid = GuidNamespace.URL;

                return generator(namespaceGuid, name);
            }

            public IEnumerable<string> Generate(
                GuidEncoding encoding,
                GuidFormat format,
                string name,
                string @namespace = null)
            {
                if (name is null)
                    throw new ArgumentNullException(nameof(name));

                var args = new List<string>(2);
                args.Add(name);
                if (@namespace is object)
                    args.Add(@namespace);
                
                return Generate(encoding, format, args);
            }
        }

        public sealed class V3 : NamespaceNameGuidGenerator
        {
            public V3() : base(
                "v3",
                "Generate a version 3 GUID based on a MD5 hash of NAME and an optional NAMESPACE. NAMESPACE may " +
                "be a GUID, 'URL', or 'DNS'. The 'URL' namespace (RFC 4122) will be used if one is not specified.",
                GuidV3)
            {
            }
        }

        public sealed class V5 : NamespaceNameGuidGenerator
        {
            public V5() : base(
                "v5",
                "Generate a version 5 GUID based on a SHA-1 hash of NAME and an optional NAMESPACE. NAMESPACE may " +
                "be a GUID, 'URL', or 'DNS'. The 'URL' namespace (RFC 4122) will be used if one is not specified.",
                GuidV5)
            {
            }
        }
    }
}
