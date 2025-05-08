using System.Drawing;

namespace Irihi.Rough.NET.Helpers;

public static class ColorHelper
{
    public static bool IsTransparent(this Color color)
    {
        return color.A == 0;
    }
}