using System.Drawing;
using Irihi.Rough.NET.DataModels;

namespace Irihi.Rough.NET.Fillers;

public interface IPatternFiller
{
    public OpSet FillPolygons(List<List<Point>> polygonList, ResolvedOptions options);
}