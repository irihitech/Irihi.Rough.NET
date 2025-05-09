using System.Drawing;
using System.Text;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Dependencies.CurveToBezier;
using Irihi.Rough.NET.Dependencies.PointsOnPath;
using Irihi.Rough.NET.Helpers;

namespace Irihi.Rough.NET;

public class RoughGenerator
{
    private readonly ResolvedOptions _defaultOptions;

    public RoughGenerator(ResolvedOptions defaultOptions)
    {
        _defaultOptions = defaultOptions;
    }

    private ResolvedOptions _o(Options? options)
    {
        return _defaultOptions.Merge(options);
    }

    private Drawable _d(DrawableShape shape, List<OpSet>? sets, ResolvedOptions? options)
    {
        return new Drawable
        {
            Shape = shape,
            Sets = sets ?? [],
            Options = options ?? _defaultOptions
        };
    }

    public Drawable Line(double x1, double y1, double x2, double y2, Options? options)
    {
        var o = _o(options);
        return _d(DrawableShape.Line, [RendererHelper.Line(x1, y1, x2, y2, o)], o);
    }

    public Drawable Rectangle(double x, double y, double width, double height, Options? options)
    {
        var o = _o(options);
        var paths = new List<OpSet>();
        var outline = RendererHelper.Rectangle(x, y, width, height, o);
        if (o.Fill is not null && !o.Fill.Value.IsTransparent())
        {
            var points =
                new List<PointF> { PointFHelper.Create(x, y), PointFHelper.Create(x + width, y), PointFHelper.Create(x + width, y + height), PointFHelper.Create(x, y + height) };
            if (o.FillStyle == FillStyle.Solid)
                paths.Add(RendererHelper.SolidFillPolygon([points], o));
            else
                paths.Add(RendererHelper.PatternFillPolygons([points], o));
        }

        if (!o.Stroke.IsTransparent()) paths.Add(outline);
        return _d(DrawableShape.Rectangle, paths, o);
    }

    public Drawable Ellipse(double x, double y, double width, double height, Options? options)
    {
        var o = _o(options);
        var paths = new List<OpSet>();
        var ellipseParams = RendererHelper.GenerateEllipseParams(width, height, o);
        var ellipseResponse = RendererHelper.EllipseWithParams(x, y, o, ellipseParams);
        if (o.Fill is not null && !o.Fill.Value.IsTransparent())
        {
            if (o.FillStyle == FillStyle.Solid)
            {
                var shape = RendererHelper.EllipseWithParams(x, y, o, ellipseParams).OpSet;
                shape.Type = OpSetType.FillPath;
                paths.Add(shape);
            }
            else
            {
                paths.Add(RendererHelper.PatternFillPolygons([ellipseResponse.Points], o));
            }
        }

        if (!o.Stroke.IsTransparent()) paths.Add(ellipseResponse.OpSet);
        return _d(DrawableShape.Ellipse, paths, o);
    }

    public Drawable Circle(double x, double y, double diameter, Options? options)
    {
        var result = Ellipse(x, y, diameter, diameter, options);
        result.Shape = DrawableShape.Circle;
        return result;
    }

    public Drawable LinearPath(List<PointF> points, Options? options)
    {
        var o = _o(options);
        return _d(DrawableShape.LinearPath, [RendererHelper.LinearPath(points, false, o)], o);
    }

    public Drawable Arc(double x, double y, double width, double height, double start, double stop, bool closed,
        Options? options)
    {
        var o = _o(options);
        var paths = new List<OpSet>();
        var outline = RendererHelper.Arc(x, y, width, height, start, stop, closed, true, o);
        if (closed && o.Fill is not null && !o.Fill.Value.IsTransparent())
        {
            if (o.FillStyle == FillStyle.Solid)
            {
                var fillOptions = o with { };
                fillOptions.DisableMultiStroke = true;
                var shape = RendererHelper.Arc(x, y, width, height, start, stop, true, false, fillOptions);
                shape.Type = OpSetType.FillPath;
                paths.Add(shape);
            }
            else
            {
                paths.Add(RendererHelper.PatternFillArc(x, y, width, height, start, stop, o));
            }
        }

        if (!o.Stroke.IsTransparent()) paths.Add(outline);
        return _d(DrawableShape.Arc, paths, o);
    }

    public Drawable Curve(List<List<PointF>> points, Options? options)
    {
        var o = _o(options);
        var paths = new List<OpSet>();
        var outline = RendererHelper.Curve(points, o);
        if (o.Fill is not null && !o.Fill.Value.IsTransparent())
        {
            if (o.FillStyle == FillStyle.Solid)
            {
                var newOptions = o with { };
                newOptions.Roughness += o.FillShapeRoughnessGain;
                newOptions.DisableMultiStroke = true;
                var fillShape = RendererHelper.Curve(points, newOptions);
                paths.Add(new OpSet
                {
                    Type = OpSetType.FillPath,
                    Ops = MergeShape(fillShape.Ops)
                });
            }
            else
            {
                var polyPoints = new List<PointF>();
                var inputPoints = points;
                if (inputPoints.Count != 0)
                {
                    var p1 = inputPoints[0];
                    var pointsList = inputPoints;
                    foreach (var ps in pointsList)
                    {
                        if (points.Count < 3)
                            polyPoints.AddRange(ps);
                        else if (points.Count == 3)
                            polyPoints.AddRange(
                                CurveToBezierFunctions.PointsOnBezierCurves(
                                    CurveToBezierFunctions.CurveToBezier([ps[0], ps[0], ps[1], ps[2]]),
                                    10, (1 + o.Roughness) / 2));
                        else
                            polyPoints.AddRange(CurveToBezierFunctions.PointsOnBezierCurves(
                                CurveToBezierFunctions.CurveToBezier(ps), 10,
                                (1 + o.Roughness) / 2));
                    }
                }

                if (polyPoints.Count != 0) paths.Add(RendererHelper.PatternFillPolygons([polyPoints], o));
            }
        }

        if (!o.Stroke.IsTransparent()) paths.Add(outline);
        return _d(DrawableShape.Curve, paths, o);
    }

    public Drawable Polygon(List<PointF> points, Options options)
    {
        var o = _o(options);
        var paths = new List<OpSet>();
        var outline = RendererHelper.LinearPath(points, true, o);
        if (o.Fill is not null && !o.Fill.Value.IsTransparent())
        {
            if (o.FillStyle == FillStyle.Solid)
                paths.Add(RendererHelper.SolidFillPolygon([points], o));
            else
                paths.Add(RendererHelper.PatternFillPolygons([points], o));
        }

        if (!o.Stroke.IsTransparent()) paths.Add(outline);
        return _d(DrawableShape.Polygon, paths, o);
    }

    public Drawable Path(string? d, Options? options)
    {
        var o = _o(options);
        var paths = new List<OpSet>();
        if (string.IsNullOrWhiteSpace(d)) return _d(DrawableShape.Path, paths, o);

        d = d.Replace("/\n/g", " ").Replace(@"/(-\s)/g", "-").Replace(@"/(\s\s)/g", " ");

        var hasFill = o.Fill is not null && !o.Fill.Value.IsTransparent();
        var hasStroke = !o.Stroke.IsTransparent();
        var simplified = o.Simplification != null && o.Simplification < 1;
        var distance = simplified ? 4 - 4 * (o.Simplification ?? 1) : (1 + o.Roughness) / 2;
        var sets = PointsOnPathFunctions.PointsOnPath(d, 1, distance);
        var shape = RendererHelper.SvgPath(d, o);

        if (hasFill)
        {
            if (o.FillStyle == FillStyle.Solid)
            {
                if (sets.Count == 1)
                {
                    var newOptions = o with { };
                    newOptions.DisableMultiStroke = true;
                    newOptions.Roughness += o.FillShapeRoughnessGain;
                    var fillShape = RendererHelper.SvgPath(d, newOptions);
                    paths.Add(new OpSet
                    {
                        Type = OpSetType.FillPath,
                        Ops = MergeShape(fillShape.Ops)
                    });
                }
                else
                {
                    paths.Add(RendererHelper.SolidFillPolygon(sets, o));
                }
            }
            else
            {
                paths.Add(RendererHelper.PatternFillPolygons(sets, o));
            }
        }

        if (hasStroke)
        {
            if (simplified)
                sets.ForEach(set => { paths.Add(RendererHelper.LinearPath(set, false, o)); });
            else
                paths.Add(shape);
        }

        return _d(DrawableShape.Path, paths, o);
    }

    [Obsolete]
    public string OpsToPath(OpSet drawing, double? fixedDecimals = null)
    {
        var sb = new StringBuilder();
        foreach (var item in drawing.Ops)
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

        return sb.ToString().Trim();
    }

    [Obsolete("Try not to serialize to string. ")]
    public List<PathInfo> ToPaths(Drawable drawable)
    {
        var sets = drawable.Sets ?? [];
        var o = drawable.Options ?? _defaultOptions;
        var paths = new List<PathInfo>();
        foreach (var drawing in sets)
        {
            PathInfo? path = null;
            switch (drawing.Type)
            {
                case OpSetType.Path:
                    path = new PathInfo
                    {
                        D = OpsToPath(drawing),
                        Stroke = o.Stroke,
                        StrokeWidth = o.StrokeWidth,
                        Fill = null
                    };
                    break;
                case OpSetType.FillPath:
                    path = new PathInfo
                    {
                        D = OpsToPath(drawing),
                        Stroke = null,
                        StrokeWidth = 0,
                        Fill = o.Fill
                    };
                    break;
                case OpSetType.FillSketch:
                    path = FillSketch(drawing, o);
                    break;
            }

            if (path != null) paths.Add(path);
        }

        return paths;
    }

    [Obsolete]
    private PathInfo FillSketch(OpSet drawing, ResolvedOptions o)
    {
        var fweight = o.FillWeight;
        if (fweight < 0) fweight = o.StrokeWidth / 2;
        return new PathInfo
        {
            D = OpsToPath(drawing),
            Stroke = o.Fill,
            StrokeWidth = fweight,
            Fill = null
        };
    }

    private List<Op> MergeShape(List<Op> input)
    {
        return input.Where((o, i) =>
        {
            if (i == 0) return true;
            if (o.op == OpType.Move) return false;
            return true;
        }).ToList();
    }
}