//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright 2018-2024 Aaron Bockover.
// Licensed under the MIT License.

using Xunit;

namespace Idgen;

public sealed class NumberParseTests
{
    delegate bool TryParseInt32Delegate(string numberString, out int number);

    static readonly TryParseInt32Delegate s_tryParseInt32 = typeof(Program)
        .Assembly
        .GetType("Idgen.NumberParse")
        .GetMethod("TryParse", [typeof(string), typeof(int).MakeByRefType()])
        .CreateDelegate<TryParseInt32Delegate>();

    static bool TryParse(string numberString, out int number)
        => s_tryParseInt32(numberString, out number);

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
        Assert.True(TryParse(str, out var actual));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ggg ")]
    [InlineData("123456789012345678901234567890")]
    [InlineData("-")]
    public void TryParse_should_fail(string str)
        => Assert.False(TryParse(str, out _));
}
