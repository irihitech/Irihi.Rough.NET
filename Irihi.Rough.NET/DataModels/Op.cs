namespace Irihi.Rough.NET.DataModels;

/// <summary>
///  Represents a drawing operation with a specific type and associated data.
/// </summary>
public class Op
{
    public OpType op { get; init; }
    public List<double> Data { get; init; } = [];
}