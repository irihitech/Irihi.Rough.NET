using System.Drawing;
using Irihi.Rough.NET.DataModels;

namespace Irihi.Rough.NET.Fillers;

/// <summary>
///  Fills polygons with Cross-hatching.
/// </summary>
/// <param name="helper"></param>
public class HatchFiller(IRoughRenderer helper) : HachureFiller(helper)
{
    /// <inheritdoc/>
    public override OpSet FillPolygons(List<List<PointF>> polygonList, ResolvedOptions options)
    {
        var set = FillPolygonsInternal(polygonList, options);
        var o2 = options with { };
        o2.HachureAngle += 90;
        var set2 = FillPolygonsInternal(polygonList, o2);
        set.Ops.AddRange(set2.Ops);
        return set;
    }
}