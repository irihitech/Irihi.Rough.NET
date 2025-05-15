using Irihi.Rough.NET.Dependencies.PathDataParser;

namespace UnitTest.PathDataParser;

public class NormalizeTests
{
    [Fact]
    public void NormalizeEmptySegmentList_ReturnsEmptyList()
    {
        List<Segment> segments = [];
        var result = segments.Normalize();
        Assert.Empty(result);
    }

    [Fact]
    public void Test()
    {
        var segments = PathDataParserFunctions.ParsePath("M 10 80 Q 52.5 10, 95 80 T 180 80");
        var absoluteSegments = segments.Absolutize();
        var normalizedSegments = absoluteSegments.Normalize();
        Assert.Equal(3, normalizedSegments.Count);

        // First segment - Move
        Assert.Equal('M', normalizedSegments[0].Key);
        Assert.Equal(10.0, normalizedSegments[0].Data[0]);
        Assert.Equal(80.0, normalizedSegments[0].Data[1]);

        // Second segment - First cubic curve
        Assert.Equal('C', normalizedSegments[1].Key);
        Assert.Equal(38.33, normalizedSegments[1].Data[0], 0.01);
        Assert.Equal(33.33, normalizedSegments[1].Data[1], 0.01);
        Assert.Equal(66.67, normalizedSegments[1].Data[2], 0.01);
        Assert.Equal(33.33, normalizedSegments[1].Data[3], 0.01);
        Assert.Equal(95.0, normalizedSegments[1].Data[4], 0.01);
        Assert.Equal(80.0, normalizedSegments[1].Data[5], 0.01);

        // Third segment - Second cubic curve
        Assert.Equal('C', normalizedSegments[2].Key);
        Assert.Equal(123.33, normalizedSegments[2].Data[0], 0.01);
        Assert.Equal(126.67, normalizedSegments[2].Data[1], 0.01);
        Assert.Equal(151.67, normalizedSegments[2].Data[2], 0.01);
        Assert.Equal(126.67, normalizedSegments[2].Data[3], 0.01);
        Assert.Equal(180.0, normalizedSegments[2].Data[4], 0.01);
        Assert.Equal(80.0, normalizedSegments[2].Data[5], 0.01);
    }
}