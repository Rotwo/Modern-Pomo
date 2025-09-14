using System;

namespace modern_pomo.Helpers;

public static class TimeFormatter
{
    public static string FormatTimespan(TimeSpan time)
    {
        return time.ToString("mm\\:ss");
    }
}