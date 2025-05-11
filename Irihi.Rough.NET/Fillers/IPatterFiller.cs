using System.Drawing;
using Irihi.Rough.NET.DataModels;

namespace Irihi.Rough.NET.Fillers;

public interface IPatternFiller
{
    /// <summary>
    ///  Fills a polygon with a pattern.
    /// </summary>
    /// <param name="polygonList"> List of polygons that forms a complete shape </param>
    /// <param name="options"> Options for the fill operation </param>
    /// <returns> An OpSet representing the fill operation </returns>
    public OpSet FillPolygons(List<List<PointF>> polygonList, ResolvedOptions options);
}