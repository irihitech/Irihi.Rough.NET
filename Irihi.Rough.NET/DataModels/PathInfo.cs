using System.Drawing;

namespace Irihi.Rough.NET.DataModels;

public class PathInfo
{
    public string D { get; set; } = string.Empty;
    public Color? Stroke { get; set; }
    public double StrokeWidth { get; set; }
    public Color? Fill { get; set; }
}