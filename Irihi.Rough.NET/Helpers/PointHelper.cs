using System.Drawing;
using System.Runtime.CompilerServices;

namespace Irihi.Rough.NET.Helpers;

public static class PointHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static Point Clone(this Point p)
    {
        return new Point(p.X, p.Y);
    }

    internal static double Distance(Point p1, Point p2)
    {
        return Math.Sqrt(DistanceSquared(p1, p2));
    }

    internal static double DistanceSquared(Point p1, Point p2)
    {
        var dx = p2.X - p1.X;
        var dy = p2.Y - p1.Y;
        return dx * dx + dy * dy;
    }
}