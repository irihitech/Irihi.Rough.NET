using System.Drawing;

namespace Irihi.Rough.NET.DataModels;

public readonly record struct HuskaLine(Point Start, Point End)
{
    public double Length => Math.Sqrt(Math.Pow(End.X - Start.X, 2) + Math.Pow(End.Y - Start.Y, 2));
}