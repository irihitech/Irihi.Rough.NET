using System.Drawing;

namespace Irihi.Rough.NET.DataModels;

internal class EllipseResult
{
    public OpSet OpSet { get; set; }
    public List<PointF> Points { get; set; } = [];
}

public struct EllipseParams
{
    public double Rx { get; set; }
    public double Ry { get; set; }
    public double Increment { get; set; }
}