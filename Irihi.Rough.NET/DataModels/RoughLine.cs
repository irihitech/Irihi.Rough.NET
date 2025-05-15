using System.Drawing;

namespace Irihi.Rough.NET.DataModels;

/// <summary>
///  Represents a line segment defined by two points.
/// </summary>
/// <param name="Start"></param>
/// <param name="End"></param>
public readonly record struct RoughLine(PointF Start, PointF End)
{
    public double Length => Math.Sqrt(Math.Pow(End.X - Start.X, 2) + Math.Pow(End.Y - Start.Y, 2));
}