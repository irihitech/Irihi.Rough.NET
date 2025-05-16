using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET;

public static class RoughConfiguration
{
    private static readonly ResolvedOptions DefaultOptions = new()
    {
        MaxRandomnessOffset = 2,
        Roughness = 1,
        Bowing = 1,
        Stroke = Color.Transparent,
        StrokeWidth = 1,
        CurveTightness = 0,
        CurveFitting = 0.95,
        CurveStepCount = 9,
        FillStyle = FillStyle.Solid,
        FillWeight = -1,
        HachureAngle = -41,
        HachureGap = -1,
        DashOffset = -1,
        DashGap = -1,
        ZigzagOffset = -1,
        Seed = MathHelper.RandomSeed(),
        Randomizer = new Random(DateTime.Now.Millisecond),
        DisableMultiStroke = false,
        DisableMultiStrokeFill = false,
        PreserveVertices = false,
        FillShapeRoughnessGain = 0.8,
    };

    private static Options? _options;

    public static Options? Options
    {
        get => _options;
        internal set
        {
            _options = value;
            _actualOptions = DefaultOptions.Merge(value);
        }
    }

    private static ResolvedOptions _actualOptions = DefaultOptions;

    public static RoughGenerator CreateGenerator()
    {
        var seed = MathHelper.RandomSeed();
        return new RoughGenerator(_actualOptions with { Seed = seed, Randomizer = new Random(seed) });
    }
}