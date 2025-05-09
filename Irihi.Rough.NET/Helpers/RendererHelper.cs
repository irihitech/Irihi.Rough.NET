using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Dependencies.PathDataParser;

namespace Irihi.Rough.NET.Helpers;

public static class RendererHelper
{
    public static OpSet Line(double x1, double y1, double x2, double y2, ResolvedOptions options)
    {
        return new OpSet { Type = OpSetType.Path, Ops = DoubleLine(x1, y1, x2, y2, options) };
    }

    private static List<Op> DoubleLine(double x1, double y1, double x2, double y2, ResolvedOptions o,
        bool filling = false)
    {
        var singleStroke = filling ? o.DisableMultiStrokeFill : o.DisableMultiStroke;
        var o1 = Line(x1, y1, x2, y2, o, true, false);
        if (singleStroke) return o1;
        var o2 = Line(x1, y1, x2, y2, o, true, true);
        return o1.Concat(o2).ToList();
    }

    public static List<Op> DoubleLineFillOps(double x1, double y1, double x2, double y2, ResolvedOptions options)
    {
        return DoubleLine(x1, y1, x2, y2, options);
    }

    private static List<Op> Line(double x1, double y1, double x2, double y2, ResolvedOptions o, bool move, bool overlay)
    {
        var lengthSq = Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2);
        var length = Math.Sqrt(lengthSq);
        double roughnessGain;
        if (length < 200)
            roughnessGain = 1;
        else if (length > 500)
            roughnessGain = 0.4;
        else
            roughnessGain = -0.0016668 * length + 1.233334;

        var offset = o.MaxRandomnessOffset;
        if (offset * offset * 100 > lengthSq) offset = length / 10;
        var halfOffset = offset / 2;
        var divergePoint = 0.2 + MathHelper.Random(o) * 0.2;
        var midDispX = o.Bowing * o.MaxRandomnessOffset * (y2 - y1) / 200;
        var midDispY = o.Bowing * o.MaxRandomnessOffset * (x1 - x2) / 200;
        midDispX = OffsetOpt(midDispX, o, roughnessGain);
        midDispY = OffsetOpt(midDispY, o, roughnessGain);
        List<Op> ops = [];
        var preserveVertices = o.PreserveVertices;
        if (move)
        {
            if (overlay)
                ops.Add(
                    new Op
                    {
                        op = OpType.Move,
                        Data =
                        [
                            x1 + (preserveVertices ? 0 : RandomHalf()),
                            y1 + (preserveVertices ? 0 : RandomHalf())
                        ]
                    });
            else
                ops.Add(new Op
                {
                    op = OpType.Move, Data =
                    [
                        x1 + (preserveVertices ? 0 : OffsetOpt(offset, o, roughnessGain)),
                        y1 + (preserveVertices ? 0 : OffsetOpt(offset, o, roughnessGain))
                    ]
                });
        }

        if (overlay)
            ops.Add(new Op
            {
                op = OpType.BCurveTo,
                Data =
                [
                    midDispX + x1 + (x2 - x1) * divergePoint + RandomHalf(),
                    midDispY + y1 + (y2 - y1) * divergePoint + RandomHalf(),
                    midDispX + x1 + 2 * (x2 - x1) * divergePoint + RandomHalf(),
                    midDispY + y1 + 2 * (y2 - y1) * divergePoint + RandomHalf(),
                    x2 + (preserveVertices ? 0 : RandomHalf()),
                    y2 + (preserveVertices ? 0 : RandomHalf())
                ]
            });
        else
            ops.Add(new Op
            {
                op = OpType.BCurveTo,
                Data =
                [
                    midDispX + x1 + (x2 - x1) * divergePoint + RandomFull(),
                    midDispY + y1 + (y2 - y1) * divergePoint + RandomFull(),
                    midDispX + x1 + 2 * (x2 - x1) * divergePoint + RandomFull(),
                    midDispY + y1 + 2 * (y2 - y1) * divergePoint + RandomFull(),
                    x2 + (preserveVertices ? 0 : RandomFull()),
                    y2 + (preserveVertices ? 0 : RandomFull())
                ]
            });
        return ops;

        double RandomFull()
        {
            return OffsetOpt(offset, o, roughnessGain);
        }

        double RandomHalf()
        {
            return OffsetOpt(halfOffset, o, roughnessGain);
        }
    }

    private static double OffsetOpt(double x, ResolvedOptions ops, double roughnessGain = 1)
    {
        return Offset(-x, x, ops, roughnessGain);
    }

    private static double Offset(double min, double max, ResolvedOptions ops, double roughnessGain = 1)
    {
        return ops.Roughness * roughnessGain * (MathHelper.Random(ops) * (max - min) + min);
    }

    public static OpSet LinearPath(List<PointF> points, bool close, ResolvedOptions o)
    {
        var len = points.Count;
        if (len > 2)
        {
            List<Op> ops = [];
            for (var i = 0; i < len - 1; i++)
                ops.AddRange(DoubleLine(points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y, o));
            if (close) ops.AddRange(DoubleLine(points[len - 1].X, points[len - 1].Y, points[0].X, points[0].Y, o));

            return new OpSet { Type = OpSetType.Path, Ops = ops };
        }

        if (len == 2) return Line(points[0].X, points[0].Y, points[1].X, points[1].Y, o);

        return new OpSet { Type = OpSetType.Path, Ops = [] };
    }

    public static OpSet Polygon(List<PointF> points, ResolvedOptions o)
    {
        return LinearPath(points, true, o);
    }

    public static OpSet Rectangle(double x, double y, double width, double height, ResolvedOptions o)
    {
        List<PointF> points =
        [
            PointFHelper.Create(x, y),
            PointFHelper.Create(x + width, y),
            PointFHelper.Create(x + width, y + height),
            PointFHelper.Create(x, y + height)
        ];
        return Polygon(points, o);
    }

    public static OpSet Curve(List<List<PointF>> inputPoints, ResolvedOptions o)
    {
        if (inputPoints.Count != 0)
        {
            var pointsList = inputPoints;
            var o1 = CurveWithOffset(pointsList[0], 1 * (1 + o.Roughness * 0.2), o);
            var o2 = o.DisableMultiStroke
                ? []
                : CurveWithOffset(pointsList[0], 1.5 * (1 + o.Roughness * 0.22), CloneOptionsAlterSeed(o));

            for (var i = 1; i < pointsList.Count; i++)
            {
                var points = pointsList[i];
                if (points.Count != 0)
                {
                    var underlay = CurveWithOffset(points, 1 * (1 + o.Roughness * 0.2), o);
                    var overlay = o.DisableMultiStroke
                        ? []
                        : CurveWithOffset(points, 1.5 * (1 + o.Roughness * 0.22), CloneOptionsAlterSeed(o));
                    foreach (var item in underlay)
                    {
                        if (item.op != OpType.Move)
                            o1.Add(item);
                    }

                    foreach (var item in overlay)
                    {
                        if (item.op != OpType.Move)
                            o2.Add(item);
                    }
                }
            }

            return new OpSet { Type = OpSetType.Path, Ops = [..o1, ..o2] };
        }

        return new OpSet { Type = OpSetType.Path, Ops = [] };
    }

    private static List<Op> CurveWithOffset(List<PointF> points, double offset, ResolvedOptions o)
    {
        if (points.Count == 0) return [];
        List<PointF> ps =
        [
            PointFHelper.Create(points[0].X + OffsetOpt(offset, o), points[0].Y + OffsetOpt(offset, o)),
            PointFHelper.Create(points[0].X + OffsetOpt(offset, o), points[0].Y + OffsetOpt(offset, o))
        ];
        for (var i = 1; i < points.Count; i++)
        {
            ps.Add(PointFHelper.Create(points[i].X + OffsetOpt(offset, o), points[i].Y + OffsetOpt(offset, o)));
            if (i == points.Count - 1)
                ps.Add(PointFHelper.Create(points[i].X + OffsetOpt(offset, o), points[i].Y + OffsetOpt(offset, o)));
        }

        return Curve(ps, null, o);
    }

    private static List<Op> Curve(List<PointF> points, PointF? closePoint, ResolvedOptions o)
    {
        var len = points.Count;
        List<Op> ops = [];
        if (len > 3)
        {
            List<PointF> b = [new(), new(), new(), new()];
            var s = 1 - o.CurveTightness;
            ops.Add(new Op { op = OpType.Move, Data = [points[1].X, points[1].Y] });
            for (var i = 1; i + 2 < len; i++)
            {
                var cachedVertArray = points[i];
                b[0] = PointFHelper.Create(cachedVertArray.X, cachedVertArray.Y);
                b[1] = PointFHelper.Create(cachedVertArray.X + (s * points[i + 1].X - s * points[i - 1].X) / 6,
                    cachedVertArray.Y + (s * points[i + 1].Y - s * points[i - 1].Y) / 6);
                b[2] = PointFHelper.Create(points[i + 1].X + (s * points[i].X - s * points[i + 2].X) / 6,
                    points[i + 1].Y + (s * points[i].Y - s * points[i + 2].Y) / 6);
                b[3] = PointFHelper.Create(points[i + 1].X, points[i + 1].Y);
                ops.Add(new Op { op = OpType.BCurveTo, Data = [b[1].X, b[1].Y, b[2].X, b[2].Y, b[3].X, b[3].Y] });
            }

            if (closePoint is not null)
            {
                var ro = o.MaxRandomnessOffset;
                ops.Add(new Op
                {
                    op = OpType.LineTo,
                    Data = [closePoint.Value.X + OffsetOpt(ro, o), closePoint.Value.Y + OffsetOpt(ro, o)]
                });
            }
        }
        else if (len == 3)
        {
            ops.Add(new Op { op = OpType.Move, Data = [points[1].X, points[1].Y] });
            ops.Add(new Op
            {
                op = OpType.BCurveTo,
                Data = [points[1].X, points[1].Y, points[2].X, points[2].Y, points[2].X, points[2].Y]
            });
        }
        else if (len == 2)
        {
            ops.AddRange(Line(points[0].X, points[0].Y, points[1].X, points[1].Y, o, true, true));
        }

        return ops;
    }

    private static ResolvedOptions CloneOptionsAlterSeed(ResolvedOptions o)
    {
        var opt = o with { };
        opt.Randomizer = null;
        opt.Seed = o.Seed + 1;
        opt.Randomizer = new Random(opt.Seed);
        return opt;
    }

    public static OpSet Ellipse(double x, double y, double width, double height, ResolvedOptions o)
    {
        var @params = GenerateEllipseParams(width, height, o);
        return EllipseWithParams(x, y, o, @params).OpSet;
    }

    internal static EllipseParams GenerateEllipseParams(double width, double height, ResolvedOptions o)
    {
        var psq = Math.Sqrt(Math.PI * 2 * Math.Sqrt((Math.Pow(width / 2, 2) + Math.Pow(height / 2, 2)) / 2));
        var stepCount = Math.Ceiling(Math.Max(o.CurveStepCount, o.CurveStepCount / Math.Sqrt(200) * psq));
        var increment = Math.PI * 2 / stepCount;
        var rx = Math.Abs(width / 2);
        var ry = Math.Abs(height / 2);
        var curveFitRandomness = 1 - o.CurveFitting;
        rx += OffsetOpt(rx * curveFitRandomness, o);
        ry += OffsetOpt(ry * curveFitRandomness, o);
        return new EllipseParams { Rx = rx, Ry = ry, Increment = increment };
    }

    internal static EllipseResult EllipseWithParams(double x, double y, ResolvedOptions o, EllipseParams ellipseParams)
    {
        var lists = ComputeEllipsePoints(ellipseParams.Increment, x, y, ellipseParams.Rx, ellipseParams.Ry, 1,
            ellipseParams.Increment * Offset(0.1, Offset(0.4, 1, o), o), o);
        var ap1 = lists[0];
        var cp1 = lists[1];
        var o1 = Curve(ap1, null, o);
        if (!o.DisableMultiStroke && o.Roughness != 0)
        {
            var lists2 = ComputeEllipsePoints(ellipseParams.Increment, x, y, ellipseParams.Rx, ellipseParams.Ry, 1.5, 0,
                o);
            var ap2 = lists2[0];
            var o2 = Curve(ap2, null, o);
            o1 = o1.Concat(o2).ToList();
        }

        return new EllipseResult
        {
            OpSet = new OpSet { Type = OpSetType.Path, Ops = o1 },
            Points = cp1
        };
    }

    private static List<List<PointF>> ComputeEllipsePoints(double increment, double cx, double cy, double rx,
        double ry, double offset, double overlap, ResolvedOptions o)
    {
        var coreOnly = o.Roughness == 0;
        List<PointF> corePoints = [];
        List<PointF> allPoints = [];

        if (coreOnly)
        {
            increment = increment / 4;
            allPoints.Add(PointFHelper.Create(cx + rx * Math.Cos(-increment), cy + ry * Math.Sin(-increment)));
            for (var angle = 0.0; angle <= Math.PI * 2; angle += increment)
            {
                var p = PointFHelper.Create(cx + rx * Math.Cos(angle), cy + ry * Math.Sin(angle));
                corePoints.Add(p);
                allPoints.Add(p);
            }

            allPoints.Add(PointFHelper.Create(cx + rx * Math.Cos(0), cy + ry * Math.Sin(0)));
            allPoints.Add(PointFHelper.Create(cx + rx * Math.Cos(increment), cy + ry * Math.Sin(increment)));
        }
        else
        {
            var radOffset = OffsetOpt(0.5, o) - Math.PI / 2;
            allPoints.Add(PointFHelper.Create(OffsetOpt(offset, o) + cx + 0.9 * rx * Math.Cos(radOffset - increment),
                OffsetOpt(offset, o) + cy + 0.9 * ry * Math.Sin(radOffset - increment)));
            var endAngle = Math.PI * 2 + radOffset - 0.01;
            for (var angle = radOffset; angle < endAngle; angle = angle + increment)
            {
                var p = PointFHelper.Create(OffsetOpt(offset, o) + cx + rx * Math.Cos(angle),
                    OffsetOpt(offset, o) + cy + ry * Math.Sin(angle));
                corePoints.Add(p);
                allPoints.Add(p);
            }

            allPoints.Add(PointFHelper.Create(
                OffsetOpt(offset, o) + cx + rx * Math.Cos(radOffset + Math.PI * 2 + overlap * 0.5),
                OffsetOpt(offset, o) + cy + ry * Math.Sin(radOffset + Math.PI * 2 + overlap * 0.5)));
            allPoints.Add(PointFHelper.Create(OffsetOpt(offset, o) + cx + 0.98 * rx * Math.Cos(radOffset + overlap),
                OffsetOpt(offset, o) + cy + 0.98 * ry * Math.Sin(radOffset + overlap)));
            allPoints.Add(PointFHelper.Create(
                OffsetOpt(offset, o) + cx + 0.9 * rx * Math.Cos(radOffset + overlap * 0.5),
                OffsetOpt(offset, o) + cy + 0.9 * ry * Math.Sin(radOffset + overlap * 0.5)));
        }

        return [allPoints, corePoints];
    }

    public static OpSet Arc(double x, double y, double width, double height, double start, double stop,
        bool closed, bool roughClosure, ResolvedOptions o)
    {
        var cx = x;
        var cy = y;
        var rx = Math.Abs(width / 2);
        var ry = Math.Abs(height / 2);
        rx += OffsetOpt(rx * 0.01, o);
        ry += OffsetOpt(ry * 0.01, o);
        var strt = start;
        var stp = stop;
        while (strt < 0)
        {
            strt += Math.PI * 2;
            stp += Math.PI * 2;
        }

        if (stp - strt > Math.PI * 2)
        {
            strt = 0;
            stp = Math.PI * 2;
        }

        var ellipseInc = Math.PI * 2 / o.CurveStepCount;
        var arcInc = Math.Min(ellipseInc / 2, (stp - strt) / 2);
        var ops = Arc(arcInc, cx, cy, rx, ry, strt, stp, 1, o);
        if (!o.DisableMultiStroke)
        {
            var o2 = Arc(arcInc, cx, cy, rx, ry, strt, stp, 1.5, o);
            ops.AddRange(o2);
        }

        if (closed)
        {
            if (roughClosure)
            {
                ops.AddRange(DoubleLine(cx, cy, cx + rx * Math.Cos(strt), cy + ry * Math.Sin(strt), o));
                ops.AddRange(DoubleLine(cx, cy, cx + rx * Math.Cos(stp), cy + ry * Math.Sin(stp), o));
            }
            else
            {
                ops.Add(new Op
                {
                    op = OpType.LineTo,
                    Data = [cx, cy]
                });
                ops.Add(new Op
                {
                    op = OpType.LineTo,
                    Data = [cx + rx * Math.Cos(strt), cy + ry * Math.Sin(strt)]
                });
            }
        }

        return new OpSet { Type = OpSetType.Path, Ops = ops };
    }

    private static List<Op> Arc(double increment, double cx, double cy, double rx, double ry, double strt, double stp,
        double offset, ResolvedOptions o)
    {
        var radOffset = strt + OffsetOpt(0.1, o);
        List<PointF> points =
        [
            PointFHelper.Create(
                OffsetOpt(offset, o) + cx + 0.9 * rx * Math.Cos(radOffset - increment),
                OffsetOpt(offset, o) + cy + 0.9 * ry * Math.Sin(radOffset - increment))
        ];
        for (var angle = radOffset; angle <= stp; angle = angle + increment)
        {
            points.Add(PointFHelper.Create(
                OffsetOpt(offset, o) + cx + rx * Math.Cos(angle),
                OffsetOpt(offset, o) + cy + ry * Math.Sin(angle)));
        }

        points.Add(PointFHelper.Create(cx + rx * Math.Cos(stp), cy + ry * Math.Sin(stp)));
        points.Add(PointFHelper.Create(cx + rx * Math.Cos(stp), cy + ry * Math.Sin(stp)));
        return Curve(points, null, o);
    }

    public static OpSet SvgPath(string path, ResolvedOptions o)
    {
        var segments = PathDataParserFunctions.ParsePath(path).Absolutize().Normalize();
        List<Op> ops = [];
        var first = PointFHelper.Create(0, 0);
        var current = PointFHelper.Create(0, 0);
        foreach (var (key, data) in segments)
        {
            switch (key)
            {
                case 'M':
                {
                    current = PointFHelper.Create(data[0], data[1]);
                    first = PointFHelper.Create(data[0], data[1]);
                    break;
                }
                case 'L':
                    ops.AddRange(DoubleLine(current.X, current.Y, data[0], data[1], o));
                    current = PointFHelper.Create(data[0], data[1]);
                    break;
                case 'C':
                {
                    double x1 = data[0], y1 = data[1], x2 = data[2], y2 = data[3], x = data[4], y = data[5];
                    ops.AddRange(BezierTo(x1, y1, x2, y2, x, y, current, o));
                    current = PointFHelper.Create(x, y);
                    break;
                }
                case 'Z':
                    ops.AddRange(DoubleLine(current.X, current.Y, first.X, first.Y, o));
                    current = PointFHelper.Create(first.X, first.Y);
                    break;
            }
        }

        return new OpSet { Type = OpSetType.Path, Ops = ops };
    }

    private static List<Op> BezierTo(double x1, double y1, double x2, double y2, double x, double y, PointF current,
        ResolvedOptions o)
    {
        List<Op> ops = [];
        double[] ros = [o.MaxRandomnessOffset, o.MaxRandomnessOffset + 0.3];
        var iterations = o.DisableMultiStroke ? 1 : 2;
        var preserveVertices = o.PreserveVertices;
        for (var i = 0; i < iterations; i++)
        {
            if (i == 0)
                ops.Add(new Op { op = OpType.Move, Data = [current.X, current.Y] });
            else
                ops.Add(new Op
                {
                    op = OpType.Move,
                    Data =
                    [
                        current.X + (preserveVertices ? 0 : OffsetOpt(ros[0], o)),
                        current.Y + (preserveVertices ? 0 : OffsetOpt(ros[0], o))
                    ]
                });

            var f = preserveVertices
                ? PointFHelper.Create(x, y)
                : PointFHelper.Create(x + OffsetOpt(ros[i], o), y + OffsetOpt(ros[i], o));
            ops.Add(new Op
            {
                op = OpType.BCurveTo,
                Data =
                [
                    x1 + OffsetOpt(ros[i], o), y1 + OffsetOpt(ros[i], o), x2 + OffsetOpt(ros[i], o),
                    y2 + OffsetOpt(ros[i], o), f.X, f.Y
                ]
            });
        }

        return ops;
    }

    public static OpSet SolidFillPolygon(List<List<PointF>> polygonList, ResolvedOptions o)
    {
        List<Op> ops = [];
        foreach (var points in polygonList)
        {
            if (points.Count == 0) continue;
            var offset = o.MaxRandomnessOffset;
            var len = points.Count;
            if (len <= 2) continue;
            ops.Add(new Op
            {
                op = OpType.Move,
                Data = [points[0].X + OffsetOpt(offset, o), points[0].Y + OffsetOpt(offset, o)]
            });
            for (var i = 1; i < len; i++)
            {
                ops.Add(new Op
                {
                    op = OpType.LineTo,
                    Data = [points[i].X + OffsetOpt(offset, o), points[i].Y + OffsetOpt(offset, o)]
                });
            }
        }

        return new OpSet { Type = OpSetType.FillPath, Ops = ops };
    }

    public static OpSet PatternFillPolygons(List<List<PointF>> polygonList, ResolvedOptions o)
    {
        return RoughHelpers.GetFiller(o, RoughRenderer.Instance).FillPolygons(polygonList, o);
    }

    public static OpSet PatternFillArc(double x, double y, double width, double height, double start, double stop,
        ResolvedOptions o)
    {
        var cx = x;
        var cy = y;
        var rx = Math.Abs(width / 2);
        var ry = Math.Abs(height / 2);
        rx += OffsetOpt(rx * 0.01, o);
        ry += OffsetOpt(ry * 0.01, o);
        var strt = start;
        var stp = stop;
        while (strt < 0)
        {
            strt += Math.PI * 2;
            stp += Math.PI * 2;
        }

        if (stp - strt > Math.PI * 2)
        {
            strt = 0;
            stp = Math.PI * 2;
        }

        var increment = (stp - strt) / o.CurveStepCount;
        List<PointF> points = [];
        for (var angle = strt; angle <= stp; angle = angle + increment)
            points.Add(PointFHelper.Create(cx + rx * Math.Cos(angle), cy + ry * Math.Sin(angle)));
        points.Add(PointFHelper.Create(cx + rx * Math.Cos(stp), cy + ry * Math.Sin(stp)));
        points.Add(PointFHelper.Create(cx, cy));
        return PatternFillPolygons([points], o);
    }

    public static double RandOffset(double x, ResolvedOptions options)
    {
        return OffsetOpt(x, options);
    }

    public static double RandOffsetWithRange(double min, double max, ResolvedOptions options)
    {
        return Offset(min, max, options);
    }
}