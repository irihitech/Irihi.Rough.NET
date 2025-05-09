using System.Drawing;
using Irihi.Rough.NET.Dependencies.CurveToBezier;
using Irihi.Rough.NET.Dependencies.PathDataParser;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET.Dependencies.PointsOnPath;

public class PointsOnPathFunctions
{
    public static List<List<PointF>> PointsOnPath(string path, double? tolerance, double? distance)
    {
        var segments = PathDataParserFunctions.ParsePath(path);
        var normalized = segments.Absolutize().Normalize();

        List<List<PointF>> sets = [];
        List<PointF> currentPoints = [];
        var start = new PointF(0, 0);
        List<PointF> pendingCurve = [];

        foreach (var (key, data) in normalized)
        {
            switch (key)
            {
                case 'M':
                    AppendPendingPoints();
                    start = PointFHelper.Create(data[0], data[1]);
                    currentPoints.Add(start);
                    break;
                case 'L':
                    AppendPendingCurve();
                    currentPoints.Add(PointFHelper.Create(data[0], data[1]));
                    break;
                case 'C':
                    if (pendingCurve.Count == 0)
                    {
                        var lastPoint = currentPoints.Count != 0 ? currentPoints[^1] : start;
                        pendingCurve.Add(lastPoint.Clone());
                    }

                    pendingCurve.Add(PointFHelper.Create(data[0], data[1]));
                    pendingCurve.Add(PointFHelper.Create(data[2], data[3]));
                    pendingCurve.Add(PointFHelper.Create(data[4], data[5]));
                    break;
                case 'Z':
                    AppendPendingCurve();
                    currentPoints.Add(start.Clone());
                    break;
            }
        }

        AppendPendingPoints();

        if (distance != 0) return sets;

        List<List<PointF>> result = [];
        foreach (var set in sets)
        {
            var simplifiedSet = CurveToBezierFunctions.Simplify(set, distance ?? 1);
            if (simplifiedSet.Count != 0) result.Add(simplifiedSet);
        }

        return result;

        void AppendPendingPoints()
        {
            AppendPendingCurve();
            if (currentPoints.Count == 0) return;
            sets.Add(currentPoints);
            currentPoints = [];
        }

        void AppendPendingCurve()
        {
            if (pendingCurve.Count >= 4)
                currentPoints.AddRange(CurveToBezierFunctions.PointsOnBezierCurves(pendingCurve, tolerance));

            pendingCurve = [];
        }
    }
}