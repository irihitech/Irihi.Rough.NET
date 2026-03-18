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
        var twoZo = 2 * zo;
        var dz = zo * Math.Sqrt(2);
        foreach (var line in lines)
        {
            var length = line.Length;
            var count = Math.Round(length / twoZo);
            var p1 = line.Start;
            var p2 = line.End;
            if (p1.X > p2.X)
            {
                p1 = line.End;
                p2 = line.Start;
            }

            var alpha = Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X));
            var cosAlpha = Math.Cos(alpha);
            var sinAlpha = Math.Sin(alpha);
            var cosAlphaPi4 = Math.Cos(alpha + Math.PI / 4);
            var sinAlphaPi4 = Math.Sin(alpha + Math.PI / 4);
            for (var i = 0; i < count; i++)
            {
                var lstart = i * twoZo;
                var lend = lstart + twoZo;
                var start = PointFHelper.Create(p1.X + lstart * cosAlpha, p1.Y + lstart * sinAlpha);
                var end = PointFHelper.Create(p1.X + lend * cosAlpha, p1.Y + lend * sinAlpha);
                var middle = PointFHelper.Create(start.X + dz * cosAlphaPi4,
                    start.Y + dz * sinAlphaPi4);
                ops.AddRange(helper.DoubleLineOps(start.X, start.Y, middle.X, middle.Y, o));
                ops.AddRange(helper.DoubleLineOps(middle.X, middle.Y, end.X, end.Y, o));
            }
        }
        return ops;
    }
}