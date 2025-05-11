using Irihi.Rough.NET.DataModels;

namespace Irihi.Rough.NET;

public interface IRoughRenderer
{
    /// <summary>
    ///  Get a random offset for the given value.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public double RandOffset(double x, ResolvedOptions options);
    
    /// <summary>
    ///  Get a random offset for the given value within a specified range.
    /// </summary>
    /// <param name="min"> The minimum value of the range.</param>
    /// <param name="max"> The maximum value of the range.</param>
    /// <param name="options"> The options to use for the random offset.</param>
    /// <returns> The random offset value.</returns>
    public double RandOffsetWithRange(double min, double max, ResolvedOptions options);
    /// <summary>
    ///  Get a operation set for drawing an ellipse.
    /// </summary>
    /// <param name="x"> The x-coordinate of the center of the ellipse.</param>
    /// <param name="y"> The y-coordinate of the center of the ellipse.</param>
    /// <param name="width"> The width of the ellipse.</param>
    /// <param name="height"> The height of the ellipse.</param>
    /// <param name="options"> The options to use for the ellipse.</param>
    /// <returns> The operation set for drawing the ellipse.</returns>
    public OpSet Ellipse(double x, double y, double width, double height, ResolvedOptions options);
    
    /// <summary>
    ///  Get a operation set for drawing a line as double line.
    /// </summary>
    /// <param name="x1"> The x-coordinate of the start point of the line.</param>
    /// <param name="y1"> The y-coordinate of the start point of the line.</param>
    /// <param name="x2"> The x-coordinate of the end point of the line.</param>
    /// <param name="y2"> The y-coordinate of the end point of the line.</param>
    /// <param name="options"> The options to use for the line.</param>
    /// <returns> The operation set for drawing the line.</returns>
    List<Op> DoubleLineOps(double x1, double y1, double x2, double y2, ResolvedOptions options);
}