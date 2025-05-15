using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET.Fillers;

/// <summary>
///  Fills polygons with zigzag lines.
/// </summary>
/// <param name="helper"></param>
public class ZigZagLineFiller(IRoughRenderer helper) : IPatternFiller
{
    /// <inheritdoc/>
    public OpSet FillPolygons(List<List<PointF>> polygonList, ResolvedOptions o)
    {
        var gap = o.HachureGap < 0 ? o.StrokeWidth * 4 : o.HachureGap;
        var zo = o.ZigzagOffset < 0 ? gap : o.ZigzagOffset;
        var o2 = o with { };
        o2.HachureGap = gap + zo;
        var lines = RoughHelpers.PolygonHachureLines(polygonList, o);
        return new OpSet { Type = OpSetType.FillSketch, Ops = ZigZagLines(lines, zo, o) };
    }

    private List<Op> ZigZagLines(List<RoughLine> lines, double zo, ResolvedOptions o)
    {
        List<Op> ops = [];
        lines.ForEach(line =>
        {
            var length = line.Length;
            var count = Math.Round(length / (2 * zo));
            var p1 = line.Start;
            var p2 = line.End;
            if (p1.X > p2.X)
            {
                p1 = line.End;
                p2 = line.Start;
            }

            var alpha = Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X));
            for (var i = 0; i < count; i++)
            {
                var lstart = i * 2 * zo;
                var lend = (i + 1) * 2 * zo;
                var dz = Math.Sqrt(2 * Math.Pow(zo, 2));
                var start = PointFHelper.Create(p1.X + lstart * Math.Cos(alpha), p1.Y + lstart * Math.Sin(alpha));
                var end = PointFHelper.Create(p1.X + lend * Math.Cos(alpha), p1.Y + lend * Math.Sin(alpha));
                var middle = PointFHelper.Create(start.X + dz * Math.Cos(alpha + Math.PI / 4),
                    start.Y + dz * Math.Sin(alpha + Math.PI / 4));
                ops.AddRange(helper.DoubleLineOps(start.X, start.Y, middle.X, middle.Y, o));
                ops.AddRange(helper.DoubleLineOps(middle.X, middle.Y, end.X, end.Y, o));
            }
        });
        return ops;
    }
}