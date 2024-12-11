//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2024 Aaron Bockover.
// Licensed under the MIT License.

using System;
using System.Security.Cryptography;
using System.Text;

namespace Idgen;

/// <summary>
/// Create versions 3 and 5 GUID/UUIDs.
/// </summary>
public static class GuidFactory
{
    /// <summary>
    /// Predefined namespaces for Versions 3 and 5 GUIDs from
    /// <seealso href="https://tools.ietf.org/html/rfc4122#appendix-C">RFC 4122 Appendix C</seealso>.
    /// </summary>
    public static class Rfc4122Namespace
    {
        /// <summary>
        /// Name string is a fully-qualified domain name:
        /// <c>6ba7b810-9dad-11d1-80b4-00c04fd430c8</c>
        /// </summary>
        public static Guid DNS { get; } = new(
            0x6ba7b810,
            0x9dad,
            0x11d1,
            0x80, 0xb4, 0x00, 0xc0, 0x4f, 0xd4, 0x30, 0xc8);

        /// <summary>
        /// Name string is a URL:
        /// <c>6ba7b811-9dad-11d1-80b4-00c04fd430c8</c>
        /// </summary>
        public static Guid URL { get; } = new(
            0x6ba7b811,
            0x9dad,
            0x11d1,
            0x80, 0xb4, 0x00, 0xc0, 0x4f, 0xd4, 0x30, 0xc8);

        /// <summary>
        /// Name string is an ISO OID:
        /// <c>6ba7b812-9dad-11d1-80b4-00c04fd430c8</c>
        /// </summary>
        public static Guid OID { get; } = new(
            0x6ba7b812,
            0x9dad,
            0x11d1,
            0x80, 0xb4, 0x00, 0xc0, 0x4f, 0xd4, 0x30, 0xc8);

        /// <summary>
        /// Name string is an X.500 DN (in DER or a text output format):
        /// <c>6ba7b814-9dad-11d1-80b4-00c04fd430c8</c>
        /// </summary>
        public static Guid X500 { get; } = new(
            //
            0x6ba7b814,
            0x9dad,
            0x11d1,
            0x80, 0xb4, 0x00, 0xc0, 0x4f, 0xd4, 0x30, 0xc8);
    }

    /// <summary>
    /// Creates a <seealso href="https://en.wikipedia.org/wiki/Universally_unique_identifier#Versions_3_and_5_(namespace_name-based)">version 5 GUID/UUID</seealso>
    /// by combining a given <paramref name="namespaceGuid"/> and arbitrary <paramref name="name"/>,
    /// hashed together using <c>SHA-1</c>. See <see cref="GuidNamespace"/> for pre-defined namespaces.
    /// </summary>
    public static Guid GuidV5(Guid namespaceGuid, ReadOnlySpan<char> name)
    {
        using var sha1 = SHA1.Create();
        return CreateHashedGuid(sha1, 0x50, namespaceGuid, name);
    }

    /// <inheritdoc cref="GuidV5"/>
    public static Guid GuidV5(Guid namespaceGuid, string name)
        => GuidV5(namespaceGuid, name.AsSpan());

    /// <summary>
    /// Creates a <seealso href="https://en.wikipedia.org/wiki/Universally_unique_identifier#Versions_3_and_5_(namespace_name-based)">version 3 GUID/UUID</seealso>
    /// by combining a given <paramref name="namespaceGuid"/> and arbitrary <paramref name="name"/>,
    /// hashed together using <c>MD5</c>. See <see cref="Rfc4122Namespace"/> for pre-defined namespaces.
    /// </summary>
    public static Guid GuidV3(Guid namespaceGuid, ReadOnlySpan<char> name)
    {
        using var md5 = MD5.Create();
        return CreateHashedGuid(md5, 0x30, namespaceGuid, name);
    }

    /// <inheritdoc cref="GuidV3"/>
    public static Guid GuidV3(Guid namespaceGuid, string name)
        => GuidV3(namespaceGuid, name.AsSpan());

    static Guid CreateHashedGuid(
        HashAlgorithm hashAlgorithm,
        byte version,
        Guid namespaceGuid,
        ReadOnlySpan<char> name)
    {
        var namespaceBytes = namespaceGuid.ToByteArray();
        var nameBytes = Encoding.UTF8.GetBytes(name.ToString());
        Swap(namespaceBytes);

        hashAlgorithm.TransformBlock(namespaceBytes, 0, namespaceBytes.Length, null, 0);
        hashAlgorithm.TransformFinalBlock(nameBytes, 0, nameBytes.Length);

        var guid = hashAlgorithm.Hash;
        Array.Resize(ref guid, 16);

        guid[6] = (byte)((guid[6] & 0x0F) | version);
        guid[8] = (byte)((guid[8] & 0x3F) | 0x80);

        Swap(guid);
        return new(guid);

        void Swap(byte[] g)
        {
            void SwapAt(int left, int right)
                => (g[right], g[left]) = (g[left], g[right]);

            SwapAt(0, 3);
            SwapAt(1, 2);
            SwapAt(4, 5);
            SwapAt(6, 7);
        }
    }
}
