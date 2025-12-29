# The Longest Palindromic Substring Challenge

### Problem statement

Assume that you are building a text analysis tool that identifies interesting patterns within strings.  One common pattern is a palindrome—a sequence of characters that reads the same forward and backward.  Given a string, your tool should find the longest contiguous substring that forms a palindrome.  For example, given the string `"babad"`, the longest palindromic substring is either `"bab"` or `"aba"`, both of which have length 3.

A substring is defined as a consecutive range of characters taken from the string.  For example, in `"abc"`, `"ab"` is a substring, but `"ac"` is not because the characters are not contiguous.

### Expected input

- A string of characters.  For example: `"babad"`

### Expected output

- If a palindromic substring exists, a string representing one of the longest palindromic substrings found within the input.  For example: `"bab"` or `"aba"`

- If multiple substrings of the same maximum length qualify, any one of them is acceptable.

### Constraints and assumptions

- The input string must be provided; failure to do so may be considered invalid.

- The string will not be empty; its length can be assumed small enough to easily fit in memory.  Unless a parameter allowing a longer length is present, a general constraint of 2,000 characters or less may be assumed.

- The string will contain only printable ASCII characters.

- Case sensitivity applies; `A` is different from `a`.

- Whitespace, punctuation, and symbols are treated as normal characters and participate in palindrome matching.

- The palindrome must be contiguous within the string.

- If the input is a single character, that character is itself a palindrome and should be returned.

- Null inputs do not need to be handled.

- The representation of the input and output is purview of the implementation; so long as the concept is clearly expressed any format or structure is allowable.

### Solution discussion

  Discussion of possible solutions and implementations can be found [here](./Solution.md).
