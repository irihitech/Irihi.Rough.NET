using System.Drawing;
using System.Runtime.CompilerServices;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET.Dependencies.CurveToBezier;

public static class CurveToBezierFunctions
{
    public static List<PointF> CurveToBezier(IReadOnlyList<PointF> pointsIn, double curveTightness = 0)
    {
        var len = pointsIn.Count;
        if (len < 3) throw new ArgumentException("A curve must have at least three points.");

        List<PointF> output = [];

        if (len == 3)
        {
            output.Add(pointsIn[0]);
            output.Add(pointsIn[1]);
            output.Add(pointsIn[2]);
            output.Add(pointsIn[2]);
        }
        else
        {
            List<PointF> points =
            [
                pointsIn[0],
                pointsIn[0]
            ];

            for (var i = 1; i < pointsIn.Count; i++)
            {
                points.Add(pointsIn[i]);
                if (i == pointsIn.Count - 1) points.Add(pointsIn[i]);
            }

            Span<PointF> b = stackalloc PointF[4];
            var s = 1 - curveTightness;
            output.Add(points[0].Clone());

            for (var i = 1; i + 2 < points.Count; i++)
            {
                var cachedVert = points[i];
                b[0] = new PointF(cachedVert.X, cachedVert.Y);
                b[1] = PointFHelper.Create(
                    cachedVert.X + (s * points[i + 1].X - s * points[i - 1].X) / 6,
                    cachedVert.Y + (s * points[i + 1].Y - s * points[i - 1].Y) / 6
                );
                b[2] = PointFHelper.Create(
                    points[i + 1].X + (s * points[i].X - s * points[i + 2].X) / 6,
                    points[i + 1].Y + (s * points[i].Y - s * points[i + 2].Y) / 6
                );
                b[3] = new PointF(points[i + 1].X, points[i + 1].Y);

                output.Add(b[1]);
                output.Add(b[2]);
                output.Add(b[3]);
            }
        }

        return output;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static PointF Lerp(PointF a, PointF b, double t)
    {
        return PointFHelper.Create(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
    }

    internal static double DistanceToSegmentSquared(PointF p, PointF v, PointF w)
    {
        var l2 = PointFHelper.DistanceSquared(v, w);
        if (l2 == 0) return PointFHelper.DistanceSquared(p, v);

        var t = ((p.X - v.X) * (w.X - v.X) + (p.Y - v.Y) * (w.Y - v.Y)) / l2;
        t = Math.Max(0, Math.Min(1, t));
        // t = Math.Clamp(t, 0, 1);
        return PointFHelper.DistanceSquared(p, Lerp(v, w, t));
    }

    // Adapted from https://seant23.wordpress.com/2010/11/12/offset-bezier-curves/
    internal static double Flatness(IReadOnlyList<PointF> points, int offset)
    {
        var p1 = points[offset + 0];
        var p2 = points[offset + 1];
        var p3 = points[offset + 2];
        var p4 = points[offset + 3];

        var ux = 3 * p2.X - 2 * p1.X - p4.X;
        ux *= ux;
        var uy = 3 * p2.Y - 2 * p1.Y - p4.Y;
        uy *= uy;

        var vx = 3 * p3.X - 2 * p4.X - p1.X;
        vx *= vx;
        var vy = 3 * p3.Y - 2 * p4.Y - p1.Y;
        vy *= vy;

        if (ux < vx) ux = vx;

        if (uy < vy) uy = vy;

        return ux + uy;
    }

    internal static List<PointF> GetPointsOnBezierCurveWithSplitting(IReadOnlyList<PointF> points, int offset,
        double tolerance, List<PointF>? newPoints = null)
    {
        var outPoints = newPoints ?? [];
        if (Flatness(points, offset) < tolerance)
        {
            var p0 = points[offset + 0];
            if (outPoints.Count > 0)
            {
                var d = PointFHelper.Distance(outPoints[^1], p0);
                if (d > 1) outPoints.Add(p0);
            }
            else
            {
                outPoints.Add(p0);
            }

            outPoints.Add(points[offset + 3]);
        }
        else
        {
            const double t = 0.5;
            var p1 = points[offset + 0];
            var p2 = points[offset + 1];
            var p3 = points[offset + 2];
            var p4 = points[offset + 3];

            var q1 = Lerp(p1, p2, t);
            var q2 = Lerp(p2, p3, t);
            var q3 = Lerp(p3, p4, t);

            var r1 = Lerp(q1, q2, t);
            var r2 = Lerp(q2, q3, t);

            var red = Lerp(r1, r2, t);

            GetPointsOnBezierCurveWithSplitting(new[] { p1, q1, r1, red }, 0, tolerance, outPoints);
            GetPointsOnBezierCurveWithSplitting(new[] { red, r2, q3, p4 }, 0, tolerance, outPoints);
        }

        return outPoints;
    }

    public static List<PointF> SimplifyPoints(IReadOnlyList<PointF> points, int start, int end, double epsilon,
        List<PointF>? newPoints = null)
    {
        var outPoints = newPoints ?? new List<PointF>();

        // find the most distance point from the endpoints
        var s = points[start];
        var e = points[end - 1];
        var maxDistSq = 0d;
        var maxNdx = 1;

        for (var i = start + 1; i < end - 1; ++i)
        {
            var distSq = DistanceToSegmentSquared(points[i], s, e);
            if (distSq > maxDistSq)
            {
                maxDistSq = distSq;
                maxNdx = i;
            }
        }

        // if that point is too far, split
        if (Math.Sqrt(maxDistSq) > epsilon)
        {
            SimplifyPoints(points, start, maxNdx + 1, epsilon, outPoints);
            SimplifyPoints(points, maxNdx, end, epsilon, outPoints);
        }
        else
        {
            if (outPoints.Count == 0) outPoints.Add(s);
            outPoints.Add(e);
        }

        return outPoints;
    }

    public static List<PointF> Simplify(IReadOnlyList<PointF> points, double distance)
    {
        return SimplifyPoints(points, 0, points.Count, distance);
    }

    public static List<PointF> PointsOnBezierCurves(IReadOnlyList<PointF> points, double? tolerance = 0.15,
        double? distance = null)
    {
        var newPoints = new List<PointF>();
        var numSegments = (points.Count - 1) / 3;

        for (var i = 0; i < numSegments; i++)
        {
            var offset = i * 3;
            GetPointsOnBezierCurveWithSplitting(points, offset, tolerance ?? 0.15, newPoints);
        }

        if (distance is > 0) return SimplifyPoints(newPoints, 0, newPoints.Count, distance.Value);

        return newPoints;
    }
}