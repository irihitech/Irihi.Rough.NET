namespace Irihi.Rough.NET.DataModels;

/// <summary>
///  Represents the different types of operations that can be performed.
/// </summary>
public enum OpSetType
{
    // Path should consume Stroke as Stroke
    Path,

    // FillPath should consume Stroke as Stroke, Fill as Fill
    FillPath,

    // FillSketch should consume Fill as Stroke
    FillSketch
}