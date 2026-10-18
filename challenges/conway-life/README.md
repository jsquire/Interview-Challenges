# Conway's Game of Life Challenge

### Problem statement

Conway's Game of Life is a cellular automaton that operates on a two-dimensional grid.  Each cell is either alive or dead, and its state in the next generation depends on the eight cells surrounding it.

Given an initial set of live cells, your task is to run the simulation for a specified number of generations and return the resulting population.  The coordinates can fall anywhere in the signed 64-bit range, so the board may be much larger than the starting population suggests.

### Rules

Each cell's neighbors are the eight positions immediately surrounding it, including diagonals.  The cell itself is not counted, and positions outside the signed 64-bit coordinate range are excluded.

- A live cell with fewer than two live neighbors dies.
- A live cell with two or three live neighbors stays alive.
- A live cell with more than three live neighbors dies.
- A dead cell with exactly three live neighbors becomes alive.  All other dead cells remain dead.

These rules apply to every cell using the completed state of the previous generation.  Changes take effect together, so a birth or death cannot affect another cell's outcome in the same generation.

### Expected input

- A set of live cells in the [Life 1.06 format](https://conwaylife.com/wiki/Life_1.06).  The first line is `#Life 1.06`, followed by one pair of space-separated x and y coordinates per line.

   For example:

  ```text
  #Life 1.06
  0 1
  1 2
  2 0
  2 1
  2 2
  -2000000000000 -2000000000000
  -2000000000001 -2000000000001
  -2000000000000 -2000000000001
  ```

### Expected output

- The set of live cells after the requested number of generations, in the same [Life 1.06 format](https://conwaylife.com/wiki/Life_1.06).

### Constraints and assumptions

- The input is read interactively from standard input by default, or from a Life 1.06 file using `--file`.

- The output is written to standard output.

- The representation of the input and output is the [Life 1.06 format](https://conwaylife.com/wiki/Life_1.06).

- Each coordinate is expressed as a pair of signed 64-bit integers.

- Each cell has up to eight neighbors, consisting of the cells immediately surrounding it within the coordinate range.

- Generations are computed as a pure function, meaning that the next generation is based on the final state of the previous generation and not any interim states of calculating the next generation, per [the rules](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life). 

- The initial population is limited to 10,000 distinct live cells.  Duplicate coordinates and comments do not count toward the limit.  Input containing more cells is invalid.  This constraint should allow the simulation to run in memory and be limited to ~5 GB of RAM use.  _(see: [Problem space](#our-chosen-limits))_

- The number of generations may be specified using `--generations` and defaults to 10.  Values from 0 through 30 are allowed.  This constraint should allow the simulation to complete in ~12 minutes on modern hardware.  _(see: [Problem space](#our-chosen-limits))_

### Problem space

#### The full coordinate space

Allowing the full signed 64-bit range on both axes gives us `2^128` possible locations.  If every location were occupied, we would need more than 1,000,000,000 trillion exabytes of storage, assuming a rough allowance of 28 bytes per coordinate.

Even if we could store that population, visiting each cell once at an assumed rate of 500,000 cells per second would take more than 1,000,000,000 trillion years.  That's just enumerating the population, without doing any of the work needed to calculate the next generation.  For the simulation to run fully in memory and complete in a reasonable time, we need a much smaller problem to work with.

#### Even a bounded starting population

Limiting the input helps, but doesn't tell us how large the population might become.  Say that we allowed 500,000 initial live cells and ran the simulation for 100 generations.

Since a new live cell needs live neighbors, the population can extend by at most one cell in any direction each generation.  After one generation, every live cell must be within a 3-by-3 square around one of the initial cells.  After two generations, those squares are 5-by-5.  After 100 generations, they are 201-by-201, giving us 40,401 possible locations for each initial cell.

More generally, for `N0` initial cells and `g` generations, the population cannot exceed `N0 * (2g + 1)^2`.  This overcounts areas where the squares overlap and includes cells that the rules would leave dead or that fall outside the coordinate range.  It gives us an upper limit to work with, not a prediction of how a pattern will grow.

With 500,000 initial cells, that limit is about 20.2 billion cells after 100 generations.  Using our estimate of 28 bytes per coordinate, we would need about 566 GB just for that population.  Keeping both the current and next populations in memory would bring the total to more than a terabyte.

For the time estimates, assume that we can process 500,000 live cells per second, including the work to examine neighbors and calculate the next population.

| Measure | Estimate |
| --- | --- |
| Initial population | 500,000 cells, ~14 MB of memory |
| Population limit after 99 generations | ~19.8 billion cells |
| Population limit after 100 generations | ~20.2 billion cells |
| Memory for the population after 100 generations | ~566 GB |
| Memory for both populations during generation 100 | ~1.12 TB |
| Time to calculate generation 100 from generation 99 | ~11 hours |
| Time to enumerate the resulting population once | ~11.2 hours |
| Total time to calculate all 100 generations | ~15.4 days |

The last generation takes about 11 hours because it processes the population from generation 99, not the larger population it produces.  To estimate the full run, we add up the population limits for generations 0 through 99 and divide by our processing rate.  For `G` generations, that sum is `N0 * G * (4G^2 - 1) / 3`.

The starting population fits comfortably in memory, but under these assumptions the full run could take more than two weeks.  Limiting the input alone isn't enough.  We also need to limit how many generations we calculate.

#### Our chosen limits

To limit the run to ~12 minutes and ~5 GB of RAM, we chose a maximum of 10,000 initial cells and 30 generations.  These maximums give us an upper limit of 37.21 million live cells.  At our assumed processing rate, all 30 generations take about 12 minutes.  Allowing 28 bytes per coordinate, the current and next populations together need ~2 GB at the final transition.

### Solution discussion

  Discussion of possible solutions and implementations can be found [here](./Solution.md).
  
### References and resources

- [Conway's Game of Life (Wikipedia)](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life)
- [Life 1.06 format](https://conwaylife.com/wiki/Life_1.06)
