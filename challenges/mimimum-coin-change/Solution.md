# The Minimum Coin Change Challenge #

### Solutions Overview ###

There are two generally accepted approaches to solve this challenge.  Though others exist, those discussed here are those favored for discussion and implementation during interview scenarios.  One employs a greedy strategy, which is fairly straight forward and intuitive but which only works on specific sets of denominations.  Fortunately, one of those sets is American currency - which can help serve as an approachable starting point.

The other approach works as a general case solution at the cost of complexity, turning the challenge into a series of related decision problems.  These can be solved recursively, but that isn't the most efficient strategy as the sub-problems tend to repeat frequently.  Instead, the general solution to this challenge is best expressed via dynamic programming.

Each strategy is discussed in more detail below.  Regardless of the strategy used, there are two possible outcomes - either you can make exact change or you cannot.  When it is possible to make change, the result should detail amount of each denomination used.

### The Greedy Approach ###

**_Steps_**

For the greedy strategy, first order set of coins by denomination, from highest to lowest.  Starting with the highest denomination, consider each coin denomination in order, until you've either made the correct change or have run out of coin denominations.  For each denomination:

1. Calculate the maximum number coins from the denomination where their total doesn't exceed the remaining desired amount of change.  Since we aren't interested in fractions of coins, this calculation is: `floor(change amount / coin denomination)`.  
    
    - If the number of coins is 0 and there are no denominations left to consider, you can't make change.
     
1. Subtract the total amount of the coins selected in step #1 from the remaining total amount of change needed.
  
    - If the remaining amount of change needed is 0, then you've made change and know the coins used.
      
    - If the remaining amount of change needed is more than 0 and there are no denominations left to consider, you can't make change.

1. If there are coin denominations remaining, move to the next and repeat the previous steps.

**_Example_**

To help illustrate, let's use a concrete example of making change for $0.63 with American coins, giving us the set:  
`{ 25 cent, 10 cent, 5 cent, 1 cent }`.   

1. Consider the 25 cent coin and take 2 for a total of $0.50.  This leaves $0.13 remaining.
1. Consider the 10 cent coin and take 1 for a total of $0.10.  This leaves $0.03 remaining.
1. Consider the  5 cent coin and take 0 because $0.05 > $0.03. This leaves $0.03 remaining.
1. Consider the  1 cent coin and take 3 for a total of $0.03.  This leaves nothing remaining.

We're done.  We were able to make change for $0.63 using `{ 25 cent x 2, 10 cent x 1, 1 cent x 3 }`

**_Complexity_**

Time Complexity: `O(n)`, where `n` is the count of coin denominations in the set that you're making change from.

**_Special Considerations_**

Unfortunately, the greedy approach only works well for specific sets of coin denominations.  For the general case, it will overvalue the larger denominations and miss the case where a smaller one would result in using less coins used.  To make matters worse, the act of deciding whether a set of denominations can be solved greedily is an NP-complete problem.  Unless you know upfront that it will work with your input, it's best to avoid the greedy approach for this challenge.

### The Dynamic Programming Approach ###

To avoid the pitfalls of the greedy approach, a static formula to decide how many coins of a given denomination to use isn't adequate.  Instead, for each coin denomination, you must consider whether using it or ignoring it results in the lesser amount of coins.  The final answer for a coin denomination is based on this decision.

Each combination of coin denomination and amount of change desired requires this choice; to make that it, you need to know the answer for other denomination/change combinations, making it likely that combinations will be needed multiple times.  Because of this, it is more efficient to calculate each combination before it would be needed, so that the answer is available for reference.  Since each denomination/change combination depends on answers with values smaller than the current one being considered, calculations are best performed in the order of smallest to largest.

**_General Strategy_**

Though implementations may vary, a good mental model for this is to think about building a table of answers, in a specific order, allowing answers to reference one another.  Since that is all a bit abstract, let's look at a concrete illustration.  Say that you have a set of coins with denominations `{ $0.03, $0.01 }` and we'd like to make change of $0.04.  Our goal is go build up the table:

   
|                | $0.00 |              $0.01           |              $0.02           |              $0.03           |              $0.04           |
|:--------------:|:-----:|:----------------------------:|:----------------------------:|:----------------------------:|:----------------------------:|
|**0 cent coin** |   -   |                -             |                -             |                -             |                -             |
|**1 cent coin** |   -   | 1&#162; x 1<br />3&#162; x 0 | 1&#162; x 2<br />3&#162; x 0 | 1&#162; x 3<br />3&#162; x 0 | 1&#162; x 4<br />3&#162; x 0 |
|**3 cent coin** |   -   |                -             |                -             | 1&#162; x 0<br />3&#162; x 1 | 1&#162; x 1<br />3&#162; x 1 |    

    * Rows represent a coin denomination
    * Columns represent the amount of change needed
    * A dash (-) indicates that change cannot be made of the combination

In the above, for the change amount of $0.04 the decision is:

- Use the 3 cent coin and, possibly, the 1 cent coin
- Ignore the 3 cent coin, using only the 1 cent coin

If you choose to use the 3 cent coin, do so by selecting the maximum number where the amount doesn't exceed the change needed.  Then, look at the remaining coin denominations and change needed - in this case, only the 1 cent coin remains and you still need change of $0.01.  Since you've already calculated the best answer to that question, look in row `[1 cent]` and column `[$0.01]` and find the answer is `use one 1 cent coin`.  Then combine, giving you the result: `use one 3 cent coin and use one 1 cent coin` to make $0.04.

If you choose to ignore the 3 cent coin, do so by looking at the best answer for the remaining coin denominations and change amount - in this case, only the 1 cent coin remains and you still need change of $0.04.  Since you've already calculated the best answer to that question, look in row `[1 cent]` and column `[$0.04]` and find the answer is `use four 1 cent coins`.   

**_Details_**

- Each cell of the table is calculated by deciding whether to use a denomination of coin or not to.  In some cases, there may be no way to make change for a given denomination/change amount combination.  When this is true, it should be considered the least desirable outcome and the other option selected if change can successfully be made. 

- When building the table, sort the coins from lowest to highest.  Begin with the lowest and consider it as the only denomination in the set.  Once you've filled out the entire row, push the next highest denomination onto the beginning of the set.   For example, if you have the set `{ 3 cent, 2 cent, 1 cent }`, the first row would be built using `{ 1 cent }`, the next with `{ 2 cent, 1 cent }`, and the last with `{ 3 cent, 2 cent, 1 cent }`.  

- When seeking an answer and a coin denomination is ignored, pretend that the set doesn't include it.  For example, if you have the set `{ 3 cent, 2 cent, 1 cent }` and you're ignoring the 3 cent coin, pretend the set is `{ 2 cent, 1 cent }`. _[ This is often represented in shorthand by using the highest number in the set and assuming the rest are included - such as in the preceding discussion.]_  

- Most often, a 0 coin denomination and change amount are included when building the table and the row/column associated with each is assumed to be a scenario where change cannot be made.  It is possible to skip this step and just assume `cannot make change` any time the set of denominations is empty or the amount of change needed is 0.  

- When a coin is included, you will always select the maximum number of that denomination where the value does not exceed the amount of change needed, just like in the greedy algorithm.  The calculation is: `floor(change amount / coin denomination)`

- The answer for each table cell depends on the cell directly "above" the current cell, the maximum calculation, and one additional cell in the row directly "above" and a column somewhere "to the left" of the current cell.  It is because of this that the table is built lowest-to-highest. 

**_Example_**

To help illustrate, let's use a concrete example of making change for $0.08 with the set of coins: `{ 6 cent, 4 cent, 1 cent }`.  

1. Sort the denominations from lowest to highest, giving us `{ 1 cent, 4 cent, 6 cent }`  

1. Insert a zero cent coin at the beginning of the set, giving us `{ 0 cent, 1 cent, 4 cent, 6 cent }`    

1. Start building the table row for the 0 cent coin, looping from no change needed to $0.08, in one cent intervals.  

The result is:

|           | $0.00 |            $0.01         |            $0.02         |            $0.03         |            $0.04         |            $0.05         |            $0.06         |            $0.07         |            $0.08         |
|:---------:|:-----:|:------------------------:|:------------------------:|:------------------------:|:------------------------:|:------------------------:|:------------------------:|:------------------------:|:------------------------:|
|**0 cent** |   -   |              -           |              -           |              -           |              -           |              -           |              -           |              -           |              -           |
  

4. Move to the next coin denomination in the set, 1 cent, and build the table row.  

The result is:

|           | $0.00 |                      $0.01                    |                      $0.02                    |                       $0.03                   |                     $0.04                     |                      $0.05                    |                     $0.06                    |                       $0.07                    |                      $0.08                    |
|:---------:|:-----:|:---------------------------------------------:|:---------------------------------------------:|:---------------------------------------------:|:---------------------------------------------:|:---------------------------------------------:|:--------------------------------------------:|:----------------------------------------------:|:---------------------------------------------:|
|**0 cent** |   -   |                        -                      |                        -                      |                        -                      |                        -                      |                        -                      |                       -                      |                         -                      |                        -                      |
|**1 cent** |   -   | 1&#162; x 1<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 2<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 3<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 4<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 5<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 6<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 7<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 8<br />4&#162; x 0<br />6&#162; x 0 |
  

5. Move to the next coin denomination in the set, the 4 cent coin, and build the table row.  
    - The first three cells don't have a choice, as a 4 cent coin can't be used for change.  The only option is to use the values from the cell directly above.  
    
    - Column `[$0.04]` is the first decision.  
        - If a 4 cent coin is used, the calculation is `floor(4 / 4) = 1`.  With a 4 cent coin used, the remaining change is 0.  The answer is 1 coin.  
        
        - If the 4 cent coin is ignored, the answer is the cell above: 4 of the 1 cent coins.  The answer is 4 coins.
          
        - Because using the 4 cent coin results in fewer coins needed, that answer is used.  The value of the cell is `1 cent x 0, 4 cent x 1, 6 cent x 0`  

    - Column `[$0.05]` is the next decision.
        - If a 4 cent coin is used, the calculation is `floor(5 / 4) = 1`.  With a 4 cent coin used, the remaining change is 1.  Look at row `[1 cent]`, column `[$0.01]` for the remainder of the answer, which uses a single 1 cent coin.  The answer is 2 coins.  
        
        - If the 4 cent coin is ignored, the answer is the cell above: 5 of the 1 cent coins.  The answer is 5 coins.  
        
        - Because using the 4 cent coin results in fewer coins needed, that answer is used.  The value of the cell is `1 cent x 1, 4 cent x 1, 6 cent x 0`

    - Next is the column for `[$0.06]`.
        - If a 4 cent coin is used, the calculation is `floor(6 / 4) = 1`.  With a 4 cent coin used, the remaining change is 2.  Look at row `[1 cent]`, column `[$0.02]` for the remainder of the answer, which uses two of the 1 cent coins.  The answer is 3 coins.  
        
        - If the 4 cent coin is ignored, the answer is the cell above: 6 of the 1 cent coins.  The answer is 6 coins.  
        
        - Because using the 4 cent coin results in fewer coins needed, that answer is used.  The value of the cell is `1 cent x 2, 4 cent x 1, 6 cent x 0`      
              
     - Next is the column for `[$0.07]`.
        - If a 4 cent coin is used, the calculation is `floor(7 / 4) = 1`.  With a 4 cent coin used, the remaining change is 3.  Look at row `[1 cent]`, column `[$0.03]` for the remainder of the answer, which uses three of the 1 cent coins.  The answer is 3 coins.  
        
        - If the 4 cent coin is ignored, the answer is the cell above: 7 of the 1 cent coins.  The answer is 7 coins.  
        
        - Because using the 4 cent coin results in fewer coins needed, that answer is used.  The value of the cell is `1 cent x 3, 4 cent x 1, 6 cent x 0`


     - Finally, the column for `[$0.08]`.
        - It a 4 cent coin is used, the calculation is `floor(8 / 4) = 2`.  With two 4 cent coins used, the remaining change is 0.  The answer is 2 coins.  
        
        - If the 4 cent coin is ignored, the answer is the cell above: 8 of the 1 cent coins.  The answer is 8 coins.  
        
        - Because using the 4 cent coin results in fewer coins needed, that answer is used.  The value of the cell is `1 cent x 0, 4 cent x 2, 6 cent x 0`

The result is:
             
|           | $0.00 |                      $0.01                    |                      $0.02                    |                       $0.03                   |                     $0.04                     |                      $0.05                    |                     $0.06                    |                       $0.07                    |                      $0.08                    |
|:---------:|:-----:|:---------------------------------------------:|:---------------------------------------------:|:---------------------------------------------:|:---------------------------------------------:|:---------------------------------------------:|:--------------------------------------------:|:----------------------------------------------:|:---------------------------------------------:|
|**0 cent** |   -   |                        -                      |                        -                      |                        -                      |                        -                      |                        -                      |                       -                      |                         -                      |                        -                      |
|**1 cent** |   -   | 1&#162; x 1<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 2<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 3<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 4<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 5<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 6<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 7<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 8<br />4&#162; x 0<br />6&#162; x 0 |
|**4 cent** |   -   | 1&#162; x 1<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 2<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 3<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 0<br />4&#162; x 1<br />6&#162; x 0 | 1&#162; x 1<br />4&#162; x 1<br />6&#162; x 0 | 1&#162; x 2<br />4&#162; x 1<br />6&#162; x 0 | 1&#162; x 3<br />4&#162; x 1<br />6&#162; x 0 | 1&#162; x 0<br />4&#162; x 2<br />6&#162; x 0 |
  

6. Move to the next coin denomination in the set, the 6 cent coin, and build the table row.  
    - The first five cells don't have a choice, as a 6 cent coin can't be used for change.  The only option is to use the values from the cell directly above.  
    
    - Column `[$0.06]` is the first decision.  
        - If a 6 cent coin is used, the calculation is `floor(6 / 6) = 1`.  With a 4 cent coin used, the remaining change is 0.  The answer is 1 coin.  
        
        - If the 6 cent coin is ignored, the answer is the cell above: 1 of the 4 cent coins and two of the 1 cent coins.  The answer is 3 coins.
          
        - Because using the 6 cent coin results in fewer coins needed, that answer is used.  The value of the cell is `1 cent x 0, 4 cent x 0, 6 cent x 1`  
  
     - Next is the column for `[$0.07]`.
        - If a 6 cent coin is used, the calculation is `floor(7 / 6) = 1`.  With a 6 cent coin used, the remaining change is 1.  Look at row `[4 cents]`, column `[$0.01]` for the remainder of the answer, which uses one of the 1 cent coins.  The answer is 2 coins.  
        
        - If the 6 cent coin is ignored, the answer is the cell above: 1 of the 4 cent coins and 3 of the 1 cent coins.  The answer is 4 coins.  
        
        - Because using the 6 cent coin results in fewer coins needed, that answer is used.  The value of the cell is `1 cent x 1, 4 cent x 0, 6 cent x 1`

     - Finally, the column for `[$0.08]`.
        - It a 6 cent coin is used, the calculation is `floor(8 / 6) = 1`.  With one 6 cent coins used, the remaining change is 2.  Look at row `[4 cents]`, column `[$0.02]` for the remainder of the answer, which uses two of the 1 cent coins.  The answer is 3 coins.
        
        - If the 6 cent coin is ignored, the answer is the cell above: 2 of the 4 cent coins.  The answer is 2 coins.  
        
        - Because ignoring the 6 cent coin results in fewer coins needed, that answer is used.  The value of the cell reflects using 2 of the 4 cent coins (and no others).
             
The result is:

|           | $0.00 |                      $0.01                    |                      $0.02                    |                       $0.03                   |                     $0.04                     |                      $0.05                    |                     $0.06                    |                       $0.07                    |                      $0.08                    |
|:---------:|:-----:|:---------------------------------------------:|:---------------------------------------------:|:---------------------------------------------:|:---------------------------------------------:|:---------------------------------------------:|:--------------------------------------------:|:----------------------------------------------:|:---------------------------------------------:|
|**0 cent** |   -   |                        -                      |                        -                      |                        -                      |                        -                      |                        -                      |                       -                      |                         -                      |                        -                      |
|**1 cent** |   -   | 1&#162; x 1<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 2<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 3<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 4<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 5<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 6<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 7<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 8<br />4&#162; x 0<br />6&#162; x 0 |
|**4 cent** |   -   | 1&#162; x 1<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 2<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 3<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 0<br />4&#162; x 1<br />6&#162; x 0 | 1&#162; x 1<br />4&#162; x 1<br />6&#162; x 0 | 1&#162; x 2<br />4&#162; x 1<br />6&#162; x 0 | 1&#162; x 3<br />4&#162; x 1<br />6&#162; x 0 | 1&#162; x 0<br />4&#162; x 2<br />6&#162; x 0 |
|**6 cent** |   -   | 1&#162; x 1<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 2<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 3<br />4&#162; x 0<br />6&#162; x 0 | 1&#162; x 0<br />4&#162; x 1<br />6&#162; x 0 | 1&#162; x 1<br />4&#162; x 1<br />6&#162; x 0 | 1&#162; x 0<br />4&#162; x 0<br />6&#162; x 1 | 1&#162; x 1<br />4&#162; x 0<br />6&#162; x 1 | 1&#162; x 0<br />4&#162; x 2<br />6&#162; x 0 |
  

7. Now that the table has been completed, the final answer can be found in the bottom right-most cell.  In this case, row `[6 cent]`, column `[$0.08]`.  The answer correctly identifies that the fewest possible coins to make $0.08 change is 2 (both 4 cent coins) with our coin denominations of `{ 1 cent, 4 cent, 6 cent }`.

**_Complexity_**

Time Complexity: `O(d * a)`, where `d` is the count of coin denominations in the set that you're making change from, and `a` is the count of the amounts of change that need to be calculated.  Because this approach calculates change in increments of 1, starting at 0 and counting up to the desired amount of change, `a` will be the desired `amount + 1`.  For example, change of $0.03 means `a = 4` as answers for $0.00, $0.01, $0.02, and $0.03 need to be calculated.

**_Special Considerations_**

It is worth noting that if we were to have employed a greedy strategy with the above example, we would end up with an incorrect answer of 3 coins, as there would have been one 6 cent coin and two 1 cent coins selected.  As detailed in the considerations for the greedy approach, it will overvalue the larger denominations and miss the case where a smaller one would result in using less coins.  Since it isn't practical to determine whether or not to use the greedy approach on-the-fly, it is recommended to use the dynamic programming approach whenever you're not already sure that your given set of denominations is appropriate for greedy consideration.

### Other Approaches ###

This challenge can be implemented using a recursive approach, though it is not one of the commonly sought answers to the problem when interviewing, at least not in my experience.  Because the individual calculations repeat frequently, a recursive implementation is encouraged to memoize calculations as they are performed to avoid duplicating effort.  

Because only the needed calculations are made in a recursive approach, it can result in fewer calculations than the the dynamic programming approach which may perform calculations that aren't used in the final answer due to making calculations before they're actually needed.  In practice, however, it is generally accepted that there is little to no performance savings with recursion due to the overhead of the recursive function calls, where the dynamic programming strategy performs calculations inline.   

### Additional Resources ###

- [Change-making problem (Wikipedia)](https://en.wikipedia.org/wiki/Change-making_problem)
- [Minimum coins to make change (Emery Riddle University)](http://www.mathcs.emory.edu/~cheung/Courses/323/Syllabus/DynProg/money-change.html)
- [Find the minimum number of coins that make a given value (GeeksForGeeks)](https://www.geeksforgeeks.org/find-minimum-number-of-coins-that-make-a-change/)
- [The Coin Change Problem (Sohag's Blog)](https://sohagbuet.wordpress.com/2014/03/27/coin-change-problem/)
- [Dynamic Programming (Wikipedia)](https://en.wikipedia.org/wiki/Dynamic_programming)
- [Greedy Algorithms (Wikipedia)](https://en.wikipedia.org/wiki/Greedy_algorithm)
- [NP-completeness (Wikipedia)](https://en.wikipedia.org/wiki/NP-completeness)
- [Memoization (Wikipedia)](https://en.wikipedia.org/wiki/Memoization)