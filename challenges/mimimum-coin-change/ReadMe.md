# The Minimum Coin Change Challenge #

### Problem Statement ###

Assume that you are coding a cash register to be used by employees at a store.  When a customer is due change, the register will decide how to disperse it using the smallest number of coins.  For example, a customer using American currency is due $0.17 and should receive a dime, a nickel, and two pennies.  Because your cash register could be used in different areas that use different currencies, it should be able to support different sets of coins with different denominations.  For example, it would be valid to have a set of coins with denominations of 10, 8, 5, 2, and 1.

Change can only be dispensed when the exact amount due can be produced from the available denominations. In cases where change cannot be dispensed, the customer is provided a credit voucher by the cashier.

### Expected Input ###

- A set of coins of specific denominations.  For example:  `{ 8, 4, 3, 1 }`

- An amount of change due.  For example: `18`

### Expected Output ###

- If it is possible to produce the exact amount due with the available denominations, a set containing the smallest possible number of coins that should be dispensed for change.  For example: `{ (8, 1); (3, 2); (1, 1) }`

- If it is not possible to produce the exact amount due with the available denominations, an empty set.

### Constraints and Assumptions ###

- The set of denominations and change due must be provided; failure to do so may be considered invalid.

- The set of denominations may be empty and the change due may be 0.  In either of these cases, it is not possible to make change, and the corresponding output should be produced. 

- Each denomination and change amount will be expressed as an integer and will be within the range of a 32-bit representation.

- Each denomination and change amount will be positive; negative values may be considered invalid.

- The number of different denominations will not exceed 100; a set containing more denominations than that may be considered invalid.

- There are an infinite number of coins for each denomination in the available set.

- The representation of the input and output sets is purview of the implementation; so long as the concept is clearly expressed any format or structure is allowable.

### Solution Discussion ###
  Discussion of possible solutions and implementations can be found [here](./Solution.md).
