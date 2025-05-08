using System.Drawing;

namespace Irihi.Rough.NET.DataModels;

public record ResolvedOptions : Options
{
    public new double MaxRandomnessOffset { get; set; }
    public new double Roughness { get; set; }
    public new double Bowing { get; set; }
    public new Color Stroke { get; set; } = Color.Transparent;
    public new double StrokeWidth { get; set; }
    public new double CurveFitting { get; set; }
    public new double CurveTightness { get; set; }
    public new double CurveStepCount { get; set; }
    public new FillStyle FillStyle { get; set; }
    public new double FillWeight { get; set; }
    public new double HachureAngle { get; set; }
    public new double HachureGap { get; set; }
    public new double DashOffset { get; set; }
    public new double DashGap { get; set; }
    public new double ZigzagOffset { get; set; }
    public new int Seed { get; set; }
    public Random? Randomizer { get; set; }
    public new bool DisableMultiStroke { get; set; }
    public new bool DisableMultiStrokeFill { get; set; }
    public new bool PreserveVertices { get; set; }
    public new double FillShapeRoughnessGain { get; set; }

    public ResolvedOptions Merge(Options? options)
    {
        var result = this with { };
        if (options is null) return result;
        result.MaxRandomnessOffset = options.MaxRandomnessOffset ?? result.MaxRandomnessOffset;
        result.Roughness = options.Roughness ?? result.Roughness;
        result.Bowing = options.Bowing ?? result.Bowing;
        result.Stroke = options.Stroke ?? result.Stroke;
        result.StrokeWidth = options.StrokeWidth ?? result.StrokeWidth;
        result.CurveFitting = options.CurveFitting ?? result.CurveFitting;
        result.CurveTightness = options.CurveTightness ?? result.CurveTightness;
        result.CurveStepCount = options.CurveStepCount ?? result.CurveStepCount;
        result.FillStyle = options.FillStyle ?? result.FillStyle;
        result.FillWeight = options.FillWeight ?? result.FillWeight;
        result.HachureAngle = options.HachureAngle ?? result.HachureAngle;
        result.HachureGap = options.HachureGap ?? result.HachureGap;
        result.Simplification = options.Simplification ?? result.Simplification;
        result.DashOffset = options.DashOffset ?? result.DashOffset;
        result.DashGap = options.DashGap ?? result.DashGap;
        result.ZigzagOffset = options.ZigzagOffset ?? result.ZigzagOffset;
        result.Seed = options.Seed ?? result.Seed;
        result.Randomizer = new Random(result.Seed);
        result.DisableMultiStroke = options.DisableMultiStroke ?? result.DisableMultiStroke;
        result.DisableMultiStrokeFill = options.DisableMultiStrokeFill ?? result.DisableMultiStrokeFill;
        result.PreserveVertices = options.PreserveVertices ?? result.PreserveVertices;
        result.FillShapeRoughnessGain = options.FillShapeRoughnessGain ?? result.FillShapeRoughnessGain;
        result.StrokeLineDash = options.StrokeLineDash ?? result.StrokeLineDash;
        result.StrokeLineDashOffset = options.StrokeLineDashOffset ?? result.StrokeLineDashOffset;
        result.FillLineDash = options.FillLineDash ?? result.FillLineDash;
        result.FillLineDashOffset = options.FillLineDashOffset ?? result.FillLineDashOffset;
        result.FixedDecimalPlaceDigits = options.FixedDecimalPlaceDigits ?? result.FixedDecimalPlaceDigits;
        result.Fill = options.Fill ?? result.Fill;
        return result;
    }
}