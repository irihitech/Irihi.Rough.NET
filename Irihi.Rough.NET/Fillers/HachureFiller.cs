using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET.Fillers;

public class HachureFiller(IRoughRenderer helper) : IPatternFiller
{
    public virtual OpSet FillPolygons(List<List<PointF>> polygonList, ResolvedOptions options)
    {
        return FillPolygonsInternal(polygonList, options);
    }

    protected OpSet FillPolygonsInternal(List<List<PointF>> polygonList, ResolvedOptions options)
    {
        var lines = RoughHelpers.PolygonHachureLines(polygonList, options);
        var ops = RenderLines(lines, options);
        return new OpSet { Type = OpSetType.FillSketch, Ops = ops };
    }

    protected List<Op> RenderLines(List<HuskaLine> lines, ResolvedOptions options)
    {
        var ops = new List<Op>();
        foreach (var line in lines)
        {
            var t = helper.DoubleLineOps(line.Start.X, line.Start.Y, line.End.X, line.End.Y, options);
            ops.AddRange(t);
        }

        return ops;
    }
}