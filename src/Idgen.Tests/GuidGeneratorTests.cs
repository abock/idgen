//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2020 Aaron Bockover.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using Xunit;

namespace Idgen
{
    public sealed class GuidGeneratorTests
    {
        public static IEnumerable<object[]> RoundTripFormats()
        {
            yield return new object[] { GuidEncoding.BigEndian, GuidFormat.D };
            yield return new object[] { GuidEncoding.BigEndian, GuidFormat.N };
            yield return new object[] { GuidEncoding.BigEndian, GuidFormat.B };
            yield return new object[] { GuidEncoding.BigEndian, GuidFormat.P };
            yield return new object[] { GuidEncoding.BigEndian, GuidFormat.X };
            yield return new object[] { GuidEncoding.MixedEndian, GuidFormat.D };
            yield return new object[] { GuidEncoding.MixedEndian, GuidFormat.N };
            yield return new object[] { GuidEncoding.MixedEndian, GuidFormat.B };
            yield return new object[] { GuidEncoding.MixedEndian, GuidFormat.P };
            yield return new object[] { GuidEncoding.MixedEndian, GuidFormat.X };
        }

        void AssertRoundTrip(GuidFormat format, string expected)
            => Assert.Equal(expected, new Guid(expected).ToString(format.ToString()));

        [Theory]
        [MemberData(nameof(RoundTripFormats))]
        public void RoundTrip_V4(GuidEncoding encoding, GuidFormat format)
            => AssertRoundTrip(
                format,
                new GuidGenerator.V4().Generate(encoding, format));

        void AssertRoundTripNamespaceName(
            GuidGenerator.NamespaceNameGuidGenerator generator,
            GuidEncoding encoding,
            GuidFormat format,
            string expectedValue)
        {
            var guidString = generator.Generate(
                encoding,
                format,
                "bojangles",
                "11de2b26-984e-56b4-aa25-b3bd28ea5ac2");

            Assert.Equal(new Guid(expectedValue).ToString(format.ToString()), guidString);
            
            AssertRoundTrip(format, guidString);
        }

        [Theory]
        [MemberData(nameof(RoundTripFormats))]
        public void RoundTrip_V5(GuidEncoding encoding, GuidFormat format)
            => AssertRoundTripNamespaceName(
                new GuidGenerator.V5(),
                encoding,
                format,
                encoding == GuidEncoding.MixedEndian
                    ? "a42594de-dde8-0b51-8e00-b6ac890c733a"
                    : "de9425a4-e8dd-510b-8e00-b6ac890c733a");

        [Theory]
        [MemberData(nameof(RoundTripFormats))]
        public void RoundTrip_V3(GuidEncoding encoding, GuidFormat format)
            => AssertRoundTripNamespaceName(
                new GuidGenerator.V3(),
                encoding,
                format,
                encoding == GuidEncoding.MixedEndian
                    ? "bb4be529-3cf6-1f3c-85e0-4f02f9805a1a"
                    : "29e54bbb-f63c-3c1f-85e0-4f02f9805a1a");
    }
}
