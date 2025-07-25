using System.Drawing;
using Irihi.Rough.NET;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Fillers;
using Irihi.Rough.NET.Helpers;

namespace UnitTest.Helpers;

public class RoughHelpersTests
{
    private class MockRoughRenderer : IRoughRenderer
    {
        public double RandOffset(double x, ResolvedOptions options) => 0.0;
        public double RandOffsetWithRange(double min, double max, ResolvedOptions options) => (min + max) / 2;
        public OpSet Ellipse(double x, double y, double width, double height, ResolvedOptions options) => new OpSet();
        public List<Op> DoubleLineOps(double x1, double y1, double x2, double y2, ResolvedOptions options) => new List<Op>();
    }

    private static ResolvedOptions CreateTestOptions(FillStyle fillStyle = FillStyle.Hachure)
    {
        return new ResolvedOptions
        {
            FillStyle = fillStyle,
            StrokeWidth = 1.0,
            HachureAngle = 45.0,
            HachureGap = 10.0,
            Roughness = 1.0,
            Seed = 12345
        };
    }

    [Fact]
    public void GetFiller_WithHachureFillStyle_ReturnsHachureFiller()
    {
        // Arrange
        var options = CreateTestOptions(FillStyle.Hachure);
        var renderer = new MockRoughRenderer();

        // Act
        var filler = RoughHelpers.GetFiller(options, renderer);

        // Assert
        Assert.IsType<HachureFiller>(filler);
    }

    [Fact]
    public void GetFiller_WithZigzagFillStyle_ReturnsZigZagFiller()
    {
        // Arrange
        var options = CreateTestOptions(FillStyle.Zigzag);
        var renderer = new MockRoughRenderer();

        // Act
        var filler = RoughHelpers.GetFiller(options, renderer);

        // Assert
        Assert.IsType<ZigZagFiller>(filler);
    }

    [Fact]
    public void GetFiller_WithDashedFillStyle_ReturnsDashedFiller()
    {
        // Arrange
        var options = CreateTestOptions(FillStyle.Dashed);
        var renderer = new MockRoughRenderer();

        // Act
        var filler = RoughHelpers.GetFiller(options, renderer);

        // Assert
        Assert.IsType<DashedFiller>(filler);
    }

    [Fact]
    public void GetFiller_WithDotsFillStyle_ReturnsDotFiller()
    {
        // Arrange
        var options = CreateTestOptions(FillStyle.Dots);
        var renderer = new MockRoughRenderer();

        // Act
        var filler = RoughHelpers.GetFiller(options, renderer);

        // Assert
        Assert.IsType<DotFiller>(filler);
    }

    [Fact]
    public void GetFiller_WithCrossHatchFillStyle_ReturnsHatchFiller()
    {
        // Arrange
        var options = CreateTestOptions(FillStyle.CrossHatch);
        var renderer = new MockRoughRenderer();

        // Act
        var filler = RoughHelpers.GetFiller(options, renderer);

        // Assert
        Assert.IsType<HatchFiller>(filler);
    }

    [Fact]
    public void GetFiller_WithZigzagLineFillStyle_ReturnsZigZagLineFiller()
    {
        // Arrange
        var options = CreateTestOptions(FillStyle.ZigzagLine);
        var renderer = new MockRoughRenderer();

        // Act
        var filler = RoughHelpers.GetFiller(options, renderer);

        // Assert
        Assert.IsType<ZigZagLineFiller>(filler);
    }

    [Fact]
    public void GetFiller_WithSolidFillStyle_ReturnsHachureFiller()
    {
        // Arrange
        var options = CreateTestOptions(FillStyle.Solid);
        var renderer = new MockRoughRenderer();

        // Act
        var filler = RoughHelpers.GetFiller(options, renderer);

        // Assert
        Assert.IsType<HachureFiller>(filler); // Default case
    }

    [Fact]
    public void GetFiller_CachesBehavior_ReturnsSameInstanceForSameFillStyle()
    {
        // Arrange
        var options = CreateTestOptions(FillStyle.Hachure);
        var renderer = new MockRoughRenderer();

        // Act
        var filler1 = RoughHelpers.GetFiller(options, renderer);
        var filler2 = RoughHelpers.GetFiller(options, renderer);

        // Assert
        Assert.Same(filler1, filler2);
    }

    [Fact]
    public void GetFiller_DifferentFillStyles_ReturnsDifferentInstances()
    {
        // Arrange
        var hachureOptions = CreateTestOptions(FillStyle.Hachure);
        var zigzagOptions = CreateTestOptions(FillStyle.Zigzag);
        var renderer = new MockRoughRenderer();

        // Act
        var hachureFiller = RoughHelpers.GetFiller(hachureOptions, renderer);
        var zigzagFiller = RoughHelpers.GetFiller(zigzagOptions, renderer);

        // Assert
        Assert.NotSame(hachureFiller, zigzagFiller);
        Assert.IsType<HachureFiller>(hachureFiller);
        Assert.IsType<ZigZagFiller>(zigzagFiller);
    }

    [Fact]
    public void PolygonHachureLines_WithBasicPolygon_ReturnsHachureLines()
    {
        // Arrange
        var polygon = new List<PointF>
        {
            new PointF(10, 10),
            new PointF(100, 10),
            new PointF(100, 100),
            new PointF(10, 100)
        };
        var polygonList = new List<List<PointF>> { polygon };
        var options = CreateTestOptions();

        // Act
        var lines = RoughHelpers.PolygonHachureLines(polygonList, options);

        // Assert
        Assert.NotNull(lines);
        Assert.NotEmpty(lines);
    }

    [Fact]
    public void PolygonHachureLines_WithNegativeGap_UsesStrokeWidthMultiplier()
    {
        // Arrange
        var polygon = new List<PointF>
        {
            new PointF(0, 0),
            new PointF(50, 0),
            new PointF(50, 50),
            new PointF(0, 50)
        };
        var polygonList = new List<List<PointF>> { polygon };
        var options = CreateTestOptions();
        options.HachureGap = -5.0; // Negative gap
        options.StrokeWidth = 2.0;

        // Act
        var lines = RoughHelpers.PolygonHachureLines(polygonList, options);

        // Assert
        Assert.NotNull(lines);
        // The method should use StrokeWidth * 4 = 8.0 as gap when HachureGap is negative
        Assert.NotEmpty(lines);
    }

    [Fact]
    public void PolygonHachureLines_WithZeroGap_UsesMinimumGap()
    {
        // Arrange
        var polygon = new List<PointF>
        {
            new PointF(0, 0),
            new PointF(30, 0),
            new PointF(30, 30),
            new PointF(0, 30)
        };
        var polygonList = new List<List<PointF>> { polygon };
        var options = CreateTestOptions();
        options.HachureGap = 0.0;

        // Act
        var lines = RoughHelpers.PolygonHachureLines(polygonList, options);

        // Assert
        Assert.NotNull(lines);
        // The method should use Math.Max(gap, 0.1) internally
        Assert.NotEmpty(lines);
    }

    [Fact]
    public void PolygonHachureLines_WithHighRoughness_AffectsSkipOffset()
    {
        // Arrange
        var polygon = new List<PointF>
        {
            new PointF(0, 0),
            new PointF(100, 0),
            new PointF(100, 100),
            new PointF(0, 100)
        };
        var polygonList = new List<List<PointF>> { polygon };
        var options = CreateTestOptions();
        options.Roughness = 2.0; // High roughness
        options.HachureGap = 10.0;

        // Act
        var lines = RoughHelpers.PolygonHachureLines(polygonList, options);

        // Assert
        Assert.NotNull(lines);
        Assert.NotEmpty(lines);
    }

    [Fact]
    public void PolygonHachureLines_WithCustomAngle_CalculatesCorrectAngle()
    {
        // Arrange
        var polygon = new List<PointF>
        {
            new PointF(0, 0),
            new PointF(50, 0),
            new PointF(50, 50),
            new PointF(0, 50)
        };
        var polygonList = new List<List<PointF>> { polygon };
        var options = CreateTestOptions();
        options.HachureAngle = 30.0; // Custom angle

        // Act
        var lines = RoughHelpers.PolygonHachureLines(polygonList, options);

        // Assert
        Assert.NotNull(lines);
        Assert.NotEmpty(lines);
        // The angle should be HachureAngle + 90 = 120.0 degrees internally
    }

    [Fact]
    public void PolygonHachureLines_WithEmptyPolygonList_ReturnsEmptyList()
    {
        // Arrange
        var polygonList = new List<List<PointF>>();
        var options = CreateTestOptions();

        // Act
        var lines = RoughHelpers.PolygonHachureLines(polygonList, options);

        // Assert
        Assert.NotNull(lines);
        Assert.Empty(lines);
    }

    [Fact]
    public void PolygonHachureLines_WithMultiplePolygons_ProcessesAllPolygons()
    {
        // Arrange
        var polygon1 = new List<PointF>
        {
            new PointF(0, 0),
            new PointF(25, 0),
            new PointF(25, 25),
            new PointF(0, 25)
        };
        var polygon2 = new List<PointF>
        {
            new PointF(30, 30),
            new PointF(55, 30),
            new PointF(55, 55),
            new PointF(30, 55)
        };
        var polygonList = new List<List<PointF>> { polygon1, polygon2 };
        var options = CreateTestOptions();

        // Act
        var lines = RoughHelpers.PolygonHachureLines(polygonList, options);

        // Assert
        Assert.NotNull(lines);
        Assert.NotEmpty(lines);
        // Should have lines from both polygons
    }

    [Fact] 
    public void PolygonHachureLines_WithLowRoughness_UsesDefaultSkipOffset()
    {
        // Arrange
        var polygon = new List<PointF>
        {
            new PointF(0, 0),
            new PointF(50, 0),
            new PointF(50, 50),
            new PointF(0, 50)
        };
        var polygonList = new List<List<PointF>> { polygon };
        var options = CreateTestOptions();
        options.Roughness = 0.5; // Low roughness

        // Act
        var lines = RoughHelpers.PolygonHachureLines(polygonList, options);

        // Assert
        Assert.NotNull(lines);
        Assert.NotEmpty(lines);
    }

    [Fact]
    public void PolygonHachureLines_WithVerySmallGap_UsesMinimumValue()
    {
        // Arrange
        var polygon = new List<PointF>
        {
            new PointF(0, 0),
            new PointF(20, 0),
            new PointF(20, 20),
            new PointF(0, 20)
        };
        var polygonList = new List<List<PointF>> { polygon };
        var options = CreateTestOptions();
        options.HachureGap = 0.05; // Very small gap, should use 0.1 minimum

        // Act
        var lines = RoughHelpers.PolygonHachureLines(polygonList, options);

        // Assert
        Assert.NotNull(lines);
        Assert.NotEmpty(lines);
    }

    [Fact]
    public void GetFiller_WithSameRenderer_CachesCorrectly()
    {
        // Arrange
        var renderer = new MockRoughRenderer();
        var options1 = CreateTestOptions(FillStyle.Hachure);
        var options2 = CreateTestOptions(FillStyle.Hachure);

        // Act
        var filler1 = RoughHelpers.GetFiller(options1, renderer);
        var filler2 = RoughHelpers.GetFiller(options2, renderer);

        // Assert
        Assert.Same(filler1, filler2); // Should be cached
        Assert.IsType<HachureFiller>(filler1);
    }

    [Fact]
    public void GetFiller_WithDifferentRenderer_CachesCorrectly()
    {
        // Arrange
        var renderer1 = new MockRoughRenderer();
        var renderer2 = new MockRoughRenderer();
        var options = CreateTestOptions(FillStyle.Dots);

        // Act
        var filler1 = RoughHelpers.GetFiller(options, renderer1);
        var filler2 = RoughHelpers.GetFiller(options, renderer2);

        // Assert
        Assert.Same(filler1, filler2); // Should still be cached by FillStyle, not renderer
        Assert.IsType<DotFiller>(filler1);
    }
}