using System;
using System.Linq;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var cleanedString = input.ToLower()
            .Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c))
            .ToArray();

        var reversedString = cleanedString.Reverse().ToArray();

        return new string(cleanedString).Equals(new string(reversedString));
    }
}
