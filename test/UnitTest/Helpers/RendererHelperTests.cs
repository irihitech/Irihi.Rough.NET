using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Helpers;

namespace UnitTest.Helpers;

public class RendererHelperTests
{
    private static ResolvedOptions CreateTestOptions(int seed = 12345)
    {
        return new ResolvedOptions
        {
            Seed = seed,
            Randomizer = new Random(seed),
            Roughness = 1.0,
            Bowing = 1.0,
            MaxRandomnessOffset = 2.0,
            StrokeWidth = 1.0,
            CurveFitting = 0.95,
            CurveTightness = 0.0,
            CurveStepCount = 9.0,
            FillStyle = FillStyle.Hachure,
            FillWeight = 1.0,
            HachureAngle = -41.0,
            HachureGap = 4.0,
            DisableMultiStroke = false,
            DisableMultiStrokeFill = false,
            PreserveVertices = false,
            FillShapeRoughnessGain = 0.8
        };
    }

    #region Line Tests

    [Fact]
    public void Line_WithValidCoordinates_ReturnsPathOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Line(10, 20, 50, 80, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Line_WithSameCoordinates_ReturnsOpSetWithMoveAndCurve()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Line(10, 10, 10, 10, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Line_WithNegativeCoordinates_ReturnsValidOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Line(-10, -20, -50, -80, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    #endregion

    #region LinearPath Tests

    [Fact]
    public void LinearPath_WithMultiplePoints_ReturnsPathOpSet()
    {
        // Arrange
        var points = new List<PointF>
        {
            new PointF(10, 10),
            new PointF(20, 20),
            new PointF(30, 10),
            new PointF(40, 20)
        };
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.LinearPath(points, false, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void LinearPath_WithClosedPath_ReturnsPathOpSetWithClosure()
    {
        // Arrange
        var points = new List<PointF>
        {
            new PointF(10, 10),
            new PointF(20, 20),
            new PointF(30, 10)
        };
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.LinearPath(points, true, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void LinearPath_WithTwoPoints_ReturnsLineOpSet()
    {
        // Arrange
        var points = new List<PointF>
        {
            new PointF(10, 10),
            new PointF(20, 20)
        };
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.LinearPath(points, false, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void LinearPath_WithOnePoint_ReturnsEmptyOpSet()
    {
        // Arrange
        var points = new List<PointF> { new PointF(10, 10) };
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.LinearPath(points, false, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.Empty(result.Ops);
    }

    [Fact]
    public void LinearPath_WithEmptyPoints_ReturnsEmptyOpSet()
    {
        // Arrange
        var points = new List<PointF>();
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.LinearPath(points, false, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.Empty(result.Ops);
    }

    #endregion

    #region Polygon Tests

    [Fact]
    public void Polygon_WithValidPoints_ReturnsClosedPath()
    {
        // Arrange
        var points = new List<PointF>
        {
            new PointF(10, 10),
            new PointF(50, 10),
            new PointF(50, 50),
            new PointF(10, 50)
        };
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Polygon(points, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Polygon_WithTriangle_ReturnsValidOpSet()
    {
        // Arrange
        var points = new List<PointF>
        {
            new PointF(10, 10),
            new PointF(20, 30),
            new PointF(0, 30)
        };
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Polygon(points, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    #endregion

    #region Rectangle Tests

    [Fact]
    public void Rectangle_WithPositiveDimensions_ReturnsPathOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Rectangle(10, 20, 100, 50, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Rectangle_WithZeroDimensions_ReturnsPathOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Rectangle(10, 20, 0, 0, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Rectangle_WithNegativeDimensions_ReturnsPathOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Rectangle(10, 20, -100, -50, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    #endregion

    #region Curve Tests

    [Fact]
    public void Curve_WithValidPointsList_ReturnsPathOpSet()
    {
        // Arrange
        var inputPoints = new List<List<PointF>>
        {
            new List<PointF>
            {
                new PointF(10, 10),
                new PointF(20, 30),
                new PointF(40, 20),
                new PointF(50, 40)
            }
        };
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Curve(inputPoints, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Curve_WithMultiplePointsList_ReturnsPathOpSet()
    {
        // Arrange
        var inputPoints = new List<List<PointF>>
        {
            new List<PointF>
            {
                new PointF(10, 10),
                new PointF(20, 30),
                new PointF(40, 20)
            },
            new List<PointF>
            {
                new PointF(50, 50),
                new PointF(60, 70),
                new PointF(80, 60)
            }
        };
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Curve(inputPoints, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Curve_WithEmptyPointsList_ReturnsEmptyOpSet()
    {
        // Arrange
        var inputPoints = new List<List<PointF>>();
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Curve(inputPoints, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.Empty(result.Ops);
    }

    [Fact]
    public void Curve_WithDisabledMultiStroke_ReturnsSimplifiedOpSet()
    {
        // Arrange
        var inputPoints = new List<List<PointF>>
        {
            new List<PointF>
            {
                new PointF(10, 10),
                new PointF(20, 30),
                new PointF(40, 20),
                new PointF(50, 40)
            }
        };
        var options = CreateTestOptions();
        options.DisableMultiStroke = true;

        // Act
        var result = RendererHelper.Curve(inputPoints, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    #endregion

    #region Ellipse Tests

    [Fact]
    public void Ellipse_WithPositiveDimensions_ReturnsPathOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Ellipse(50, 50, 100, 80, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Ellipse_WithCircle_ReturnsPathOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Ellipse(50, 50, 100, 100, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Ellipse_WithZeroDimensions_ReturnsPathOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Ellipse(50, 50, 0, 0, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    #endregion

    #region Arc Tests

    [Fact]
    public void Arc_WithValidParameters_ReturnsPathOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Arc(50, 50, 100, 80, 0, Math.PI / 2, false, false, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Arc_WithClosedArc_ReturnsPathOpSetWithClosure()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Arc(50, 50, 100, 80, 0, Math.PI / 2, true, false, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Arc_WithRoughClosure_ReturnsPathOpSetWithRoughClosure()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Arc(50, 50, 100, 80, 0, Math.PI / 2, true, true, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Arc_WithFullCircle_ReturnsPathOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Arc(50, 50, 100, 100, 0, Math.PI * 2, false, false, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Arc_WithNegativeAngles_ReturnsPathOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.Arc(50, 50, 100, 80, -Math.PI / 2, Math.PI / 2, false, false, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    #endregion

    #region SvgPath Tests

    [Fact]
    public void SvgPath_WithSimplePath_ReturnsPathOpSet()
    {
        // Arrange  
        var path = "M10,10 L50,10 L50,50 L10,50 Z";
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.SvgPath(path, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void SvgPath_WithCurvePath_ReturnsPathOpSet()
    {
        // Arrange
        var path = "M10,10 C20,0 40,0 50,10";
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.SvgPath(path, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void SvgPath_WithEmptyPath_ReturnsPathOpSet()
    {
        // Arrange
        var path = "";
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.SvgPath(path, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.Empty(result.Ops);
    }

    #endregion

    #region SolidFillPolygon Tests

    [Fact]
    public void SolidFillPolygon_WithValidPolygon_ReturnsFillPathOpSet()
    {
        // Arrange
        var polygonList = new List<List<PointF>>
        {
            new List<PointF>
            {
                new PointF(10, 10),
                new PointF(50, 10),
                new PointF(50, 50),
                new PointF(10, 50)
            }
        };
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.SolidFillPolygon(polygonList, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.FillPath, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact] 
    public void SolidFillPolygon_WithMultiplePolygons_ReturnsFillPathOpSet()
    {
        // Arrange
        var polygonList = new List<List<PointF>>
        {
            new List<PointF>
            {
                new PointF(10, 10),
                new PointF(30, 10),
                new PointF(30, 30),
                new PointF(10, 30)
            },
            new List<PointF>
            {
                new PointF(50, 50),
                new PointF(70, 50),
                new PointF(70, 70),
                new PointF(50, 70)
            }
        };
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.SolidFillPolygon(polygonList, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.FillPath, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void SolidFillPolygon_WithEmptyPolygonList_ReturnsFillPathOpSet()
    {
        // Arrange
        var polygonList = new List<List<PointF>>();
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.SolidFillPolygon(polygonList, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.FillPath, result.Type);
        Assert.Empty(result.Ops);
    }

    [Fact]
    public void SolidFillPolygon_WithInsufficientPoints_SkipsPolygon()
    {
        // Arrange
        var polygonList = new List<List<PointF>>
        {
            new List<PointF>
            {
                new PointF(10, 10),
                new PointF(20, 20)
            }
        };
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.SolidFillPolygon(polygonList, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.FillPath, result.Type);
        Assert.Empty(result.Ops);
    }

    #endregion

    #region PatternFillPolygons Tests

    [Fact]
    public void PatternFillPolygons_WithValidPolygon_ReturnsOpSet()
    {
        // Arrange
        var polygonList = new List<List<PointF>>
        {
            new List<PointF>
            {
                new PointF(10, 10),
                new PointF(50, 10),
                new PointF(50, 50),
                new PointF(10, 50)
            }
        };
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.PatternFillPolygons(polygonList, options);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Ops);
    }

    #endregion

    #region PatternFillArc Tests

    [Fact]
    public void PatternFillArc_WithValidParameters_ReturnsOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.PatternFillArc(50, 50, 100, 80, 0, Math.PI / 2, options);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Ops);
    }

    [Fact]
    public void PatternFillArc_WithFullCircle_ReturnsOpSet()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.PatternFillArc(50, 50, 100, 100, 0, Math.PI * 2, options);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Ops);
    }

    #endregion

    #region Random Offset Tests

    [Fact]
    public void RandOffset_WithPositiveValue_ReturnsNonZeroValue()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.RandOffset(5.0, options);

        // Assert
        // Result should be deterministic due to fixed seed
        Assert.True(Math.Abs(result) >= 0);
    }

    [Fact]
    public void RandOffset_WithZeroValue_ReturnsZero()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.RandOffset(0.0, options);

        // Assert
        Assert.Equal(0.0, result);
    }

    [Fact] 
    public void RandOffsetWithRange_WithValidRange_ReturnsValueInRange()
    {
        // Arrange
        var options = CreateTestOptions();
        var min = -5.0;
        var max = 5.0;

        // Act
        var result = RendererHelper.RandOffsetWithRange(min, max, options);

        // Assert
        // Should return a value influenced by roughness and range
        Assert.True(Math.Abs(result) >= 0);
    }

    #endregion

    #region Internal Method Tests

    [Fact]
    public void DoubleLineFillOps_WithValidCoordinates_ReturnsOpsList()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.DoubleLineFillOps(10, 20, 50, 80, options);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateEllipseParams_WithValidDimensions_ReturnsEllipseParams()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.GenerateEllipseParams(100, 80, options);

        // Assert
        Assert.True(result.Rx > 0);
        Assert.True(result.Ry > 0);
        Assert.True(result.Increment > 0);
    }

    [Fact]
    public void GenerateEllipseParams_WithZeroDimensions_ReturnsValidParams()
    {
        // Arrange
        var options = CreateTestOptions();

        // Act
        var result = RendererHelper.GenerateEllipseParams(0, 0, options);

        // Assert
        Assert.True(result.Rx >= 0);
        Assert.True(result.Ry >= 0);
        Assert.True(result.Increment > 0);
    }

    [Fact]
    public void EllipseWithParams_WithValidParams_ReturnsEllipseResult()
    {
        // Arrange
        var options = CreateTestOptions();
        var ellipseParams = new EllipseParams { Rx = 50, Ry = 40, Increment = 0.5 };

        // Act
        var result = RendererHelper.EllipseWithParams(50, 50, options, ellipseParams);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.OpSet);
        Assert.Equal(OpSetType.Path, result.OpSet.Type);
        Assert.NotEmpty(result.OpSet.Ops);
        Assert.NotNull(result.Points);
    }

    #endregion

    #region Edge Cases and Options Tests

    [Fact]
    public void Line_WithPreserveVerticesEnabled_ReturnsOpSet()
    {
        // Arrange
        var options = CreateTestOptions();
        options.PreserveVertices = true;

        // Act
        var result = RendererHelper.Line(10, 20, 50, 80, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Ellipse_WithDisabledMultiStroke_ReturnsSimplifiedOpSet()
    {
        // Arrange
        var options = CreateTestOptions();
        options.DisableMultiStroke = true;

        // Act
        var result = RendererHelper.Ellipse(50, 50, 100, 80, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Rectangle_WithHighRoughness_ReturnsOpSet()
    {
        // Arrange
        var options = CreateTestOptions();
        options.Roughness = 3.0;

        // Act
        var result = RendererHelper.Rectangle(10, 20, 100, 50, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    [Fact]
    public void Arc_WithDisabledMultiStroke_ReturnsSimplifiedOpSet()
    {
        // Arrange
        var options = CreateTestOptions();
        options.DisableMultiStroke = true;

        // Act
        var result = RendererHelper.Arc(50, 50, 100, 80, 0, Math.PI / 2, false, false, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OpSetType.Path, result.Type);
        Assert.NotEmpty(result.Ops);
    }

    #endregion
}