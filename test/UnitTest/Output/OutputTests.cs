using System.Drawing;
using System.Text;
using Irihi.Rough.NET;
using Irihi.Rough.NET.DataModels;

namespace UnitTest.Output;

public class OutputTests
{
    [Fact]
    public void GenerateAndSaveSvgFile()
    {
        const string pathString = "M23 12a11 11 0 1 1-22 0 11 11 0 0 1 22 0Zm-9.5 5.5a1.5 1.5 0 1 0-3 0 1.5 1.5 0 0 0 3 0ZM12 5a1.9 1.9 0 0 0-1.89 2l.3 5.5a1.59 1.59 0 0 0 3.17 0l.3-5.5c.07-1.09-.8-2-1.88-2Z";
        var generator = RoughConfiguration.CreateGenerator();
        var options = new Options
        {
            Fill = Color.White,
            Stroke = Color.Black
        };
        var drawable = generator.Path(pathString, options);

        var sb = new StringBuilder();
        foreach (var opSet in drawable.Sets)
        {
            opSet.GenerateGeometry();
            sb.Append(opSet.Geometry);
        }

        var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output", "svg");
        Directory.CreateDirectory(outputDirectory);

        var filePath = Path.Combine(outputDirectory, "icon.svg");
        File.WriteAllText(filePath,
            $"""
             <svg xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 24 24">
                 <path d="{sb}"/>
             </svg>
             """);

        Assert.True(File.Exists(filePath), "SVG 创建成功");
    }

    [Fact]
    public void GenerateAndSaveRectangleFile()
    {
        var generator = RoughConfiguration.CreateGenerator();
        var options = new Options
        {
            Fill = Color.White,
            Stroke = Color.Black
        };
        var drawable = generator.Rectangle(12, 12, 150, 100, options);

        var sb = new StringBuilder();
        foreach (var opSet in drawable.Sets)
        {
            opSet.GenerateGeometry();
            sb.Append(opSet.Geometry);
        }

        var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output", "svg");
        Directory.CreateDirectory(outputDirectory);

        var filePath = Path.Combine(outputDirectory, "rect.svg");
        File.WriteAllText(filePath,
            $"""
             <svg xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 200 200">
                 <path d="{sb}"/>
             </svg>
             """);

        Assert.True(File.Exists(filePath), "SVG 创建成功");
    }
}