using System.Globalization;

/// <summary>
/// MinimapWnd <c>HandleUpdateGameTime</c> / <c>SelectSunOrMoon</c> (Interlude Interface.u).
/// </summary>
public static class InterludeMinimapGameTime
{
    public const int MinutesPerGameDay = 24 * 60;

    public static bool IsDaylightHour(int gameHour24)
    {
        return gameHour24 >= 6 && gameHour24 < 24;
    }

    public static string FormatHudLabel(int gameHour24, int gameMinute)
    {
        int hour12 = gameHour24;
        string suffix = gameHour24 >= 12 ? "PM " : "AM ";
        if (hour12 >= 12)
            hour12 -= 12;
        if (hour12 == 0)
            hour12 = 12;

        string hourPart = hour12 < 10
            ? "0" + hour12.ToString(CultureInfo.InvariantCulture) + " : "
            : hour12.ToString(CultureInfo.InvariantCulture) + " : ";
        string minutePart = gameMinute < 10
            ? "0" + gameMinute.ToString(CultureInfo.InvariantCulture)
            : gameMinute.ToString(CultureInfo.InvariantCulture);
        return suffix + hourPart + minutePart;
    }
}