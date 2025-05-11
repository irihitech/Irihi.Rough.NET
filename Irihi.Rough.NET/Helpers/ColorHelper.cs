using System.Drawing;
using System.Runtime.CompilerServices;

namespace Irihi.Rough.NET.Helpers;

public static class ColorHelper
{
    /// <summary>
    ///  Checks if the color is transparent.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsTransparent(this Color color)
    {
        return color.A == 0;
    }
}