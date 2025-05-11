namespace Irihi.Rough.NET.Dependencies.PathDataParser;

public static class AbsolutizeFunctions
{
    /// <summary>
    ///  Converts relative path commands to absolute path commands.
    /// </summary>
    /// <param name="segments">The list of segments to convert.</param>
    /// <returns>A new list of segments with all relative commands converted to absolute commands.</returns>
    public static List<Segment> Absolutize(this List<Segment> segments)
    {
        double cx = 0, cy = 0;
        double subx = 0, suby = 0;
        List<Segment> output = [];

        foreach (var segment in segments)
        {
            switch (segment.Key)
            {
                case 'M':
                    output.Add(segment with { Key = 'M' });
                    (cx, cy) = (segment.Data[0], segment.Data[1]);
                    (subx, suby) = (cx, cy);
                    break;
                case 'm':
                    cx += segment.Data[0];
                    cy += segment.Data[1];
                    output.Add(new Segment('M', [cx, cy]));
                    (subx, suby) = (cx, cy);
                    break;
                case 'L':
                    output.Add(segment with { Key = 'L' });
                    (cx, cy) = (segment.Data[0], segment.Data[1]);
                    break;
                case 'l':
                    cx += segment.Data[0];
                    cy += segment.Data[1];
                    output.Add(new Segment('L', [cx, cy]));
                    break;
                case 'C':
                    output.Add(new Segment('C', segment.Data.ToArray()));
                    cx = segment.Data[4];
                    cy = segment.Data[5];
                    break;
                case 'c':
                    var newCurveData = segment.Data.Select((d, i) => i % 2 == 1 ? d + cy : d + cx).ToArray();
                    output.Add(new Segment('C', newCurveData));
                    cx = newCurveData[4];
                    cy = newCurveData[5];
                    break;
                case 'Q':
                    output.Add(segment with { Key = 'Q' });
                    cx = segment.Data[2];
                    cy = segment.Data[3];
                    break;
                case 'q':
                    var newQuadData = segment.Data.Select((d, i) => i % 2 == 1 ? d + cy : d + cx).ToArray();
                    output.Add(new Segment('Q', newQuadData));
                    cx = newQuadData[2];
                    cy = newQuadData[3];
                    break;
                case 'A':
                    output.Add(segment with { Key = 'A' });
                    cx = segment.Data[5];
                    cy = segment.Data[6];
                    break;
                case 'a':
                    cx += segment.Data[5];
                    cy += segment.Data[6];
                    output.Add(new Segment('A', [
                        segment.Data[0], segment.Data[1], segment.Data[2],
                        segment.Data[3], segment.Data[4], cx, cy
                    ]));
                    break;
                case 'H':
                    output.Add(segment with { Key = 'H' });
                    cx = segment.Data[0];
                    break;
                case 'h':
                    cx += segment.Data[0];
                    output.Add(new Segment('H', [cx]));
                    break;
                case 'V':
                    output.Add(segment with { Key = 'V' });
                    cy = segment.Data[0];
                    break;
                case 'v':
                    cy += segment.Data[0];
                    output.Add(new Segment('V', [cy]));
                    break;
                case 'S':
                    output.Add(segment with { Key = 'S' });
                    cx = segment.Data[2];
                    cy = segment.Data[3];
                    break;
                case 's':
                    var newSmoothData = segment.Data.Select((d, i) => i % 2 == 1 ? d + cy : d + cx).ToArray();
                    output.Add(new Segment('S', newSmoothData));
                    cx = newSmoothData[2];
                    cy = newSmoothData[3];
                    break;
                case 'T':
                    output.Add(segment with { Key = 'T' });
                    cx = segment.Data[0];
                    cy = segment.Data[1];
                    break;
                case 't':
                    cx += segment.Data[0];
                    cy += segment.Data[1];
                    output.Add(new Segment('T', [cx, cy]));
                    break;
                case 'Z':
                case 'z':
                    output.Add(new Segment('Z', []));
                    cx = subx;
                    cy = suby;
                    break;
            }
        }

        return output;
    }
}