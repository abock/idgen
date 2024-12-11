//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2024 Aaron Bockover.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Options;

using static Idgen.GuidFactory;

namespace Idgen;

public abstract class GuidGenerator : IIdGenerator
{
    GuidEncoding _encoding;
    GuidFormat _format;
    bool _uppercase;

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
                    if (!Enum.TryParse<GuidFormat> (v, true, out _format))
                        throw new Exception ($"Invalid FORMAT '{v}'for the -format option.");
                }
            },
            {
                "u|upper",
                "Output the GUID in upper-case (except for when -format is 'Base64' or 'Short').",
                v => _uppercase = v != null
            },
            {
                "me|mixed-endian",
                "Use mixed endian encoding whereby the first three components are " +
                "little-endian and the last two are big-endian. " +
                "See https://en.wikipedia.org/wiki/Universally_unique_identifier#Encoding",
                v => _encoding = v is null
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
        => format switch
        {
            GuidFormat.N => guid.ToString("N"),
            GuidFormat.B => guid.ToString("B"),
            GuidFormat.P => guid.ToString("P"),
            GuidFormat.X => guid.ToString("X"),
            GuidFormat.Base64 => Convert.ToBase64String(guid.ToByteArray()),
            GuidFormat.Short => Convert.ToBase64String(guid.ToByteArray())
                .Replace("/", "_")
                .Replace("+", "-")
                [..22],
            _ => guid.ToString("D"),
        };

    protected abstract Guid GenerateGuid(IEnumerable<string> args);

    public IEnumerable<string> Generate(IEnumerable<string> args = null)
        => Generate(_encoding, _format, args);

    public IEnumerable<string> Generate(
        GuidEncoding encoding,
        GuidFormat format,
        IEnumerable<string> args = null)
    {
        var guid = GenerateGuid(args ?? []);

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
                if (_uppercase)
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

    public abstract class NamespaceNameGuidGenerator(
        string command,
        string commandDescription,
        Func<Guid, string, Guid> generator) : GuidGenerator(
            command,
            commandDescription)
    {
        public override string UsageArguments { get; } = "NAME [NAMESPACE]";

        protected sealed override Guid GenerateGuid(IEnumerable<string> args)
        {
            var name = args.FirstOrDefault();
            if (string.IsNullOrEmpty(name))
                throw new Exception("Must specify NAME as the first positional argument.");

            if (name is "-")
                name = Console.In.ReadToEnd();

            var @namespace = args.Skip(1).FirstOrDefault();
            Guid namespaceGuid = default;

            if (!string.IsNullOrEmpty(@namespace))
            {
                switch (@namespace.ToLowerInvariant())
                {
                    case "url":
                        namespaceGuid = Rfc4122Namespace.URL;
                        break;
                    case "dns":
                        namespaceGuid = Rfc4122Namespace.DNS;
                        break;
                    case "oid":
                        namespaceGuid = Rfc4122Namespace.OID;
                        break;
                    default:
                        if (!Guid.TryParse(@namespace, out namespaceGuid))
                            throw new Exception("Invalid namespace GUID");
                        break;
                }
            }

            if (namespaceGuid == default)
                namespaceGuid = Rfc4122Namespace.URL;

            return generator(namespaceGuid, name);
        }

        public IEnumerable<string> Generate(
            GuidEncoding encoding,
            GuidFormat format,
            string name,
            string @namespace = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(name));
            return Generate(
                encoding,
                format,
                @namespace is null
                    ? [name]
                    : [name, @namespace]);
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
