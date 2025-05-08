using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET;

public class RoughRenderer : IRoughRenderer
{
    public static RoughRenderer Instance { get; } = new RoughRenderer();

    public double RandOffset(double x, ResolvedOptions options)
    {
        return RendererHelper.RandOffset(x, options);
    }

    public double RandOffsetWithRange(double min, double max, ResolvedOptions options)
    {
        return RendererHelper.RandOffsetWithRange(min, max, options);
    }

    public OpSet Ellipse(double x, double y, double width, double height, ResolvedOptions options)
    {
        return RendererHelper.Ellipse(x, y, width, height, options);
    }

    public List<Op> DoubleLineOps(double x1, double y1, double x2, double y2, ResolvedOptions options)
    {
        return RendererHelper.DoubleLineFillOps(x1, y1, x2, y2, options);
    }
}