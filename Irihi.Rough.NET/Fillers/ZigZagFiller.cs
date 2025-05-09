using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET.Fillers;

public class ZigZagFiller(IRoughRenderer helper) : HachureFiller(helper)
{
    public override OpSet FillPolygons(List<List<PointF>> polygonList, ResolvedOptions o)
    {
        var gap = o.HachureGap;
        if (gap < 0) gap = o.StrokeWidth * 4;
        gap = Math.Max(gap, 0.1);
        var o2 = o with { };
        o2.HachureGap = gap;
        var lines = RoughHelpers.PolygonHachureLines(polygonList, o2);
        var zigZagAngle = Math.PI / 180 * o.HachureAngle;
        List<HuskaLine> zigzagLines = [];
        var dgx = gap * 0.5 * Math.Cos(zigZagAngle);
        var dgy = gap * 0.5 * Math.Sin(zigZagAngle);
        foreach (var line in lines)
        {
            var p1 = line.Start;
            var p2 = line.End;
            if (line.Length != 0)
            {
                zigzagLines.Add(new HuskaLine
                {
                    Start = PointFHelper.Create(p1.X - dgx, p1.Y + dgy),
                    End = p2
                });
                zigzagLines.Add(new HuskaLine
                {
                    Start = PointFHelper.Create(p1.X + dgx, p1.Y - dgy),
                    End = p2
                });
            }
        }

        var ops = RenderLines(zigzagLines, o);
        return new OpSet { Type = OpSetType.FillSketch, Ops = ops };
    }
}