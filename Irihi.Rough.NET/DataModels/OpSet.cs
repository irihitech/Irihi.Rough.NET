using System.Drawing;

namespace Irihi.Rough.NET.DataModels;

/// <summary>
///  Represents a set of operations to be performed on a shape.
/// </summary>
public class OpSet
{
    public OpSetType Type { get; set; }
    public List<Op> Ops { get; init; } = [];
    public SizeF? Size { get; set; }
    public string? Geometry { get; set; }
}