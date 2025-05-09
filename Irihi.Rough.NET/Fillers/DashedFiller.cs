using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET.Fillers;

public class DashedFiller(IRoughRenderer helper) : IPatternFiller
{
    public OpSet FillPolygons(List<List<PointF>> polygonList, ResolvedOptions options)
    {
        var lines = RoughHelpers.PolygonHachureLines(polygonList, options);
        return new OpSet { Type = OpSetType.FillSketch, Ops = DashedLine(lines, options) };
    }

    private List<Op> DashedLine(List<HuskaLine> lines, ResolvedOptions o)
    {
        var offset = o.DashOffset < 0 ? o.HachureGap < 0 ? o.StrokeWidth * 4 : o.HachureGap : o.DashOffset;
        var gap = o.DashGap < 0 ? o.HachureGap < 0 ? o.StrokeWidth * 4 : o.HachureGap : o.DashGap;
        List<Op> ops = [];
        foreach (var line in lines)
        {
            var length = line.Length;
            var count = Math.Floor(length / (offset + gap));
            var startOffset = (length + gap - count * (offset + gap)) / 2;
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
                var lstart = i * (offset + gap);
                var lend = lstart + offset;
                var start = PointFHelper.Create(p1.X + lstart * Math.Cos(alpha) + startOffset * Math.Cos(alpha),
                    p1.Y + lstart * Math.Sin(alpha) + startOffset * Math.Sin(alpha));
                var end = PointFHelper.Create(p1.X + lend * Math.Cos(alpha) + startOffset * Math.Cos(alpha),
                    p1.Y + lend * Math.Sin(alpha) + startOffset * Math.Sin(alpha));
                ops.AddRange(helper.DoubleLineOps(start.X, start.Y, end.X, end.Y, o));
            }
        }

        return ops;
    }
}