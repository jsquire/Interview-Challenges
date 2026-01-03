namespace Squire.LongestPalindromeChallenge.Infrastructure;

/// <summary>
///   A pair of indexes representing the start and end position
///   of a candidate substring within a string.
/// </summary>
///
/// <remarks>
///   This struct is mutable to allow candidate length adjustment
///   as the substring is evaluated without needing to create new instances.
///
///   IMPORTANT: Because this struct is mutable, it should not be used as a key
///   in hash-based collections.
/// </remarks>
///
internal struct CandidatePair
{
    /// <summary>
    ///   Represents the absence of a symbol for the pair.
    /// </summary>
    ///
    public const char NoSymbol = '\0';

    /// <summary>
    ///   The symbol of the candidate, if meaningful in the context.
    /// </summary>
    ///
    /// <value>The symbol of the candidate, if known; otherwise, contains the <see cref="NoSymbol" /> value.</value>
    ///
    public readonly char Symbol;

    /// <summary>The start index of the substring.</summary>
    public readonly int StartIndex;

    /// <summary>
    ///   The length of the substring.
    /// </summary>
    ///
    /// <remarks>
    ///   This member is mutable.
    /// </remarks>
    ///
    public int Length;

    /// <summary>
    ///   Initializes a new <see cref="CandidatePair"/> instance.
    /// </summary>
    ///
    /// <param name="startIndex">The start index of the substring.</param>
    /// <param name="endIndex">The end index of the substring.</param>
    ///
    public CandidatePair(int startIndex,
                         int endIndex) : this(NoSymbol, startIndex, endIndex)
    {
    }

    /// <summary>
    ///   Initializes a new <see cref="CandidatePair"/> instance.
    /// </summary>
    ///
    /// <param name="symbol">The symbol of the candidate.</param>
    /// <param name="startIndex">The start index of the substring.</param>
    /// <param name="endIndex">The end index of the substring.</param>
    ///
    public CandidatePair(char symbol,
                         int startIndex,
                         int endIndex) => (Symbol, StartIndex, Length) = (symbol, startIndex, endIndex - startIndex + 1);
}
