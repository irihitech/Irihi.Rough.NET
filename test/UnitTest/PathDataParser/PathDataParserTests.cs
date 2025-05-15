using Irihi.Rough.NET.Dependencies.PathDataParser;

namespace UnitTest.PathDataParser;

public class PathDataParserTests
{
    [Theory]
    [InlineData("M10,20", 1)]
    [InlineData("m10,20", 1)]
    [InlineData("m10,20L30,40", 2)]
    [InlineData("M10,20L30,40", 2)]
    [InlineData("M10,20l30,40", 2)]
    public void ParsingPathWithMoveCommand_ReturnsCorrectSegments(string path, int expectedSegments)
    {
        var segments = PathDataParserFunctions.ParsePath(path);
        Assert.Equal(expectedSegments, segments.Count);
    }

    [Theory]
    [InlineData("L10,20")]
    [InlineData("H10")]
    [InlineData("V20")]
    public void ParsingPathWithoutMoveCommand_PrependsImplicitMove(string path)
    {
        var segments = PathDataParserFunctions.ParsePath(path);
        Assert.Equal('M', segments[0].Key);
        Assert.Equal(2, segments.Count);
    }

    [Fact]
    public void ParsingPathWithInvalidNumber_ThrowsException()
    {
        var segments = PathDataParserFunctions.ParsePath("M10,abc");
        Assert.Empty(segments);
    }

    [Fact]
    public void ParsingPathWithInsufficientParameters_ThrowsException()
    {
        var ex = Assert.Throws<Exception>(() => PathDataParserFunctions.ParsePath("M10"));
        Assert.Contains("Path data ended short", ex.Message);
    }

    [Fact]
    public void ParsingPathWithInvalidCommand_ThrowsException()
    {
        var segments = PathDataParserFunctions.ParsePath("M10,20K30,40");
        Assert.Empty(segments);
    }

    [Fact]
    public void ParsingEmptyPath_ReturnsEmptyList()
    {
        var segments = PathDataParserFunctions.ParsePath(string.Empty);
        Assert.Empty(segments);
    }


    [Fact]
    public void SerializeEmptySegmentList_ReturnsEmptyString()
    {
        List<Segment> segments = [];
        Assert.Equal(string.Empty, PathDataParserFunctions.Serialize(segments));
    }

    [Theory]
    [InlineData("M 10 20", 'M', new double[] { 10, 20 })]
    [InlineData("L 30 40", 'L', new double[] { 30, 40 })]
    public void SerializeSimpleSegment_ReturnsCorrectFormat(string expected, char key, double[] data)
    {
        List<Segment> segments = [new(key, data)];
        Assert.Equal(expected, PathDataParserFunctions.Serialize(segments));
    }

    [Fact]
    public void SerializeCubicBezier_FormatsWithCommas()
    {
        List<Segment> segments = [new('C', [10.0, 20.0, 30.0, 40.0, 50.0, 60.0])];
        Assert.Equal("C 10 20, 30 40, 50 60", PathDataParserFunctions.Serialize(segments));
    }

    [Fact]
    public void SerializeQuadraticBezier_FormatsWithCommas()
    {
        List<Segment> segments = [new('Q', [10.0, 20.0, 30.0, 40.0])];
        Assert.Equal("Q 10 20, 30 40", PathDataParserFunctions.Serialize(segments));
    }

    [Fact]
    public void SerializeMultipleSegments_JoinsWithSpaces()
    {
        List<Segment> segments =
        [
            new('M', [10.0, 20.0]),
            new('L', [30.0, 40.0]),
            new('Z', [])
        ];
        Assert.Equal("M 10 20 L 30 40 Z", PathDataParserFunctions.Serialize(segments));
    }

    [Fact]
    public void SerializeDecimalNumbers_UsesInvariantCulture()
    {
        List<Segment> segments = [new('M', [10.5, 20.7])];
        Assert.Equal("M 10.5 20.7", PathDataParserFunctions.Serialize(segments));
    }

    [Theory]
    [InlineData('s')]
    [InlineData('S')]
    public void SerializeSmoothCurveCommand_FormatsWithCommas(char command)
    {
        List<Segment> segments = [new(command, [10.0, 20.0, 30.0, 40.0])];
        Assert.Equal($"{command} 10 20, 30 40", PathDataParserFunctions.Serialize(segments));
    }
}