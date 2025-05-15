using System.Drawing;
using Irihi.Rough.NET.DataModels;
using Irihi.Rough.NET.Dependencies.HachureFill;

namespace UnitTest.HachureFill;

public class HachureFillTests
{
    [Fact]
    public void RotatePoint_test()
    {
        var polygon = new RoughPolygon
        {
            new PointF(1, 1),
        };
        HachureFillFunctions.RotatePoints(polygon, new PointF(2, 10), 90);
        Assert.Equal(11, polygon[0].X);
        Assert.Equal(9, polygon[0].Y);
    }

    [Fact]
    public void Regular_HachureFill_Test()
    {
        var polygon = new RoughPolygon
        {
            new PointF(10, 10), new PointF(200, 10), new PointF(300, 100), new PointF(260, 150), new PointF(60, 200)
        };
        var fills = HachureFillFunctions.HachureLines([polygon], 30, 39);
        Assert.Equal(9, fills.Count);
    }

    [Fact]
    public void Regular_HachureFill_Test2()
    {
        var polygon = new RoughPolygon
        {
            new PointF(310, 260), new PointF(500, 260), new PointF(600, 350), new PointF(560, 400), new PointF(360, 450),
            new PointF(450, 300), new PointF(550, 360), new PointF(450, 400), new PointF(350, 360), new PointF(450, 300),
        };
        var fills = HachureFillFunctions.HachureLines([polygon], 30, 39);
        Assert.Equal(13, fills.Count);
    }

    [Fact]
    public void Regular_HachureFill_Test3()
    {
        var polygon = new RoughPolygon
        {
            new PointF(310, 260), new PointF(500, 260), new PointF(600, 350), new PointF(560, 400), new PointF(360, 450),
            new PointF(450, 300), new PointF(550, 360), new PointF(450, 400), new PointF(350, 360), new PointF(450, 300),
        };
        var fills = HachureFillFunctions.HachureLines([polygon], 12, 39);
        Assert.Equal(34, fills.Count);
    }

    [Fact]
    public void Regular_HachureFill_Test4()
    {
        var polygon = new RoughPolygon
        {
            new PointF(310, 10), new PointF(500, 10), new PointF(400, 100), new PointF(600, 100), new PointF(360, 200),
        };
        var fills = HachureFillFunctions.HachureLines([polygon], 11, 120);
        Assert.Equal(36, fills.Count);
    }
}