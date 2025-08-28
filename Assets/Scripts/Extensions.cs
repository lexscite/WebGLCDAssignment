using System;

namespace WebGLCD
{
public static class Extensions
{
    public static string ToPrettyString(this TimeSpan ts)
    {
        return ts.TotalMilliseconds < 1000 ? $"{ts.TotalMilliseconds:F0}ms" : $"{ts.TotalSeconds:F2}s";
    }
}
}