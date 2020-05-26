//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2020 Aaron Bockover.
// Licensed under the MIT License.

using System;

using Xunit;

namespace Idgen
{
    public sealed class NumberParseTests
    {
        [Theory]
        [InlineData("0", 0)]
        [InlineData("0x0", 0x0)]
        [InlineData("0h", 0x0)]
        [InlineData("0b0", 0b0)]
        [InlineData("-1", -1)]
        [InlineData("-0x1", -0x1)]
        [InlineData("-1h", -0x1)]
        [InlineData("-0b1", -0b1)]
        [InlineData("0xabcd", 0xabcd)]
        [InlineData("0Xabcd", 0Xabcd)]
        [InlineData("abcdh", 0xabcd)]
        [InlineData("0b1001", 0b1001)]
        [InlineData("1_2_3", 1_2_3)]
        [InlineData("1,2,3", 1_2_3)]
        [InlineData("  - 100, 213, 35  ", -100_213_35)]
        public void TryParse_should_succeed(string str, int expected)
        {
            Assert.True(NumberParse.TryParse(str, out var actual));
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ggg ")]
        [InlineData("123456789012345678901234567890")]
        [InlineData("-")]
        public void TryParse_should_fail(string str)
            => Assert.False(NumberParse.TryParse(str, out _));
    }
}
