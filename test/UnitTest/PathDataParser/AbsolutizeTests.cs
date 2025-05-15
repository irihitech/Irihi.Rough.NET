using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Dependencies.HachureFill;
using Irihi.Rough.NET.Dependencies.PathDataParser;

namespace UnitTest.PathDataParser;

public class AbsolutizeTests
{
    [Fact]
    public void RotateLines_Test()
    {
        List<RoughLine> lines = [new(new PointF(1, 1), new PointF(3, 3))];
        HachureFillFunctions.RotateLines(lines, new PointF(2, 10), 90);
        Assert.Equal(11, lines[0].Start.X);
        Assert.Equal(9, lines[0].Start.Y);
        Assert.Equal(9, lines[0].End.X);
        Assert.Equal(11, lines[0].End.Y);
    }

    [Fact]
    public void AbsolutizeEmptySegmentList_ReturnsEmptyList()
    {
        List<Segment> segments = [];
        var result = segments.Absolutize();
        Assert.Empty(result);
    }

    [Fact]
    public void AbsolutizeMoveCommands_ConvertsRelativeToAbsolute()
    {
        List<Segment> segments =
        [
            new('m', [10.0, 20.0]),
            new('m', [30.0, 40.0])
        ];
        var result = segments.Absolutize();
        Assert.Equal('M', result[0].Key);
        Assert.Equal('M', result[1].Key);
        Assert.Equal([10.0, 20.0], result[0].Data);
        Assert.Equal([40.0, 60.0], result[1].Data);
    }

    [Fact]
    public void AbsolutizeCurveCommand_ConvertsRelativeToAbsolute()
    {
        List<Segment> segments =
        [
            new('M', [10.0, 10.0]),
            new('c', [20.0, 20.0, 40.0, 40.0, 60.0, 60.0])
        ];
        var result = segments.Absolutize();
        Assert.Equal('C', result[1].Key);
        Assert.Equal([30.0, 30.0, 50.0, 50.0, 70.0, 70.0], result[1].Data);
    }

    [Fact]
    public void AbsolutizeClosePath_MaintainsSubpathStartPoint()
    {
        List<Segment> segments =
        [
            new('M', [10.0, 20.0]),
            new('l', [30.0, 40.0]),
            new('Z', []),
            new('l', [50.0, 60.0])
        ];
        var result = segments.Absolutize();
        Assert.Equal([60.0, 80.0], result[3].Data);
    }

    [Fact]
    public void AbsolutizeHorizontalVerticalLines_ConvertsToAbsolute()
    {
        List<Segment> segments =
        [
            new('M', [10.0, 10.0]),
            new('h', [20.0]),
            new('v', [30.0])
        ];
        var result = segments.Absolutize();
        Assert.Equal([30.0], result[1].Data);
        Assert.Equal([40.0], result[2].Data);
    }

    [Fact]
    public void AbsolutizeSmoothCurve_ConvertsRelativeToAbsolute()
    {
        List<Segment> segments =
        [
            new('M', [10.0, 10.0]),
            new('s', [20.0, 20.0, 30.0, 30.0])
        ];
        var result = segments.Absolutize();
        Assert.Equal('S', result[1].Key);
        Assert.Equal([30.0, 30.0, 40.0, 40.0], result[1].Data);
    }

    [Fact]
    public void AbsolutizeQuadraticCurve_ConvertsRelativeToAbsolute()
    {
        List<Segment> segments =
        [
            new('M', [10.0, 10.0]),
            new('q', [20.0, 20.0, 30.0, 30.0])
        ];
        var result = segments.Absolutize();
        Assert.Equal('Q', result[1].Key);
        Assert.Equal([30.0, 30.0, 40.0, 40.0], result[1].Data);
    }
}