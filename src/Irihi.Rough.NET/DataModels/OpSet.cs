using System.Drawing;
using System.Text;

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

    public void GenerateGeometry(double? fixedDecimals = null)
    {
        var sb = new StringBuilder();
        foreach (var item in Ops)
        {
            var data = fixedDecimals >= 0
                ? item.Data.Select(d => d.ToString($"F{fixedDecimals}")).ToList()
                : item.Data.Select(d => d.ToString()).ToList();
            switch (item.op)
            {
                case OpType.Move:
                    sb.Append($"M{data[0]} {data[1]} ");
                    break;
                case OpType.BCurveTo:
                    sb.Append($"C{data[0]} {data[1]}, {data[2]} {data[3]}, {data[4]} {data[5]} ");
                    break;
                case OpType.LineTo:
                    sb.Append($"L{data[0]} {data[1]} ");
                    break;
            }
        }

        Geometry = sb.ToString().Trim();
    }
}