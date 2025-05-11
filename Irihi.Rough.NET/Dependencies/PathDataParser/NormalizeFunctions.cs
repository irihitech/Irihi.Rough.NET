using System.Globalization;
using System.Runtime.CompilerServices;

namespace Irihi.Rough.NET.Dependencies.PathDataParser;

public static class NormalizeFunctions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double DegToRad(double degrees)
    {
        return Math.PI * degrees / 180.0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (double X, double Y) Rotate(double x, double y, double angleRad)
    {
        var localX = x * Math.Cos(angleRad) - y * Math.Sin(angleRad);
        var localY = x * Math.Sin(angleRad) + y * Math.Cos(angleRad);
        return (localX, localY);
    }

    /// <summary>
    ///  Converts an elliptical arc to cubic Bezier curves.
    /// </summary>
    /// <param name="x1"> The x-coordinate of the start point of the arc.</param>
    /// <param name="y1"> The y-coordinate of the start point of the arc.</param>
    /// <param name="x2"> The x-coordinate of the end point of the arc.</param>
    /// <param name="y2"> The y-coordinate of the end point of the arc.</param>
    /// <param name="r1"> The x-radius of the ellipse.</param>
    /// <param name="r2"> The y-radius of the ellipse.</param>
    /// <param name="angle"> The rotation angle of the ellipse in degrees.</param>
    /// <param name="largeArcFlag"> Indicates whether the arc is large (1) or small (0).</param>
    /// <param name="sweepFlag"> Indicates the direction of the arc (1 for positive-angle, 0 for negative-angle).</param>
    /// <param name="recursive"> Optional parameter for recursive calls.</param>
    /// <returns></returns>
    public static List<double[]> ArcToCubicCurves(
        double x1, double y1, double x2, double y2,
        double r1, double r2, double angle,
        int largeArcFlag, int sweepFlag,
        double[]? recursive = null)
    {
        var angleRad = DegToRad(angle);
        List<double[]> curves = [];

        double f1, f2, cx, cy;
        if (recursive != null)
        {
            (f1, f2, cx, cy) = (recursive[0], recursive[1], recursive[2], recursive[3]);
        }
        else
        {
            var (rx1, ry1) = Rotate(x1, y1, -angleRad);
            var (rx2, ry2) = Rotate(x2, y2, -angleRad);
            (x1, y1, x2, y2) = (rx1, ry1, rx2, ry2);

            var x = (x1 - x2) / 2;
            var y = (y1 - y2) / 2;
            var h = x * x / (r1 * r1) + y * y / (r2 * r2);
            if (h > 1)
            {
                h = Math.Sqrt(h);
                r1 *= h;
                r2 *= h;
            }

            var sign = largeArcFlag == sweepFlag ? -1 : 1;

            var r1Pow = r1 * r1;
            var r2Pow = r2 * r2;

            var left = r1Pow * r2Pow - r1Pow * y * y - r2Pow * x * x;
            var right = r1Pow * y * y + r2Pow * x * x;

            var k = sign * Math.Sqrt(Math.Abs(left / right));

            cx = k * r1 * y / r2 + (x1 + x2) / 2;
            cy = k * -r2 * x / r1 + (y1 + y2) / 2;

            f1 = Math.Asin(((y1 - cy) / r2).ToString("F9", CultureInfo.InvariantCulture).ToDouble());
            f2 = Math.Asin(((y2 - cy) / r2).ToString("F9", CultureInfo.InvariantCulture).ToDouble());

            if (x1 < cx) f1 = Math.PI - f1;
            if (x2 < cx) f2 = Math.PI - f2;

            if (f1 < 0) f1 = Math.PI * 2 + f1;
            if (f2 < 0) f2 = Math.PI * 2 + f2;

            if (sweepFlag == 1 && f1 > f2) f1 -= Math.PI * 2;
            if (sweepFlag == 0 && f2 > f1) f2 -= Math.PI * 2;
        }

        var df = f2 - f1;

        if (Math.Abs(df) > Math.PI * 120 / 180)
        {
            var f2old = f2;
            var x2old = x2;
            var y2old = y2;

            f2 = sweepFlag == 1 && f2 > f1
                ? f1 + Math.PI * 120 / 180
                : f1 - Math.PI * 120 / 180;

            x2 = cx + r1 * Math.Cos(f2);
            y2 = cy + r2 * Math.Sin(f2);

            curves = ArcToCubicCurves(x2, y2, x2old, y2old, r1, r2, angle, 0, sweepFlag, [f2, f2old, cx, cy]);
        }

        df = f2 - f1;

        var c1 = Math.Cos(f1);
        var s1 = Math.Sin(f1);
        var c2 = Math.Cos(f2);
        var s2 = Math.Sin(f2);
        var t = Math.Tan(df / 4);
        var hx = 4.0 / 3.0 * r1 * t;
        var hy = 4.0 / 3.0 * r2 * t;

        double[] m1 = [x1, y1];
        double[] m2 = [x1 + hx * s1, y1 - hy * c1];
        double[] m3 = [x2 + hx * s2, y2 - hy * c2];
        double[] m4 = [x2, y2];

        m2[0] = 2 * m1[0] - m2[0];
        m2[1] = 2 * m1[1] - m2[1];

        if (recursive != null)
        {
            List<double[]> temp = [m2, m3, m4];
            return temp.Concat(curves).ToList();
        }

        curves = new List<double[]> { m2, m3, m4 }.Concat(curves).ToList();

        List<double[]> result = [];
        for (var i = 0; i < curves.Count; i += 3)
        {
            var (r1x, r1y) = Rotate(curves[i][0], curves[i][1], angleRad);
            var (r2x, r2y) = Rotate(curves[i + 1][0], curves[i + 1][1], angleRad);
            var (r3x, r3y) = Rotate(curves[i + 2][0], curves[i + 2][1], angleRad);
            result.Add([r1x, r1y, r2x, r2y, r3x, r3y]);
        }

        return result;
    }

    /// <summary>
    ///  Converts a string to a double using InvariantCulture.
    /// </summary>
    /// <param name="s"> The string to convert.</param>
    /// <returns> The converted double value.</returns>
    public static double ToDouble(this string s)
    {
        return double.Parse(s, CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///  Normalize path to include only M, L, C, and Z commands
    /// </summary>
    /// <param name="segments"> The list of segments to normalize.</param>
    /// <returns> The normalized list of segments.</returns>
    public static List<Segment> Normalize(this List<Segment> segments)
    {
        List<Segment> output = [];

        var lastType = '\0';
        double cx = 0, cy = 0;
        double subx = 0, suby = 0;
        double lcx = 0, lcy = 0;

        foreach (var segment in segments)
        {
            switch (segment.Key)
            {
                case 'M':
                    output.Add(segment with { Key = 'M' });
                    (cx, cy) = (segment.Data[0], segment.Data[1]);
                    (subx, suby) = (cx, cy);
                    break;

                case 'C':
                    output.Add(segment with { Key = 'C' });
                    cx = segment.Data[4];
                    cy = segment.Data[5];
                    lcx = segment.Data[2];
                    lcy = segment.Data[3];
                    break;

                case 'L':
                    output.Add(segment with { Key = 'L' });
                    (cx, cy) = (segment.Data[0], segment.Data[1]);
                    break;

                case 'H':
                    cx = segment.Data[0];
                    output.Add(new Segment('L', [cx, cy]));
                    break;

                case 'V':
                    cy = segment.Data[0];
                    output.Add(new Segment('L', [cx, cy]));
                    break;

                case 'S':
                    var (cx1, cy1) = lastType is 'C' or 'S'
                        ? (cx + (cx - lcx), cy + (cy - lcy))
                        : (cx, cy);

                    output.Add(new Segment('C', [cx1, cy1, .. segment.Data]));
                    lcx = segment.Data[0];
                    lcy = segment.Data[1];
                    cx = segment.Data[2];
                    cy = segment.Data[3];
                    break;

                case 'T':
                    var (x, y) = (segment.Data[0], segment.Data[1]);
                    var (x1, y1) = lastType is 'Q' or 'T'
                        ? (cx + (cx - lcx), cy + (cy - lcy))
                        : (cx, cy);

                    var tcx1 = cx + 2 * (x1 - cx) / 3;
                    var tcy1 = cy + 2 * (y1 - cy) / 3;
                    var tcx2 = x + 2 * (x1 - x) / 3;
                    var tcy2 = y + 2 * (y1 - y) / 3;
                    output.Add(new Segment('C', [tcx1, tcy1, tcx2, tcy2, x, y]));
                    (lcx, lcy) = (x1, y1);
                    (cx, cy) = (x, y);
                    break;

                case 'Q':
                    (x1, y1, x, y) = (segment.Data[0], segment.Data[1], segment.Data[2], segment.Data[3]);
                    var qcx1 = cx + 2 * (x1 - cx) / 3;
                    var qcy1 = cy + 2 * (y1 - cy) / 3;
                    var qcx2 = x + 2 * (x1 - x) / 3;
                    var qcy2 = y + 2 * (y1 - y) / 3;
                    output.Add(new Segment('C', [qcx1, qcy1, qcx2, qcy2, x, y]));
                    (lcx, lcy) = (x1, y1);
                    (cx, cy) = (x, y);
                    break;

                case 'A':
                    var r1 = Math.Abs(segment.Data[0]);
                    var r2 = Math.Abs(segment.Data[1]);
                    var angle = segment.Data[2];
                    var largeArcFlag = (int)segment.Data[3];
                    var sweepFlag = (int)segment.Data[4];
                    var ax = segment.Data[5];
                    var ay = segment.Data[6];

                    if (r1 == 0 || r2 == 0)
                    {
                        output.Add(new Segment('C', [cx, cy, ax, ay, ax, ay]));
                        (cx, cy) = (ax, ay);
                    }
                    else if (cx != ax || cy != ay)
                    {
                        var curves = ArcToCubicCurves(cx, cy, ax, ay, r1, r2, angle, largeArcFlag, sweepFlag);
                        output.AddRange(curves.Select(curve => new Segment('C', curve)));
                        (cx, cy) = (ax, ay);
                    }

                    break;

                case 'Z':
                    output.Add(new Segment('Z', []));
                    (cx, cy) = (subx, suby);
                    break;
            }

            lastType = segment.Key;
        }

        return output;
    }
}