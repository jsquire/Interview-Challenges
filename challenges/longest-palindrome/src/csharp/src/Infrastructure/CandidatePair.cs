namespace Squire.LongestPalindromeChallenge.Infrastructure;

/// <summary>
///   A pair of indexes representing the start and end position
///   of a candidate substring within a string.
/// </summary>
///
internal struct CandidatePair
{
    /// <summary>The start index of the substring.</summary>
    public readonly int StartIndex;

    /// <summary>The length of the substring.</summary>
    public readonly int Length;

    /// <summary>
    ///   Initializes a new <see cref="CandidatePair"/> instance.
    /// </summary>
    ///
    /// <param name="startIndex">The start index of the substring.</param>
    /// <param name="endIndex">The end index of the substring.</param>
    ///
    public CandidatePair(int startIndex, int endIndex) =>
        (StartIndex, Length) = (startIndex, endIndex - startIndex + 1);
}
