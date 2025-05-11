using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Dependencies.HachureFill;
using Irihi.Rough.NET.Fillers;

namespace Irihi.Rough.NET.Helpers;

public static class RoughHelpers
{
    private static readonly Dictionary<FillStyle, IPatternFiller> Fillers = new(12);

    /// <summary>
    ///  Gets the appropriate pattern filler based on the fill style and renderer.
    /// </summary>
    /// <param name="options">
    /// <param name="renderer"></param>
    /// <returns></returns>
    public static IPatternFiller GetFiller(ResolvedOptions options, IRoughRenderer renderer)
    {
        if (Fillers.TryGetValue(options.FillStyle, out var filler)) return filler;
        Fillers[options.FillStyle] = options.FillStyle switch
        {
            FillStyle.Hachure => new HachureFiller(renderer),
            FillStyle.Zigzag => new ZigZagFiller(renderer),
            FillStyle.Dashed => new DashedFiller(renderer),
            FillStyle.Dots => new DotFiller(renderer),
            FillStyle.CrossHatch => new HatchFiller(renderer),
            FillStyle.ZigzagLine => new ZigZagLineFiller(renderer),
            _ => new HachureFiller(renderer)
        };
        return Fillers[options.FillStyle];
    }
    
    internal static List<HuskaLine> PolygonHachureLines(List<List<PointF>> polygonList, ResolvedOptions o)
    {
        var angle = o.HachureAngle + 90;
        var gap = o.HachureGap;
        if (gap < 0) gap = o.StrokeWidth * 4;

        gap = Math.Round(Math.Max(gap, 0.1));

        double? skipOffset = 1.0;
        if (o.Roughness >= 1)
        {
            // TODO Use accurate randomizer in project. 
            var random = MathHelper.Random(o);
            if (random > 0.7) skipOffset = gap;
        }

        return HachureFillFunctions.HachureLines(polygonList, gap, angle, skipOffset ?? 1);
    }
}