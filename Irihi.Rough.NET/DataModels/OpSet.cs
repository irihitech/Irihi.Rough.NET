using System.Drawing;

namespace Irihi.Rough.NET.DataModels;

public class OpSet
{
    public OpSetType Type { get; set; }
    public List<Op> Ops { get; init; } = [];
    public Size? Size { get; set; }
    public StreamGeometry? Geometry { get; set; }

    internal void GenerateGeometry()
    {
        StreamGeometry geometry = new StreamGeometry();
        bool isFilled = Type == OpSetType.FillPath;
        using var context = geometry.Open();
        if (isFilled) context.SetFillRule(FillRule.NonZero);
        foreach (var op in Ops)
        {
            if (op.op == OpType.Move)
            {
                context.BeginFigure(new Point(op.Data[0], op.Data[1]), isFilled);
            }
            else if (op.op == OpType.LineTo)
            {
                context.LineTo(new Point(op.Data[0], op.Data[1]));
            }
            else if (op.op == OpType.BCurveTo)
            {
                context.CubicBezierTo(new Point(op.Data[0], op.Data[1]),
                    new Point(op.Data[2], op.Data[3]),
                    new Point(op.Data[4], op.Data[5]));
            }
        }

        if (isFilled)
        {
            context.EndFigure(isFilled);
        }

        Geometry = geometry;
        Size = geometry.Bounds.Size;
    }
}