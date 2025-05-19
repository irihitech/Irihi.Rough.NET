namespace Irihi.Rough.NET.DataModels;

/// <summary>
///  Represents a drawable object that stores drawing options and a list of operation sets.
/// </summary>
public class Drawable
{
    public DrawableShape Shape { get; set; }
    public ResolvedOptions? Options { get; init; }
    public List<OpSet> Sets { get; init; } = [];
}