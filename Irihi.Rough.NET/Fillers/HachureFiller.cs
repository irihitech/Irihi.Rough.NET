using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET.Fillers;

/// <summary>
///  Fill the polygons with hachure lines.
/// </summary>
/// <param name="helper"></param>
public class HachureFiller(IRoughRenderer helper) : IPatternFiller
{
    /// <inheritdoc/>
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

    protected List<Op> RenderLines(List<RoughLine> lines, ResolvedOptions options)
    {
        List<Op> ops = [];
        foreach (var line in lines)
        {
            var t = helper.DoubleLineOps(line.Start.X, line.Start.Y, line.End.X, line.End.Y, options);
            ops.AddRange(t);
        }

        return ops;
    }
}