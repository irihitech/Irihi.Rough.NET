using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET.Fillers;

/// <summary>
///  Fill the polygon with dashed lines.
/// </summary>
/// <param name="helper"></param>
public class DashedFiller(IRoughRenderer helper) : IPatternFiller
{
    /// <inheritdoc/>
    public OpSet FillPolygons(List<List<PointF>> polygonList, ResolvedOptions options)
    {
        var lines = RoughHelpers.PolygonHachureLines(polygonList, options);
        return new OpSet { Type = OpSetType.FillSketch, Ops = DashedLine(lines, options) };
    }

    private List<Op> DashedLine(List<RoughLine> lines, ResolvedOptions o)
    {
        var offset = o.DashOffset < 0 ? o.HachureGap < 0 ? o.StrokeWidth * 4 : o.HachureGap : o.DashOffset;
        var gap = o.DashGap < 0 ? o.HachureGap < 0 ? o.StrokeWidth * 4 : o.HachureGap : o.DashGap;
        var step = offset + gap;
        List<Op> ops = [];
        foreach (var line in lines)
        {
            var length = line.Length;
            var count = Math.Floor(length / step);
            var startOffset = (length + gap - count * step) / 2;
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
            var startOffsetCos = startOffset * cosAlpha;
            var startOffsetSin = startOffset * sinAlpha;
            for (var i = 0; i < count; i++)
            {
                var lstart = i * step;
                var lend = lstart + offset;
                var start = PointFHelper.Create(p1.X + lstart * cosAlpha + startOffsetCos,
                    p1.Y + lstart * sinAlpha + startOffsetSin);
                var end = PointFHelper.Create(p1.X + lend * cosAlpha + startOffsetCos,
                    p1.Y + lend * sinAlpha + startOffsetSin);
                ops.AddRange(helper.DoubleLineOps(start.X, start.Y, end.X, end.Y, o));
            }
        }

        return ops;
    }
}