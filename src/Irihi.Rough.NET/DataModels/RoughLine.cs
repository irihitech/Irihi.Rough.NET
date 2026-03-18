using System.Drawing;

namespace Irihi.Rough.NET.DataModels;

/// <summary>
///  Represents a line segment defined by two points.
/// </summary>
/// <param name="Start"></param>
/// <param name="End"></param>
public readonly record struct RoughLine(PointF Start, PointF End)
{
    public double Length
    {
        get
        {
            var dx = End.X - Start.X;
            var dy = End.Y - Start.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}