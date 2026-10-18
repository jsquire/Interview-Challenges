using NUnit.Framework;
using Squire.ConwayLifeChallenge.Infrastructure;

namespace Squire.ConwayLifeChallenge.Tests.Infrastructure;

/// <summary>
///   The suite of tests for the <see cref="StrategyBase" /> contract and its implementations.
/// </summary>
///
/// <remarks>
///   Every strategy runs the same cases.  Populations are deliberately small, with at most 30 generations,
///   so these tests exercise correctness rather than the resource limits of the application.
///   Shared patterns are copied only when needed to isolate strategy inputs from other cases and expected results.
///   Tests should complete within 15 seconds, but the enforced limit is 60 seconds to allow for parallel
///   execution overhead.  The elapsed-time limit detects slow completed cases.  NUnit cannot abort
///   uncooperative synchronous code, so <c>testconfig.json</c> enables a watchdog that terminates the test host after
///   60 seconds without test activity.  This fails the run without collecting a memory dump or requiring
///   a command-line argument.  The watchdog bounds unresponsive execution, not the total suite duration.
/// </remarks>
///
[TestFixture]
public class SimulationTests
{
    /// <summary>The maximum elapsed time allowed for a completed simulation test, in milliseconds.</summary>
    private const int MaximumTestMilliseconds = 60_000;

    /// <summary>
    ///   The base glider repeats its shape after four generations, shifted one cell along both axes.
    ///   Three complete cycles therefore mean 12 generations and displacement (3, 3).  Choosing
    ///   cycles first avoids truncating an arbitrary generation count.  For example, 13 / 4 is still
    ///   3, but generation 13 has a different shape that cannot be obtained by translating the original.
    /// </summary>
    ///
    private const int GliderGenerationsPerCycle = 4;

    /// <summary>
    ///   Enumerates every strategy so that new implementations automatically receive the shared coverage.
    /// </summary>
    ///
    /// <returns>The strategies declared by the application.</returns>
    ///
    public static IEnumerable<Strategy> GetStrategies() => Enum.GetValues<Strategy>();

    /// <summary>
    ///   Enumerates every alive/dead arrangement in the Moore neighborhoods at the locations supplied by
    ///   <see cref="GetNeighborhoodLocations" />.
    /// </summary>
    ///
    /// <returns>The neighborhood cases for every strategy.</returns>
    ///
    /// <remarks>
    ///   A neighborhood describes positions around a cell, not which of those positions are alive.
    ///   Each case chooses a set of live surrounding cells and separately makes the center alive or dead.
    ///   Eight surrounding positions give 256 arrangements, or 512 cases with both center states.
    ///   Edges and corners have fewer positions because coordinates outside the signed range are omitted.
    /// </remarks>
    ///
    /// <seealso href="https://mathworld.wolfram.com/MooreNeighborhood.html" />
    ///
    public static IEnumerable<TestCaseData> GetNeighborhoodCases()
    {
        foreach (var strategy in GetStrategies())
        {
            foreach (var (name, center) in GetNeighborhoodLocations())
            {
                var neighborhoods = new List<HashSet<Coordinate>> { new() };

                foreach (var neighbor in center.GetNeighbors())
                {
                    // Keep each population without this neighbor and add a copy with it.
                    // Capture the original count so newly added populations are not extended again in this pass.

                    var existingCount = neighborhoods.Count;

                    for (var index = 0; index < existingCount; ++index)
                    {
                        var withNeighbor = new HashSet<Coordinate>(neighborhoods[index])
                        {
                            neighbor
                        };

                        neighborhoods.Add(withNeighbor);
                    }
                }

                var caseName = name == "Interior"
                    ? $"{ nameof(SimulateAppliesRulesToEveryNeighborhood) }({ strategy }"
                    : $"{ nameof(SimulateAppliesRulesToEveryNeighborhood) }({ strategy },{ name }";

                for (var index = 0; index < neighborhoods.Count; ++index)
                {
                    var withoutCenter = neighborhoods[index];
                    var neighborCount = withoutCenter.Count;

                    // Prepare independent inputs before yielding either case, since a consumer may
                    // modify the first population before requesting the second.

                    var withCenter = new HashSet<Coordinate>(withoutCenter)
                    {
                        center
                    };

                    yield return new TestCaseData(strategy, center, withoutCenter, neighborCount, false)
                        .SetName($"{ caseName },arrangement={ index },alive=False)");

                    yield return new TestCaseData(strategy, center, withCenter, neighborCount, true)
                        .SetName($"{ caseName },arrangement={ index },alive=True)");
                }
            }
        }
    }

    /// <summary>
    ///   Combines the expected populations with every available strategy.
    /// </summary>
    ///
    /// <returns>The complete-population cases for every strategy.</returns>
    ///
    public static IEnumerable<TestCaseData> GetSimulationCases()
    {
        foreach (var strategy in GetStrategies())
        {
            foreach (var scenario in GetScenarios())
            {
                yield return new TestCaseData(strategy, scenario.Initial, scenario.Generations, scenario.Expected)
                    .SetName($"{ nameof(SimulateProducesExpectedPopulation) }({ strategy },{ scenario.Name })");
            }
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.CreateStrategy" /> method.
    /// </summary>
    ///
    [TestCaseSource(nameof(GetStrategies))]
    public void CreateStrategyReturnsRequestedStrategy(Strategy strategy)
    {
        var implementation = StrategyBase.CreateStrategy(strategy);
        Assert.That(implementation.Strategy, Is.EqualTo(strategy), "The factory should return an implementation of the requested strategy.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.CreateStrategy" /> method.
    /// </summary>
    ///
    [TestCase(-1)]
    [TestCase(int.MaxValue)]
    public void CreateStrategyRejectsUnknownStrategies(int value)
    {
        Assert.That(() => StrategyBase.CreateStrategy((Strategy)value),
            Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("strategy"),
            "An unknown strategy should fail rather than select an unrelated implementation.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Simulate" /> method.
    /// </summary>
    ///
    /// <remarks>
    ///   Each case checks the next state of the center cell in a Moore neighborhood.  Life uses
    ///   the center's current state and the number of live surrounding cells, including diagonals.
    ///   The center is not counted as its own neighbor.  The positions are described by
    ///   <see cref="GetNeighborhoodLocations" />.
    /// </remarks>
    ///
    [TestCaseSource(nameof(GetNeighborhoodCases))]
    [MaxTime(MaximumTestMilliseconds)]
    public void SimulateAppliesRulesToEveryNeighborhood(Strategy strategy,
                                                        Coordinate center,
                                                        HashSet<Coordinate> cells,
                                                        int neighborCount,
                                                        bool initiallyAlive)
    {
        var expectedAlive = (neighborCount == 3) || ((initiallyAlive) && (neighborCount == 2));
        var result = StrategyBase.CreateStrategy(strategy).Simulate(cells, 1);

        Assert.That(result.Contains(center), Is.EqualTo(expectedAlive), $"The cell at { center } initially alive={ initiallyAlive } with { neighborCount } live neighbors should follow B3/S23.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Simulate" /> method.
    /// </summary>
    ///
    [TestCaseSource(nameof(GetSimulationCases))]
    [MaxTime(MaximumTestMilliseconds)]
    public void SimulateProducesExpectedPopulation(Strategy strategy,
                                                   HashSet<Coordinate> initial,
                                                   int generations,
                                                   HashSet<Coordinate> expected)
    {
        var result = StrategyBase.CreateStrategy(strategy).Simulate(initial, generations);
        Assert.That(result, Is.EquivalentTo(expected), "The complete population should match, with no missing or additional live cells.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Simulate" /> method.
    /// </summary>
    ///
    [TestCaseSource(nameof(GetStrategies))]
    [MaxTime(MaximumTestMilliseconds)]
    public void SimulateDoesNotDependOnInputEnumerationOrder(Strategy strategy)
    {
        const int CycleCount = 3;
        const int GenerationCount = GliderGenerationsPerCycle * CycleCount;

        var initial = CreatePattern(".#.", "..#", "###");
        var reversedInitial = new HashSet<Coordinate>(initial.Reverse());
        var expected = Transform(initial, CycleCount, CycleCount);
        var implementation = StrategyBase.CreateStrategy(strategy);

        var forward = implementation.Simulate(initial, GenerationCount);
        Assert.That(forward, Is.EquivalentTo(expected), "Forward insertion order should produce the expected population.");

        var reversed = implementation.Simulate(reversedInitial, GenerationCount);
        Assert.That(reversed, Is.EquivalentTo(expected), "Reversing insertion order must not change the resulting population.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Simulate" /> method.
    /// </summary>
    ///
    [TestCaseSource(nameof(GetStrategies))]
    [MaxTime(MaximumTestMilliseconds)]
    public void SimulateSupportsIndependentCallsOnTheSameInstance(Strategy strategy)
    {
        var implementation = StrategyBase.CreateStrategy(strategy);
        var block = CreatePattern("##", "##");
        var blinker = CreatePattern("...", "###");
        var expectedBlinker = CreatePattern(".#.", ".#.", ".#.");

        Assert.That(implementation.Simulate([ ..block ], 2), Is.EquivalentTo(block), "The first call should return the stable block.");
        Assert.That(implementation.Simulate([ ..blinker ], 1), Is.EquivalentTo(expectedBlinker), "A later call must not retain cells or neighbor counts from the first population.");
        Assert.That(implementation.Simulate([], 3), Is.Empty, "An empty population must remain empty after earlier nonempty simulations.");
        Assert.That(implementation.Simulate([ ..block ], 1), Is.EquivalentTo(block), "A nonempty simulation should still work after an empty one.");

        // Unlike an empty input, extinction requires processing live cells before returning an
        // empty result.  The next call must not inherit state from that work.

        Assert.That(implementation.Simulate([ new(0, 0) ], 1), Is.Empty, "A single cell should become extinct even after earlier nonempty simulations.");
        Assert.That(implementation.Simulate(blinker, 1), Is.EquivalentTo(expectedBlinker), "A new population should evolve correctly after the previous population became extinct.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="StrategyBase.Simulate" /> method.
    /// </summary>
    ///
    [TestCaseSource(nameof(GetStrategies))]
    [MaxTime(MaximumTestMilliseconds)]
    public void SimulateMatchesRepeatedSingleGenerations(Strategy strategy)
    {
        const int CycleCount = 3;
        const int GenerationCount = GliderGenerationsPerCycle * CycleCount;

        var initial = CreatePattern(".#.", "..#", "###");
        var expected = Transform(initial, CycleCount, CycleCount);
        var implementation = StrategyBase.CreateStrategy(strategy);
        var incremental = new HashSet<Coordinate>(initial);

        for (var generation = 0; generation < GenerationCount; ++generation)
        {
            incremental = implementation.Simulate(incremental, 1);
        }

        var combined = StrategyBase.CreateStrategy(strategy).Simulate(initial, GenerationCount);

        Assert.That(incremental, Is.EquivalentTo(expected), "Repeated single steps should produce the expected glider position.");
        Assert.That(combined, Is.EquivalentTo(incremental), "A multi-generation call should agree with the same number of single steps.");
    }

    /// <summary>
    ///   Selects nine representative coordinates for exercising the Life rules with different
    ///   sets of available neighbor positions.
    /// </summary>
    ///
    /// <returns>The center coordinates and names used to identify their cases.</returns>
    ///
    /// <remarks>
    ///   The locations cover three scenarios:
    ///   <list type="bullet">
    ///     <item>
    ///       <term>Interior.</term>
    ///       <description>
    ///         The origin has all eight neighbors available.  It exercises the Life rules without
    ///         a coordinate boundary affecting which neighbor positions can contribute.
    ///       </description>
    ///     </item>
    ///     <item>
    ///       <term>Edges.</term>
    ///       <description>
    ///         Four locations place one axis at a signed limit and keep the other at zero, leaving
    ///         five neighbors.  They isolate the effect of a single boundary and exercise both
    ///         minimum and maximum limits on each axis.
    ///       </description>
    ///     </item>
    ///     <item>
    ///       <term>Corners.</term>
    ///       <description>
    ///         Four locations place both axes at signed limits, leaving three neighbors.  They
    ///         exercise two boundaries acting together, including every minimum/maximum combination
    ///         rather than assuming that correct edge behavior also guarantees correct corner behavior.
    ///       </description>
    ///     </item>
    ///   </list>
    ///
    ///   This method selects locations only.  <see cref="GetNeighborhoodCases" /> obtains their
    ///   surrounding positions through <see cref="Coordinate.GetNeighbors" /> and constructs every
    ///   alive/dead arrangement.  Neighbor enumeration and boundary filtering remain the responsibility
    ///   of that shared method, whose geometry is covered by <see cref="CoordinateTests" />.
    /// </remarks>
    ///
    /// <seealso href="https://mathworld.wolfram.com/MooreNeighborhood.html" />
    ///
    private static IEnumerable<(string Name, Coordinate Center)> GetNeighborhoodLocations()
    {
        yield return ("Interior", new(0, 0));
        yield return ("MinimumX", new(long.MinValue, 0));
        yield return ("MinimumY", new(0, long.MinValue));
        yield return ("MaximumX", new(long.MaxValue, 0));
        yield return ("MaximumY", new(0, long.MaxValue));
        yield return ("MinimumX_MinimumY", new(long.MinValue, long.MinValue));
        yield return ("MinimumX_MaximumY", new(long.MinValue, long.MaxValue));
        yield return ("MaximumX_MinimumY", new(long.MaxValue, long.MinValue));
        yield return ("MaximumX_MaximumY", new(long.MaxValue, long.MaxValue));
    }

    /// <summary>
    ///   Supplies small, known populations and their expected evolution.
    /// </summary>
    ///
    /// <returns>The named simulation scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetScenarios()
    {
        const int GliderCycleCount = 3;

        // Start with populations whose complete evolution is known, covering extinction,
        // stability, oscillation, movement, and simultaneous births and deaths.

        foreach (var scenario in GetExtinctionScenarios())
        {
            yield return scenario;
        }

        foreach (var scenario in GetZeroGenerationScenarios())
        {
            yield return scenario;
        }

        foreach (var scenario in GetStillLifeScenarios())
        {
            yield return scenario;
        }

        foreach (var scenario in GetOscillatorScenarios())
        {
            yield return scenario;
        }

        foreach (var scenario in GetGliderScenarios(GliderCycleCount))
        {
            yield return scenario;
        }

        foreach (var scenario in GetDensePopulationScenarios())
        {
            yield return scenario;
        }

        // Distant populations must stay independent, while nearby populations can grow close enough
        // to affect each other's live-neighbor counts.  Transformed gliders should follow the same
        // rules regardless of position or orientation.

        foreach (var scenario in GetIndependentPopulationScenarios())
        {
            yield return scenario;
        }

        foreach (var scenario in GetPartialExtinctionScenarios(GliderCycleCount))
        {
            yield return scenario;
        }

        foreach (var scenario in GetInteractingPopulationScenarios())
        {
            yield return scenario;
        }

        foreach (var scenario in GetTranslatedGliderScenarios(GliderCycleCount))
        {
            yield return scenario;
        }

        foreach (var scenario in GetOrientedGliderScenarios(GliderCycleCount))
        {
            yield return scenario;
        }

        // The coordinate limits exclude unrepresentable neighbor positions.  They must never connect
        // opposite sides of the board or interfere with distant populations.

        foreach (var scenario in GetCornerScenarios())
        {
            yield return scenario;
        }

        foreach (var scenario in GetEdgeScenarios())
        {
            yield return scenario;
        }

        foreach (var scenario in GetGliderBoundaryScenarios())
        {
            yield return scenario;
        }

        foreach (var scenario in GetNonWrappingScenarios())
        {
            yield return scenario;
        }
    }

    /// <summary>
    ///   Supplies a mixed-coordinate population that must be preserved without advancing time.
    /// </summary>
    ///
    /// <returns>The zero-generation identity scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetZeroGenerationScenarios()
    {
        // No generations means no normalization, clipping, or neighbor expansion.  Widely
        // separated extremes also expose unnecessary overflowing bounds calculations in this path.

        var cells = new HashSet<Coordinate>
        {
            new(long.MinValue, long.MinValue),
            new(long.MinValue, long.MaxValue),
            new(long.MaxValue, long.MinValue),
            new(long.MaxValue, long.MaxValue),
            new(-3, 7),
            new(0, -5),
            new(int.MinValue - 1L, int.MaxValue + 1L),
            new(9_007_199_254_740_993L, -9_007_199_254_740_993L)
        };

        yield return ("MixedCoordinateExtremes_0", new(cells), 0, cells);
    }

    /// <summary>
    ///   Supplies populations that cannot sustain life.
    /// </summary>
    ///
    /// <returns>The extinction scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetExtinctionScenarios()
    {
        var shortRuns = new[] { 0, 1, 30 };

        // Zero generations must preserve the input.  Later observations distinguish immediate
        // extinction from a population that incorrectly reappears after becoming empty.

        foreach (var generations in shortRuns)
        {
            yield return ($"Empty_{ generations }", [], generations, []);
            yield return ($"SingleCell_{ generations }", [ new(0, 0) ], generations, generations == 0 ? [ new(0, 0) ] : []);
        }

        yield return ("HorizontalPair", [ new(0, 0), new(1, 0) ], 1, []);
        yield return ("VerticalPair", [ new(0, 0), new(0, 1) ], 1, []);
        yield return ("DiagonalPair", [ new(0, 0), new(1, 1) ], 1, []);
    }

    /// <summary>
    ///   Supplies patterns that remain unchanged between generations.
    /// </summary>
    ///
    /// <returns>The still-life scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetStillLifeScenarios()
    {
        var shortRuns = new[] { 0, 1, 30 };
        var block = CreatePattern("##", "##");
        var stillLifes = new (string Name, HashSet<Coordinate> Cells)[]
        {
            ("Block", block),
            ("Beehive", CreatePattern(".##.", "#..#", ".##.")),
            ("Loaf", CreatePattern(".##.", "#..#", ".#.#", "..#.")),
            ("Boat", CreatePattern("##.", "#.#", ".#.")),
            ("Tub", CreatePattern(".#.", "#.#", ".#."))
        };

        // Different outlines exercise both surviving cells and adjacent dead cells that must
        // stay dead.  Each input is copied so a strategy cannot modify the shared expectation.

        foreach (var (name, cells) in stillLifes)
        {
            foreach (var generations in shortRuns)
            {
                yield return ($"{ name }_{ generations }", new(cells), generations, cells);
            }
        }
    }

    /// <summary>
    ///   Supplies patterns that alternate between two known phases.
    /// </summary>
    ///
    /// <returns>The oscillator scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetOscillatorScenarios()
    {
        var horizontalBlinker = CreatePattern("...", "###");
        var verticalBlinker = CreatePattern(".#.", ".#.", ".#.");
        var toad = CreatePattern("....", ".###", "###.");
        var nextToad = CreatePattern("..#.", "#..#", "#..#", ".#..");
        var beacon = CreatePattern("##..", "##..", "..##", "..##");
        var nextBeacon = CreatePattern("##..", "#...", "...#", "..##");
        var oscillatorRuns = new[] { 0, 1, 2, 3, 10, 29, 30 };

        // These patterns have a two-generation period.  Both parities, including consecutive
        // late observations, expose skipped steps and off-by-one generation counts.

        foreach (var generations in oscillatorRuns)
        {
            var even = generations % 2 == 0;

            yield return ($"HorizontalBlinker_{ generations }", new(horizontalBlinker), generations, even ? horizontalBlinker : verticalBlinker);
            yield return ($"VerticalBlinker_{ generations }", new(verticalBlinker), generations, even ? verticalBlinker : horizontalBlinker);
            yield return ($"Toad_{ generations }", new(toad), generations, even ? toad : nextToad);
            yield return ($"Beacon_{ generations }", new(beacon), generations, even ? beacon : nextBeacon);
        }
    }

    /// <summary>
    ///   Supplies a glider's early phases and positions after complete cycles.
    /// </summary>
    ///
    /// <param name="gliderCycleCount">The number of complete four-generation cycles for an additional observation.</param>
    ///
    /// <returns>The glider scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetGliderScenarios(int gliderCycleCount)
    {
        var glider = CreatePattern(".#.", "..#", "###");
        var firstPhase = CreatePattern("...", "#.#", ".##", ".#.");
        var secondPhase = CreatePattern("...", "..#", "#.#", ".##");
        var thirdPhase = CreatePattern("....", ".#..", "..##", ".##.");

        // Handwritten intermediate phases distinguish correct movement from merely translating
        // the starting shape, without calculating expectations through a simulation algorithm.

        yield return ("Glider_0", new(glider), 0, glider);
        yield return ("Glider_1", new(glider), 1, firstPhase);
        yield return ("Glider_2", new(glider), 2, secondPhase);
        yield return ("Glider_3", new(glider), 3, thirdPhase);

        var gliderCycleCounts = new[] { 1, 2, gliderCycleCount, 5 };

        // Observe complete cycles so the starting shape is also the expected shape.
        // The cycle count gives its displacement directly; multiplying by the period gives
        // the generation count without accidentally selecting one of the intermediate phases above.

        foreach (var cycleCount in gliderCycleCounts)
        {
            var generations = GliderGenerationsPerCycle * cycleCount;
            yield return ($"Glider_{ generations }", new(glider), generations, Transform(glider, cycleCount, cycleCount));
        }

        // A partial cycle changes the shape as well as the position.  Translate the handwritten
        // phase instead of the starting shape, including observations near the generation limit.

        var partialCycles = new (int Cycles, int Phase, HashSet<Coordinate> Cells)[]
        {
            (1, 1, firstPhase),
            (1, 2, secondPhase),
            (1, 3, thirdPhase),
            (7, 1, firstPhase),
            (7, 2, secondPhase)
        };

        foreach (var (cycles, phase, cells) in partialCycles)
        {
            var generations = (GliderGenerationsPerCycle * cycles) + phase;
            yield return ($"Glider_{ generations }", new(glider), generations, Transform(cells, cycles, cycles));
        }
    }

    /// <summary>
    ///   Supplies a crowded population with simultaneous births and deaths.
    /// </summary>
    ///
    /// <returns>The dense-population scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetDensePopulationScenarios()
    {
        // The filled square loses crowded cells while producing births outside its original
        // outline.  Reading partially updated neighbors can change either outcome.

        var dense = CreatePattern(".....", ".###.", ".###.", ".###.");

        yield return ("DenseSquare_1", new(dense), 1, CreatePattern("..#..", ".#.#.", "#...#", ".#.#.", "..#.."));
        yield return ("DenseSquare_2", new(dense), 2, CreatePattern("..#..", ".###.", "##.##", ".###.", "..#.."));
    }

    /// <summary>
    ///   Supplies distant patterns with different evolution.
    /// </summary>
    ///
    /// <returns>The independent-population scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetIndependentPopulationScenarios()
    {
        // The block must remain fixed while the blinker alternates.  Their separation prevents
        // legitimate interaction, so their combined result is the union of the known outcomes.

        var block = CreatePattern("##", "##");
        var horizontalBlinker = CreatePattern("...", "###");
        var verticalBlinker = CreatePattern(".#.", ".#.", ".#.");
        var distantBlock = Transform(block, 1_000, -1_000);

        yield return ("IndependentPopulations_1", [ ..horizontalBlinker, ..distantBlock ], 1, [ ..verticalBlinker, ..distantBlock ]);
        yield return ("IndependentPopulations_30", [ ..horizontalBlinker, ..distantBlock ], 30, [ ..horizontalBlinker, ..distantBlock ]);
    }

    /// <summary>
    ///   Supplies a moving population that outlives a separate pair of cells.
    /// </summary>
    ///
    /// <param name="gliderCycleCount">The number of complete four-generation cycles for the later observation.</param>
    ///
    /// <returns>The partial-extinction scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetPartialExtinctionScenarios(int gliderCycleCount)
    {
        var glider = CreatePattern(".#.", "..#", "###");
        var firstPhase = CreatePattern("...", "#.#", ".##", ".#.");
        var generations = GliderGenerationsPerCycle * gliderCycleCount;

        // The pair has too few neighbors to survive and is separated from the glider, which
        // moves away from it.  Only the pair should disappear after one generation; the later
        // observation ensures its extinction neither stops nor disrupts the glider's evolution.

        yield return ("PartialExtinction_1", [ ..glider, new(-5, -5), new(-4, -5) ], 1, firstPhase);
        yield return ($"PartialExtinction_{ generations }", [ ..glider, new(-5, -5), new(-4, -5) ], generations, Transform(glider, gliderCycleCount, gliderCycleCount));
    }

    /// <summary>
    ///   Supplies initially separated blinkers whose later generations interact.
    /// </summary>
    ///
    /// <returns>The delayed-interaction scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetInteractingPopulationScenarios()
    {
        // Horizontal blinkers at rows 0 and 3 first become adjacent vertical triples.
        // The adjacent triples contribute to the same live-neighbor counts, producing a filled
        // rectangle in the next generation rather than two independent blinkers.

        var initial = CreatePattern("###", "...", "...", "###");
        var firstGeneration = Transform(CreatePattern(".#.", ".#.", ".#.", ".#.", ".#.", ".#."), 0, -1);
        var secondGeneration = CreatePattern("###", "###", "###", "###");

        yield return ("InteractingBlinkers_1", new(initial), 1, firstGeneration);
        yield return ("InteractingBlinkers_2", new(initial), 2, secondGeneration);
    }

    /// <summary>
    ///   Supplies gliders at offsets that exercise coordinate precision and range.
    /// </summary>
    ///
    /// <param name="gliderCycleCount">The number of complete four-generation cycles to simulate.</param>
    ///
    /// <returns>The translated-glider scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetTranslatedGliderScenarios(int gliderCycleCount)
    {
        var glider = CreatePattern(".#.", "..#", "###");

        // Include negative positions, values beyond 32-bit integers and the consecutive-integer
        // precision of double, and positions near each signed limit.  The margins leave room
        // for the glider's movement, so translation alone must not change its evolution.

        var offsets = new (long X, long Y)[]
        {
            (-100, -200),
            (-2, -2),
            (int.MinValue - 2L, int.MinValue - 2L),
            (int.MaxValue - 2L, int.MaxValue - 2L),
            (int.MinValue - 10L, int.MaxValue + 10L),
            (9_007_199_254_740_993L, -9_007_199_254_740_993L),
            (long.MinValue + 10, long.MinValue + 10),
            (long.MinValue + 10, long.MaxValue - 10),
            (long.MaxValue - 10, long.MinValue + 10),
            (long.MaxValue - 10, long.MaxValue - 10)
        };
        var generations = GliderGenerationsPerCycle * gliderCycleCount;
        var translatedGlider = Transform(glider, gliderCycleCount, gliderCycleCount);

        // The offsets two cells below zero and each 32-bit limit cross those thresholds during
        // movement.  They must not become artificial boundaries within the signed 64-bit board.

        foreach (var (x, y) in offsets)
        {
            yield return ($"TranslatedGlider_{ x }_{ y }", Transform(glider, x, y), generations, Transform(translatedGlider, x, y));
        }
    }

    /// <summary>
    ///   Supplies reflected and transposed versions of the same glider evolution.
    /// </summary>
    ///
    /// <param name="gliderCycleCount">The number of complete four-generation cycles to simulate.</param>
    ///
    /// <returns>The oriented-glider scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetOrientedGliderScenarios(int gliderCycleCount)
    {
        var glider = CreatePattern(".#.", "..#", "###");
        var generations = GliderGenerationsPerCycle * gliderCycleCount;
        var translatedGlider = Transform(glider, gliderCycleCount, gliderCycleCount);
        var orientations = new (string Name, int DirectionX, int DirectionY)[]
        {
            ("NegateBothAxes", -1, -1),
            ("NegateX", -1, 1),
            ("NegateY", 1, -1),
            ("Unchanged", 1, 1)
        };

        // Negating coordinates reverses movement along the selected axes.  Swapping x and y
        // also covers the transposed shapes, exposing direction-dependent neighbor handling.

        foreach (var (name, directionX, directionY) in orientations)
        {
            var reflectedInitial = Transform(glider, 0, 0, directionX, directionY);
            var reflectedExpected = Transform(translatedGlider, 0, 0, directionX, directionY);
            var transposedInitial = Transform(glider.Select(cell => new Coordinate(cell.Y, cell.X)), 0, 0, directionX, directionY);
            var transposedExpected = Transform(translatedGlider.Select(cell => new Coordinate(cell.Y, cell.X)), 0, 0, directionX, directionY);

            yield return ($"ReflectedGlider_{ name }", reflectedInitial, generations, reflectedExpected);
            yield return ($"TransposedGlider_{ name }", transposedInitial, generations, transposedExpected);
        }
    }

    /// <summary>
    ///   Supplies births and stable populations at each coordinate-space corner.
    /// </summary>
    ///
    /// <returns>The corner scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetCornerScenarios()
    {
        var block = CreatePattern("##", "##");

        // Each direction points inward from its limit so the same diagram can occupy all four
        // corners.  Three cells should fill the missing corner and the resulting block should persist.

        var corners = new (long X, long Y, int DirectionX, int DirectionY)[]
        {
            (long.MinValue, long.MinValue, 1, 1),
            (long.MinValue, long.MaxValue, 1, -1),
            (long.MaxValue, long.MinValue, -1, 1),
            (long.MaxValue, long.MaxValue, -1, -1)
        };
        var missingCorner = CreatePattern(".#", "##");

        foreach (var (x, y, directionX, directionY) in corners)
        {
            var initial = Transform(missingCorner, x, y, directionX, directionY);
            var expected = Transform(block, x, y, directionX, directionY);

            yield return ($"CornerBirth_{ x }_{ y }", initial, 1, expected);
            yield return ($"CornerBlock_{ x }_{ y }", new(expected), 30, expected);
        }
    }

    /// <summary>
    ///   Supplies blinkers clipped by each coordinate-space edge.
    /// </summary>
    ///
    /// <returns>The edge scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetEdgeScenarios()
    {
        // A blinker on an edge cannot produce its outward birth.  The two remaining cells
        // then die rather than continuing the normal oscillation.

        foreach (var edge in new[] { long.MinValue, long.MaxValue })
        {
            var inward = edge == long.MinValue ? 1L : -1L;
            var vertical = new HashSet<Coordinate>
            {
                new(edge, -1),
                new(edge, 0),
                new(edge, 1)
            };

            var horizontal = new HashSet<Coordinate>
            {
                new(-1, edge),
                new(0, edge),
                new(1, edge)
            };

            yield return ($"VerticalEdge_{ edge }_1", new(vertical), 1, [ new(edge, 0), new(edge + inward, 0) ]);
            yield return ($"HorizontalEdge_{ edge }_1", new(horizontal), 1, [ new(0, edge), new(0, edge + inward) ]);
            yield return ($"VerticalEdge_{ edge }_2", new(vertical), 2, []);
            yield return ($"HorizontalEdge_{ edge }_2", new(horizontal), 2, []);
            yield return ($"VerticalEdge_{ edge }_30", new(vertical), 30, []);
            yield return ($"HorizontalEdge_{ edge }_30", new(horizontal), 30, []);
        }
    }

    /// <summary>
    ///   Supplies a glider approaching each signed limit and settling after an outward birth is clipped.
    /// </summary>
    ///
    /// <returns>The moving-boundary scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetGliderBoundaryScenarios()
    {
        const int BoundaryOffset = 3;

        var glider = CreatePattern(".#.", "..#", "###");

        // In these local diagrams, x=3 is the last representable column.  The glider first
        // reaches it at generation 3.  At generation 7, a birth at x=4 is unavailable.
        // The remaining cells then settle into a block rather than continuing as a glider.
        // These later snapshots are explicit because clipping an unrestricted glider's final
        // position would ignore how the missing birth changes subsequent generations.

        var touching = CreatePattern("....", ".#..", "..##", ".##.");
        var firstCycle = CreatePattern("....", "..#.", "...#", ".###");
        var clipped = CreatePattern("....", "....", "..#.", "...#", "..##");
        var settling = CreatePattern("....", "....", "....", "...#", "..##");
        var block = CreatePattern("....", "....", "....", "..##", "..##");
        var observations = new (int Generations, HashSet<Coordinate> Cells)[]
        {
            (3, touching),
            (4, firstCycle),
            (7, clipped),
            (8, settling),
            (9, block),
            (30, block)
        };
        var edges = new (string Name, long X, long Y, int DirectionX, int DirectionY, bool Transpose)[]
        {
            ("MaximumX", long.MaxValue - BoundaryOffset, 0, 1, 1, false),
            ("MinimumX", long.MinValue + BoundaryOffset, 0, -1, 1, false),
            ("MaximumY", 0, long.MaxValue - BoundaryOffset, 1, 1, true),
            ("MinimumY", 0, long.MinValue + BoundaryOffset, 1, -1, true)
        };

        foreach (var (name, x, y, directionX, directionY, transpose) in edges)
        {
            var orientedInitial = transpose ? glider.Select(cell => new Coordinate(cell.Y, cell.X)) : glider;

            foreach (var (generations, cells) in observations)
            {
                var orientedExpected = transpose ? cells.Select(cell => new Coordinate(cell.Y, cell.X)) : cells;

                yield return ($"GliderReaches{ name }_{ generations }",
                    Transform(orientedInitial, x, y, directionX, directionY),
                    generations,
                    Transform(orientedExpected, x, y, directionX, directionY));
            }
        }
    }

    /// <summary>
    ///   Supplies populations on opposite boundaries that must not become neighbors.
    /// </summary>
    ///
    /// <returns>The non-wrapping scenarios.</returns>
    ///
    private static IEnumerable<(string Name, HashSet<Coordinate> Initial, int Generations, HashSet<Coordinate> Expected)> GetNonWrappingScenarios()
    {
        // These sparse populations would interact if overflowing coordinates wrapped to the
        // opposite limit.  In the actual coordinate space, they are too far apart to sustain life.

        yield return ("NoHorizontalWrap", [ new(long.MinValue, 0), new(long.MaxValue, -1), new(long.MaxValue, 1) ], 1, []);
        yield return ("NoVerticalWrap", [ new(0, long.MinValue), new(-1, long.MaxValue), new(1, long.MaxValue) ], 1, []);
        yield return ("NoCornerWrap", [ new(long.MinValue, long.MinValue), new(long.MaxValue, long.MinValue), new(long.MinValue, long.MaxValue) ], 1, []);

        // Stable blocks at opposite corners must both survive without sharing neighbor counts.

        var block = CreatePattern("##", "##");
        var lowerBlock = Transform(block, long.MinValue, long.MinValue);
        var upperBlock = Transform(block, long.MaxValue - 1, long.MaxValue - 1);

        yield return ("DistantBoundaryPopulations", [ ..lowerBlock, ..upperBlock ], 30, [ ..lowerBlock, ..upperBlock ]);
    }

    /// <summary>
    ///   Creates coordinates from a diagram, with columns increasing x and rows increasing y.
    /// </summary>
    ///
    /// <param name="rows">Rows containing '#' for a live cell and '.' for a dead cell.</param>
    ///
    /// <returns>The coordinates of the live cells.</returns>
    ///
    private static HashSet<Coordinate> CreatePattern(params string[] rows)
    {
        var cells = new HashSet<Coordinate>();

        // Literal diagrams make the expected shape visible without deriving it through simulation
        // logic.  Column and row indices become x and y, with the first character at (0, 0).

        for (var y = 0; y < rows.Length; ++y)
        {
            for (var x = 0; x < rows[y].Length; ++x)
            {
                if (rows[y][x] == '#')
                {
                    // Store only live cells directly in the final set.  Dots preserve diagram
                    // spacing but must not become entries in the sparse population.

                    cells.Add(new Coordinate(x, y));
                }
                else if (rows[y][x] != '.')
                {
                    // Reject diagram typos rather than silently changing the expected population.

                    throw new ArgumentException("Pattern rows must contain only '#' and '.'.", nameof(rows));
                }
            }
        }

        return cells;
    }

    /// <summary>
    ///   Translates and optionally reflects a pattern without losing coordinate precision.
    /// </summary>
    ///
    /// <param name="cells">The pattern to transform.</param>
    /// <param name="x">The x origin.</param>
    /// <param name="y">The y origin.</param>
    /// <param name="directionX">The direction of the x axis.</param>
    /// <param name="directionY">The direction of the y axis.</param>
    ///
    /// <returns>The transformed coordinates.</returns>
    ///
    private static HashSet<Coordinate> Transform(IEnumerable<Coordinate> cells,
                                                 long x,
                                                 long y,
                                                 int directionX = 1,
                                                 int directionY = 1)
    {
        var transformed = new HashSet<Coordinate>();

        foreach (var cell in cells)
        {
            transformed.Add(new Coordinate(checked(x + (directionX * cell.X)), checked(y + (directionY * cell.Y))));
        }

        return transformed;
    }
}
