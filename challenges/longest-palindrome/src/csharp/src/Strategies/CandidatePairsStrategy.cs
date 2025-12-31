using System.Runtime.InteropServices;
using Squire.LongestPalindromeChallenge.Infrastructure;

namespace Squire.LongestPalindromeChallenge.Strategies;

internal class CandidatePairsStrategy : StrategyBase
{
    /// <inheritdoc />
    public override Strategy Strategy => Strategy.CandidatePairs;

    /// <inheritdoc />
    public override Palindrome? Solve(string value)
    {
        // If the value has a single character, that is considered a valid palindrome
        // and no further processing is required.

        if (value is { Length: 1 })
        {
            return new Palindrome(value, 0, 1);
        }

        var letters = new Dictionary<char, List<int>>(95);
        var rawCandidates = new List<CandidatePair>();
        var valueSpan = value.AsSpan();

        // Scan forward and process each symbol in the value, collecting candidate pairs
        // when the same symbol is found again later in the sequence.

        for (var index = 0; index < valueSpan.Length; ++index)
        {
            var symbol = valueSpan[index];

            // If this symbol hasn't been previously seen, add it to the dictionary.  Since there
            // can be only one occurrence, we know this is not a candidate pair.

            if (!letters.TryGetValue(symbol, out var positions))
            {
                positions = new List<int>();
                letters[symbol] = positions;

                positions.Add(index);
            }
            else
            {
                // This symbol has been seen before, so we know that it is a candidate.  Create the
                // candidate pairs for each previous occurrence of this symbol and the current index.
                // Because the scan is moving forward, any previous occurrence is guaranteed to be before
                // the current index.

                foreach (var startIndex in positions)
                {
                    rawCandidates.Add(new CandidatePair(startIndex, index));
                }

                positions.Add(index);
            }
        }

        // Now that all candidate pairs have been identified, sort by length and evaluate.  The first palindrome
        // is guaranteed to be the longest.  In the case where multiple palindromes have the same length, the one
        // which occurs earlier in the string is preferred.

        var candidates = CollectionsMarshal.AsSpan(rawCandidates);

        candidates.Sort((left, right) =>
        {
            var lengthComparison = right.Length.CompareTo(left.Length);

            return lengthComparison != 0
                ? lengthComparison
                : left.StartIndex.CompareTo(right.StartIndex);
        });

        foreach (var candidate in candidates)
        {
            var slice = valueSpan.Slice(candidate.StartIndex, candidate.Length);

            if (IsPalindrome(slice))
            {
                return new Palindrome(slice.ToString(), candidate.StartIndex, candidate.Length);
            }
        }

        // If no palindrome was found, then no solution exists.

        return default;
    }
}
