namespace Irihi.Rough.NET.DataModels;

public class Drawable
{
    public DrawableShape Shape { get; set; }
    public ResolvedOptions? Options { get; init; }
    public List<OpSet>? Sets { get; init; }
}