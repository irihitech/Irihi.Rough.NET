using Irihi.Rough.NET.DataModels;

namespace Irihi.Rough.NET.Helpers;

public static class MathHelper
{
    public static int RandomSeed()
    {
        return System.Random.Shared.Next();
    }

    public static double Random()
    {
        return System.Random.Shared.NextDouble();
    }

    public static double Random(ResolvedOptions options)
    {
        return options.Randomizer?.NextDouble() ?? new Random(options.Seed).NextDouble();
    }

    public static bool AlmostEqual(double a, double b)
    {
        return Math.Abs(a - b) <= double.Epsilon;
    }

    public static bool NotEqual(double a, double b)
    {
        return Math.Abs(a - b) > double.Epsilon;
    }
}