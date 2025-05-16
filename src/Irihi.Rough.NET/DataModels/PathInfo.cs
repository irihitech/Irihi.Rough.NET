using System.Drawing;

namespace Irihi.Rough.NET.DataModels;

/// <summary>
///  Represents the information of a path.
/// </summary>
public class PathInfo
{
    public string D { get; set; } = string.Empty;
    public Color? Stroke { get; set; }
    public double StrokeWidth { get; set; }
    public Color? Fill { get; set; }
}