using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Squire.LongestPalindromeChallenge.Infrastructure;

namespace Squire.LongestPalindromeChallenge.Strategies;

internal class CandidatePairsOptimizedStrategy : StrategyBase
{
    /// <inheritdoc />
    public override Strategy Strategy => Strategy.CandidatePairsOptimized;

    /// <inheritdoc />
    public override Palindrome? Solve(string value)
    {
        var valueSpan = value.AsSpan();

        // If the value is a valid palindrome, no further processing is required.  A
        // single-character string is is considered a valid palindrome and should be treated
        // as such.

        if ((valueSpan is { Length: 1 }) || (IsPalindrome(valueSpan)))
        {
            return new Palindrome(value, 0, value.Length);
        }

        var symbols = new Dictionary<char, List<int>>(CalculatePrintableAsciiCount());
        var longestPalindrome = default(CandidatePair);
        var lastSeenPalindrome = default(CandidatePair);

        // When tracking candidate pairs, the goal is to be able to identify the longest potential
        // palindrome.  Since there can be only a single result, we can avoid paying the full sort cost
        // in cases where there is a longer palindrome present by using a heap rather than sorting a set.
        // To avoid the need for a custom comparer, the priority information is encoded into a single long value.

        var candidateCapacity = EstimateCandidateCapacity(valueSpan.Length);
        var candidates = new PriorityQueue<CandidatePair, long>(candidateCapacity);

        // Scan forward and process each symbol in the value, collecting candidate pairs
        // when the same symbol is found again later in the sequence.

        for (var index = 0; index < valueSpan.Length; ++index)
        {
            var symbol = valueSpan[index];

            // Since a repeating string of the same symbol is guaranteed to be a palindrome, track candidates
            // inline rather than immediately expanding into their component pairs.

            if (lastSeenPalindrome.Symbol == symbol)
            {
                ++lastSeenPalindrome.Length;
                _ = TrackSymbolPosition(symbols, symbol, index);
            }
            else
            {
                // If the symbol has changed, test the last seen palindrome against the longest found so far.
                // If it is longer then the last seen will replace the current longest palindrome, so the
                // previous longest must be expanded into pairs for future processing.

                if ((lastSeenPalindrome.Length > 1) && (lastSeenPalindrome.Length > longestPalindrome.Length))
                {
                    ExpandPairs(longestPalindrome, candidates, symbols, longestPalindrome.Length);
                    longestPalindrome = lastSeenPalindrome;
                }
                else
                {
                    // If the last seen palindrome is not longer than the longest found so far, then
                    // expand the last seen palindrome into candidate pairs for future processing.

                    ExpandPairs(lastSeenPalindrome, candidates, symbols, lastSeenPalindrome.Length);
                }

                lastSeenPalindrome = new CandidatePair(symbol, index, index);

                // Track the symbol and its position.  If this symbol was previously seen, we know there
                // is alt least one other occurrence, so it should be tracked as a candidate pair.

                var (symbolSeen, positions) = TrackSymbolPosition(symbols, symbol, index);

                if (symbolSeen)
                {
                    // Create the candidate pairs for each previous occurrence of this symbol and the current index.
                    // Because the scan is moving forward, any previous occurrence is guaranteed to be before
                    // the current index.

                    foreach (var startIndex in CollectionsMarshal.AsSpan(positions))
                    {
                        var candidatelength = (index - startIndex + 1);

                        // There is no benefit to enqueuing candidates that are shorter than the longest
                        // palindrome that has been discovered thus far.

                        if ((candidatelength > 1) && (candidatelength > longestPalindrome.Length))
                        {
                            var candidate = new CandidatePair(symbol, startIndex, index);
                            candidates.Enqueue(candidate, ComputeHeapPriority(candidate));
                        }
                    }
                }
            }
        }

        // After the scan is complete, check the last seen palindrome to see if it is the longest and
        // expand the candidate set for the shorter candidate.

        if (lastSeenPalindrome.Length > longestPalindrome.Length)
        {
            ExpandPairs(longestPalindrome, candidates, symbols, lastSeenPalindrome.Length);
            longestPalindrome = lastSeenPalindrome;
        }
        else
        {
            ExpandPairs(lastSeenPalindrome, candidates, symbols, longestPalindrome.Length);
        }

        // If there was a palindrome made up of at least two repeating symbols, check to see if it is longer than
        // the biggest candidate; if so, that is the result.

        if (longestPalindrome.Length > 1)
        {
            candidates.TryPeek(out var longestCandidate, out _);

            if (longestPalindrome.Length >= longestCandidate.Length)
            {
                return CreatePalindrome(longestPalindrome);
            }
        }

        // Process each candidate pair in order of longest to shortest, checking for palindromes.

        while (candidates.TryDequeue(out var candidate, out _))
        {
            var slice = valueSpan.Slice(candidate.StartIndex, candidate.Length);

            // A single-character candidate is technically a palindrome, but should only be
            // the result if it comprises the entire string.  Otherwise, only candidates with
            // two or more characters are valid.

            if (slice.Length > 1)
            {
                // If the current candidate is shorter than the longest palindrome found so far, then
                // all remaining candidates will also be shorter; no further processing is required.

                if (slice.Length <= longestPalindrome.Length)
                {
                    return CreatePalindrome(longestPalindrome);
                }

                if (IsPalindrome(slice))
                {
                    return new Palindrome(slice.ToString(), candidate.StartIndex, candidate.Length);
                }
            }
        }

        // All candidates potentially longer than the tracked palindrome have been evaluated and
        // none were palindromes.  If a palindrome was found during the inline scan, return it.

        if (longestPalindrome.Length > 1)
        {
            return CreatePalindrome(longestPalindrome);
        }

        // If no palindrome was found, then no solution exists.

        return default;
    }

    /// <summary>
    ///   Tracks a symbol and its position within the source string.
    /// </summary>
    ///
    /// <param name="symbols">The set tracking candidate symbols and their position.</param>
    /// <param name="symbol">The symbol to record the position of.</param>
    /// <param name="position">The position of the symbol to record.</param>
    ///
    /// <returns>A tuple consisting of a boolean indicating if the symbol had been seen before and the position tracking set reference.</returns>
    ///
    /// <remarks>
    ///   The <paramref name="candidates" /> collection will be modified by this method.
    /// </remarks>
    ///
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (bool SymbolSeen, List<int> Positions) TrackSymbolPosition(Dictionary<char, List<int>> symbols,
                                            char symbol,
                                            int position)
    {
        var symbolSeen = symbols.TryGetValue(symbol, out var positions);

        if (!symbolSeen)
        {
            positions = new List<int>();
            symbols[symbol] = positions;
        }

        positions!.Add(position);
        return (symbolSeen, positions);
    }

    /// <summary>
    ///   Expands a single symbol palindrome candidate into all possible candidate pairs to
    ///   enable future scanning.
    /// </summary>
    ///
    /// <param name="singleSymbolCandidate">The single symbol candidate.</param>
    /// <param name="candidates">The collection of candidate pairs.</param>
    /// <param name="symbols">The collection of symbol locations within the source string.</param>
    /// <param name="longestPalindromeLength">The length of the longest palindrome found so far.</param>
    ///
    /// <remarks>
    ///   The <paramref name="candidates" /> collection will be modified by this method.
    /// </remarks>
    ///
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ExpandPairs(CandidatePair singleSymbolCandidate,
                                    PriorityQueue<CandidatePair, long> candidates,
                                    Dictionary<char, List<int>> symbols,
                                    int longestPalindromeLength)
    {
        // If the candidate is not associated with a symbol, it cannot be expanded.

        if (singleSymbolCandidate.Symbol == CandidatePair.NoSymbol)
        {
            return;
        }

        // Expand the candidate into all possible pairs and register for future pairing.  Since the symbol
        // positions have already been recorded, they can be used to create pairs.

        var symbol = singleSymbolCandidate.Symbol;
        var positions = symbols[symbol];
        var startIndex = singleSymbolCandidate.StartIndex;
        var endIndex = singleSymbolCandidate.StartIndex + singleSymbolCandidate.Length;

        // Create pairs for positions earlier in the string than the current index.

        for (var index = startIndex; index < endIndex; ++index)
        {
            foreach (var position in CollectionsMarshal.AsSpan(positions))
            {
                if (position < index)
                {
                    var candidatelength = (index - position + 1);

                    // There is no benefit to enqueuing candidates that are shorter than the longest
                    // palindrome that has been discovered thus far.  Since the positions are ordered,
                    // once a candidate is found to be too short, all subsequent candidates will also be
                    // too short and can be ignored.

                    if (candidatelength <= longestPalindromeLength)
                    {
                        break;
                    }

                    var candidate = new CandidatePair(symbol, position, index);
                    candidates.Enqueue(candidate, ComputeHeapPriority(candidate));
                }
            }
        }
    }

    /// <summary>
    ///   Computes a priority value for heap ordering that enables max-heap behavior
    ///   with stability for the default min-heap <see cref="PriorityQueue{TElement, TPriority}"/>.
    /// </summary>
    ///
    /// <param name="pair">The candidate pair to compute priority for.</param>
    ///
    /// <returns>A 64-bit priority value where lower values indicate higher priority.</returns>
    ///
    /// <remarks>
    ///   The .NET <see cref="PriorityQueue{TElement, TPriority}"/> is a min-heap, meaning
    ///   elements with the lowest priority value are dequeued first. To achieve max-heap
    ///   behavior (longest candidates first), we negate the length so that longer lengths
    ///   produce smaller (more negative) values.
    ///
    ///   For stability, the start index is used as a tiebreaker when lengths are equal.
    ///   Earlier positions in the string are preferred, so smaller start indices should
    ///   dequeue first.
    ///
    ///   Both values are packed into a single 64-bit integer. This avoids allocating a
    ///   custom comparer and enables fast single-instruction comparisons. The packing
    ///   uses the natural property of integer comparison: high-order bits dominate, so
    ///   the negated length (in the high 32 bits) is the primary sort key, and the start
    ///   index (in the low 32 bits) only affects ordering when lengths are equal.
    ///
    ///   The bit layout is:
    ///
    ///   |------- High 32 bits -------|------- Low 32 bits --------|
    ///   |     Negated Length         |       Start Index          |
    ///
    ///   For example, consider two candidates with the same length (10) at positions 0 and 5:
    ///
    ///     Candidate A (start=0, length=10):  ((-10) shifted left 32) | 0 = 0xFFFFFFF6_00000000
    ///     Candidate B (start=5, length=10):  ((-10) shifted left 32) | 5 = 0xFFFFFFF6_00000005
    ///
    ///   A is less than B, so A dequeues first (earlier position wins).
    /// </remarks>
    ///
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long ComputeHeapPriority(CandidatePair pair)
    {
        var negatedLength = (long)(-pair.Length);
        var startIndex = (uint)pair.StartIndex;

        return (negatedLength << 32) | startIndex;
    }

    /// <summary>
    ///   Creates a <see cref="Palindrome" /> from a <see cref="CandidatePair" />
    ///   instance.
    /// </summary>
    ///
    /// <param name="palindrome">The <see cref="CandidatePair" /> that represents a palindrome.</param>
    ///
    /// <returns>The <see cref="Palindrome" /> created from the candidate.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Palindrome CreatePalindrome(CandidatePair palindrome) =>
        new Palindrome(new string(palindrome.Symbol, palindrome.Length), palindrome.StartIndex, palindrome.Length);

    /// <summary>
    ///   Estimates the capacity needed for the candidate pair, using a simple approach
    ///   within a ~2% margin of error for pathological cases.
    /// </summary>
    ///
    /// <param name="stringLength">Length of the string being evaluated.</param>
    ///
    /// <returns>An estimate for the number of candidate pairs.</returns>
    ///
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int EstimateCandidateCapacity(int stringLength) => (stringLength * stringLength / 50);
}
