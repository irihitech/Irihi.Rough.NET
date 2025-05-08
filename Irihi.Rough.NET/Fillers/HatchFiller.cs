using System.Drawing;
using Irihi.Rough.NET.DataModels;

namespace Irihi.Rough.NET.Fillers;

public class HatchFiller(IRoughRenderer helper) : HachureFiller(helper)
{
    public override OpSet FillPolygons(List<List<Point>> polygonList, ResolvedOptions options)
    {
        var set = FillPolygonsInternal(polygonList, options);
        var o2 = options with { };
        o2.HachureAngle += 90;
        var set2 = FillPolygonsInternal(polygonList, o2);
        set.Ops.AddRange(set2.Ops);
        return set;
    }
}