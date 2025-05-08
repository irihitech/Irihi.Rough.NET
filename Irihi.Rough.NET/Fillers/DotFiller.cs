using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET.Fillers;

public class DotFiller(IRoughRenderer helper) : IPatternFiller
{
    public OpSet FillPolygons(List<List<Point>> polygonList, ResolvedOptions options)
    {
        var o = options with { };
        o.HachureAngle = 0;
        var lines = RoughHelpers.PolygonHachureLines(polygonList, o);
        return DotsOnLines(lines, o);
    }

    private OpSet DotsOnLines(List<HuskaLine> lines, ResolvedOptions o)
    {
        var ops = new List<Op>();
        var gap = o.HachureGap;
        if (gap < 0)
        {
            gap = o.StrokeWidth * 4;
        }

        gap = Math.Max(gap, 0.1);
        var fweight = o.FillWeight;
        if (fweight < 0)
        {
            fweight = o.StrokeWidth / 2;
        }

        var ro = gap / 4;
        foreach (var line in lines)
        {
            var length = line.Length;
            var dl = length / gap;
            var count = Math.Ceiling(dl) - 1;
            var offset = length - (count * gap);
            var x = ((line.Start.X + line.End.X) / 2) - (gap / 4);
            var minY = Math.Min(line.Start.Y, line.End.Y);

            for (var i = 0; i < count; i++)
            {
                var y = minY + offset + (i * gap);
                var cx = (x - ro) + MathHelper.Random() * 2 * ro;
                var cy = (y - ro) + MathHelper.Random() * 2 * ro;
                var el = helper.Ellipse(cx, cy, fweight, fweight, o);
                ops.AddRange(el.Ops);
            }
        }

        return new OpSet() { Type = OpSetType.FillSketch, Ops = ops };
    }
}