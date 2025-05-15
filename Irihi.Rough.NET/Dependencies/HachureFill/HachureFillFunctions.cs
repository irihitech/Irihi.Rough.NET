using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET.Dependencies.HachureFill;

public static class HachureFillFunctions
{
    internal static void RotatePoints(IList<PointF> points, PointF center, double degree)
    {
        var (cx, cy) = (center.X, center.Y);
        var angle = Math.PI / 180.0 * degree;
        var cos = Math.Cos(angle);
        var sin = Math.Sin(angle);
        for (var index = 0; index < points.Count; index++)
        {
            var p = points[index];
            var (x, y) = (p.X, p.Y);
            points[index] = PointFHelper.Create((x - cx) * cos - (y - cy) * sin + cx, (x - cx) * sin + (y - cy) * cos + cy);
        }
    }
    
    internal static void RotateLines(IList<RoughLine> lines, PointF center, double degree)
    {
        var (cx, cy) = (center.X, center.Y);
        var angle = Math.PI / 180.0 * degree;
        var cos = Math.Cos(angle);
        var sin = Math.Sin(angle);
        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var (x1, y1) = (line.Start.X, line.Start.Y);
            var (x2, y2) = (line.End.X, line.End.Y);
            lines[i] = new RoughLine(
                PointFHelper.Create((x1 - cx) * cos - (y1 - cy) * sin + cx, (x1 - cx) * sin + (y1 - cy) * cos + cy),
                PointFHelper.Create((x2 - cx) * cos - (y2 - cy) * sin + cx, (x2 - cx) * sin + (y2 - cy) * cos + cy)
            );
        }
    }

    private static List<RoughLine> StraightHachureLines(IList<List<PointF>> polygons, double gap,
        double hachureStepOffset)
    {
        List<List<PointF>> vertexArray = [];
        foreach (var polygon in polygons)
        {
            List<PointF> vertices = [..polygon];
            if (vertices[0] != vertices[^1]) vertices.Add(vertices[0]);
            if (vertices.Count > 2) vertexArray.Add(vertices);
        }

        List<RoughLine> lines = [];
        gap = Math.Max(gap, 0.1);

        List<EdgeEntry> edges = [];

        foreach (var vertices in vertexArray)
        {
            for (var i = 0; i < vertices.Count - 1; i++)
            {
                var p1 = vertices[i];
                var p2 = vertices[i + 1];
                if (p1.Y != p2.Y)
                {
                    var yMin = Math.Min(p1.Y, p2.Y);
                    edges.Add(new EdgeEntry
                    {
                        YMin = yMin,
                        YMax = Math.Max(p1.Y, p2.Y),
                        X = yMin == p1.Y ? p1.X : p2.X,
                        IsLope = (p2.X - p1.X) / (p2.Y - p1.Y)
                    });
                }
            }
        }

        edges.Sort((e1, e2) =>
        {
            if (e1.YMin < e2.YMin) return -1;
            if (e1.YMin > e2.YMin) return 1;
            if (e1.X < e2.X) return -1;
            if (e1.X > e2.X) return 1;
            if (e1.YMax == e2.YMax) return 0;
            var result = e1.YMax - e2.YMax;
            if (result == 0) return 0;
            return result < 0 ? -1 : 1;
        });

        if (edges.Count == 0) return lines;

        // Start Scanning
        List<ActiveEdgeEntry> activeEdges = [];
        var y = edges[0].YMin;
        var iteration = 0;
        while (activeEdges.Count > 0 || edges.Count > 0)
        {
            if (edges.Count > 0)
            {
                var ix = -1;
                for (var i = 0; i < edges.Count; i++)
                {
                    if (edges[i].YMin > y) break;
                    ix = i;
                }

                var removed = edges[..(ix + 1)].ToList();
                activeEdges.AddRange(removed.Select(edge => new ActiveEdgeEntry { S = y, Edge = edge }));
                edges.RemoveRange(0, ix + 1);
            }

            activeEdges = activeEdges.Where(a => a.Edge.YMax > y).ToList();

            activeEdges.Sort((ae1, ae2) =>
            {
                if (ae1.Edge.X == ae2.Edge.X) return 0;
                var result = ae1.Edge.X - ae2.Edge.X;
                if (result == 0) return 0;
                return result < 0 ? -1 : 1;
            });

            // Fill between the edges
            if (hachureStepOffset != 1 || iteration % gap == 0)
                if (activeEdges.Count > 1)
                    for (var i = 0; i < activeEdges.Count; i = i + 2)
                    {
                        var next = i + 1;
                        if (next >= activeEdges.Count) break;
                        var ce = activeEdges[i].Edge;
                        var ne = activeEdges[next].Edge;
                        lines.Add(new RoughLine(PointFHelper.Create(Math.Round(ce.X), y), PointFHelper.Create(Math.Round(ne.X), y)));
                    }

            y += hachureStepOffset;
            for (var i = 0; i < activeEdges.Count; i++)
            {
                var ae = activeEdges[i];
                ae.Edge.X += hachureStepOffset * ae.Edge.IsLope;
                activeEdges[i] = ae;
            }

            iteration++;
        }

        return lines;
    }

    internal static List<RoughLine> HachureLines(IList<List<PointF>> polygons, double hachureGap,
        double hachureAngle,
        double hachureStepOffset = 1)
    {
        var angle = hachureAngle;
        var gap = Math.Max(hachureGap, 0.1);
        var rotationCenter = new PointF(0, 0);
        if (angle != 0)
            foreach (var polygon in polygons)
                RotatePoints(polygon, rotationCenter, angle);

        var lines = StraightHachureLines(polygons, gap, hachureStepOffset);
        if (angle != 0)
        {
            foreach (var pol in polygons)
                RotatePoints(pol, rotationCenter, -angle);
            RotateLines(lines, rotationCenter, -angle);
        }

        return lines;
    }
}