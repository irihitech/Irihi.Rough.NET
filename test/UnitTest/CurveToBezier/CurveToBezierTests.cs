using System.Drawing;
using Irihi.Rough.NET.Dependencies.CurveToBezier;
using Irihi.Rough.NET.Dependencies.PathDataParser;
using Irihi.Rough.NET.Helpers;

namespace UnitTest.CurveToBezier;

public class CurveToBezierTests
{
    [Fact]
    public void ThrowsArgumentExceptionWhenLessThanThreePoints()
    {
        List<PointF> points = [new(0, 0), new(1, 1)];

        Assert.Throws<ArgumentException>(() => CurveToBezierFunctions.CurveToBezier(points));
    }

    [Fact]
    public void ReturnsCorrectPointsForThreePointCurve()
    {
        List<PointF> points = [new(0, 0), new(1, 1), new(2, 0)];

        var result = CurveToBezierFunctions.CurveToBezier(points);

        Assert.Equal(4, result.Count);
        Assert.Equal(points[0], result[0]);
        Assert.Equal(points[1], result[1]);
        Assert.Equal(points[2], result[2]);
        Assert.Equal(points[2], result[3]);
    }

    [Fact]
    public void ReturnsExpectedPointsForFourPointCurve()
    {
        List<PointF> points = [new(0, 0), new(1, 1), new(2, 1), new(3, 0)];

        var result = CurveToBezierFunctions.CurveToBezier(points);

        Assert.Equal(10, result.Count);
    }

    [Fact]
    public void CurveTightnessAffectsControlPoints()
    {
        List<PointF> points = [new(0, 0), new(1, 1), new(2, 1), new(3, 0)];

        var resultDefault = CurveToBezierFunctions.CurveToBezier(points);
        var resultTight = CurveToBezierFunctions.CurveToBezier(points, 0.5);

        Assert.NotEqual(resultDefault[1], resultTight[1]);
    }

    [Fact]
    public void ClonePointReturnsNewInstanceWithSameCoordinates()
    {
        var original = new PointF(1.5f, 2.5f);

        var cloned = original.Clone();

        Assert.Equal(original.X, cloned.X);
        Assert.Equal(original.Y, cloned.Y);
    }

    [Fact]
    public void DistanceMethodCalculatesCorrectly()
    {
        var p1 = new PointF(0, 0);
        var p2 = new PointF(3, 4);

        var distance = PointFHelper.Distance(p1, p2);

        Assert.Equal(5.0, distance);
    }

    [Theory]
    [InlineData(0, 0, 3, 4, 25)] // 3-4-5 triangle
    [InlineData(1, 1, 4, 5, 25)] // Same distance, different origin
    [InlineData(-2, -2, 1, 1, 18)] // Negative coordinates
    [InlineData(0, 0, 0, 0, 0)] // Same point
    [InlineData(5, 0, -5, 0, 100)] // Horizontal line
    [InlineData(0, 5, 0, -5, 100)] // Vertical line
    public void DistanceSquaredCalculatesCorrectly(double x1, double y1, double x2, double y2, double expected)
    {
        var p1 = PointFHelper.Create(x1, y1);
        var p2 = PointFHelper.Create(x2, y2);

        var result = PointFHelper.DistanceSquared(p1, p2);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 0, 10, 10, 0, 0, 0)] // t=0 returns first point
    [InlineData(0, 0, 10, 10, 1, 10, 10)] // t=1 returns second point
    [InlineData(0, 0, 10, 10, 0.5, 5, 5)] // t=0.5 returns midpoint
    [InlineData(2, 2, 4, 4, 0.25, 2.5, 2.5)] // t=0.25 returns quarter point
    [InlineData(-4, -4, 4, 4, 0.5, 0, 0)] // Interpolation with negative coordinates
    [InlineData(0, 0, 10, 0, 0.3, 3, 0)] // Horizontal line
    [InlineData(0, 0, 0, 10, 0.3, 0, 3)] // Vertical line
    public void LerpInterpolatesCorrectly(double x1, double y1, double x2, double y2, double t, double expectedX, double expectedY)
    {
        var p1 = PointFHelper.Create(x1, y1);
        var p2 = PointFHelper.Create(x2, y2);

        var result = CurveToBezierFunctions.Lerp(p1, p2, t);

        Assert.Equal(expectedX, result.X);
        Assert.Equal(expectedY, result.Y);
    }

    [Theory]
    [InlineData(5, 5, 0, 0, 10, 0, 25)] // Point above horizontal line segment
    [InlineData(5, 0, 0, 0, 10, 0, 0)] // Point on horizontal line segment
    [InlineData(-5, 0, 0, 0, 10, 0, 25)] // Point before start of segment
    [InlineData(15, 0, 0, 0, 10, 0, 25)] // Point past end of segment
    [InlineData(1, 1, 0, 0, 0, 0, 2)] // Zero length segment
    [InlineData(5, 5, 0, 0, 5, 5, 0)] // Point on diagonal segment endpoint
    [InlineData(2.5, 2.5, 0, 0, 5, 5, 0)] // Point on diagonal segment middle
    [InlineData(0, 5, 0, 0, 0, 10, 0)] // Point on vertical segment
    [InlineData(3, 4, 0, 0, 0, 5, 9)] // Point near vertical segment
    public void DistanceToSegmentSquaredCalculatesCorrectly(
        double px, double py, // Point coordinates
        double vx, double vy, // Segment start
        double wx, double wy, // Segment end
        double expected) // Expected squared distance
    {
        var point = PointFHelper.Create(px, py);
        var segmentStart = PointFHelper.Create(vx, vy);
        var segmentEnd = PointFHelper.Create(wx, wy);

        var result = CurveToBezierFunctions.DistanceToSegmentSquared(point, segmentStart, segmentEnd);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 0, 1, 0, 2, 0, 3, 0, 0)] // Straight horizontal line
    [InlineData(0, 0, 0, 1, 0, 2, 0, 3, 0)] // Straight vertical line
    [InlineData(0, 0, 1, 1, 2, 2, 3, 3, 0)] // Straight diagonal line
    [InlineData(0, 0, 3, 3, 3, -3, 6, 0, 90)] // Curved path
    [InlineData(0, 0, 0, 3, 6, 3, 6, 0, 117)] // Semi-circular curve
    [InlineData(0, 0, 2, 2, -2, 2, 0, 0, 72)] // Control points forming triangle
    [InlineData(-3, -3, -1, -1, 1, 1, 3, 3, 0)] // Straight line with negative coordinates
    [InlineData(0, 0, 10, 0, 10, 0, 0, 0, 900)] // Loop curve
    public void FlatnessCalculatesCorrectly(
        double x1, double y1, double x2, double y2,
        double x3, double y3, double x4, double y4,
        double expected)
    {
        PointF[] points =
        [
            PointFHelper.Create(x1, y1), PointFHelper.Create(x2, y2),
            PointFHelper.Create(x3, y3), PointFHelper.Create(x4, y4)
        ];

        var result = CurveToBezierFunctions.Flatness(points, 0);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   \t\n,,,   ")]
    public void TokenizeEmptyOrWhitespaceReturnsOnlyEndToken(string input)
    {
        var tokens = PathDataParserFunctions.Tokenize(input);

        Assert.Single(tokens);
        Assert.Equal(PathToken.End, tokens[0]);
    }

    [Theory]
    [InlineData("X")]
    [InlineData("123x")]
    [InlineData("M 12 x")]
    [InlineData("1e")]
    public void TokenizeInvalidInputReturnsEmptyList(string input)
    {
        var tokens = PathDataParserFunctions.Tokenize(input);

        Assert.Empty(tokens);
    }

    [Theory]
    [InlineData("M 10,20 L 30,40", 7)]
    [InlineData("M10-20L30-40", 7)]
    [InlineData("M.5.6L.7.8", 7)]
    public void TokenizeParsesMultipleCommandsAndNumbers(string input, int expectedCount)
    {
        var tokens = PathDataParserFunctions.Tokenize(input);

        Assert.Equal(expectedCount, tokens.Count);
    }
}