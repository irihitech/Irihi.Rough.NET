using System.Drawing;

namespace Irihi.Rough.NET.DataModels;

/// <summary>
///  Represents the result of an ellipse drawing operation.
/// </summary>
internal class EllipseResult
{
    public OpSet OpSet { get; set; }
    public List<PointF> Points { get; set; } = [];
}

/// <summary>
///  Represents the parameters for drawing an ellipse.
/// </summary>
public struct EllipseParams
{
    public double Rx { get; set; }
    public double Ry { get; set; }
    public double Increment { get; set; }
}