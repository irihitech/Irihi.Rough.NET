using System.Drawing;
using System.Runtime.CompilerServices;

namespace Irihi.Rough.NET.Helpers;

public static class PointFHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static PointF Clone(this PointF p)
    {
        return new PointF(p.X, p.Y);
    }

    internal static PointF Create(double x, double y)
    {
        return new PointF((float)x, (float)y);
    }

    internal static double Distance(PointF p1, PointF p2)
    {
        return Math.Sqrt(DistanceSquared(p1, p2));
    }

    internal static double DistanceSquared(PointF p1, PointF p2)
    {
        var dx = p2.X - p1.X;
        var dy = p2.Y - p1.Y;
        return dx * dx + dy * dy;
    }
}