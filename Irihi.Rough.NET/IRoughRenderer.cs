using Irihi.Rough.NET.DataModels;

namespace Irihi.Rough.NET;

public interface IRoughRenderer
{
    public double RandOffset(double x, ResolvedOptions options);
    public double RandOffsetWithRange(double min, double max, ResolvedOptions options);
    public OpSet Ellipse(double x, double y, double width, double height, ResolvedOptions options);
    List<Op> DoubleLineOps(double x1, double y1, double x2, double y2, ResolvedOptions options);
}