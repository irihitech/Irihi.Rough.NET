namespace Irihi.Rough.NET.DataModels;

public class Op
{
    public OpType op { get; init; }
    public List<double> Data { get; init; } = [];
}