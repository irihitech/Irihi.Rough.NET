using System.Drawing;

namespace Irihi.Rough.NET.DataModels;

public record Options
{
    public double? MaxRandomnessOffset { get; set; }
    public double? Roughness { get; set; }
    public double? Bowing { get; set; }
    public Color? Stroke { get; set; }
    public double? StrokeWidth { get; set; }
    public double? CurveFitting { get; set; }
    public double? CurveTightness { get; set; }
    public double? CurveStepCount { get; set; }
    public Color? Fill { get; set; }
    public FillStyle? FillStyle { get; set; }
    public double? FillWeight { get; set; }
    public double? HachureAngle { get; set; }
    public double? HachureGap { get; set; }
    public double? Simplification { get; set; }
    public double? DashOffset { get; set; }
    public double? DashGap { get; set; }
    public double? ZigzagOffset { get; set; }
    public int? Seed { get; set; }
    public double[]? StrokeLineDash { get; set; }
    public double? StrokeLineDashOffset { get; set; }
    public double[]? FillLineDash { get; set; }
    public double? FillLineDashOffset { get; set; }
    public bool? DisableMultiStroke { get; set; }
    public bool? DisableMultiStrokeFill { get; set; }
    public bool? PreserveVertices { get; set; }
    public int? FixedDecimalPlaceDigits { get; set; }
    public double? FillShapeRoughnessGain { get; set; }
}