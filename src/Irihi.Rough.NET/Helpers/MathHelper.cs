using Irihi.Rough.NET.DataModels;

namespace Irihi.Rough.NET.Helpers;

public static class MathHelper
{
    /// <summary>
    ///  Get a random seed.
    /// </summary>
    /// <returns></returns>
    public static int RandomSeed()
    {
        return System.Random.Shared.Next();
    }

    /// <summary>
    ///  Get a random number between 0 and 1.
    /// </summary>
    /// <returns></returns>
    public static double Random()
    {
        return System.Random.Shared.NextDouble();
    }

    /// <summary>
    ///  Get a random number between 0 and 1 from resolved options.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static double Random(ResolvedOptions options)
    {
        return options.Randomizer?.NextDouble() ?? new Random(options.Seed).NextDouble();
    }

    /// <summary>
    ///  Check if two double values are almost equal.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool AlmostEqual(double a, double b)
    {
        return Math.Abs(a - b) <= double.Epsilon;
    }

    /// <summary>
    ///  Check if two double values are not quite equal.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool NotEqual(double a, double b)
    {
        return Math.Abs(a - b) > double.Epsilon;
    }
}