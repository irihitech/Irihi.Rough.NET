namespace Irihi.Rough.NET.DataModels;

public enum OpSetType
{
    // Path should consume Stroke as Stroke
    Path,

    // FillPath should consume Stroke as Stroke, Fill as Fill
    FillPath,

    // FillSketch should consume Fill as Stroke
    FillSketch
}