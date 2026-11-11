using System;

public static partial class NetChat
{
    public static string ColorFor(int chatType)
    {
        switch (chatType)
        {
            case 0:
                return "#dcd9dc";
            case 1:
                return "#ffb266";
            case 2:
                return "#ff6bdc";
            case 3:
                return "#4cff4c";
            case 4:
                return "#66ccff";
            case 5:
                return "#ffff66";
            case 6:
                return "#b3b3ff";
            case 7:
                return "#ff9933";
            case 8:
                return "#ffd700";
            case 9:
                return "#66ffff";
            case 10:
                return "#00ffff";
            case 11:
                return "#80ccff";
            case 12:
                return "#99ff99";
            case 13:
                return "#4de6ff";
            case 14:
                return "#ccccff";
            case 15:
                return "#ff9999";
            case 16:
                return "#9999ff";
            case 17:
                return "#ffd700";
            case 18:
                return "#ff0000";
            default:
                return "#dcd9dc";
        }
    }

    public static string FormatLine(string name, string text, int chatType)
    {
        string who = name ?? "";
        if (chatType == Announcement || chatType == CriticalAnnounce)
            who = "Announcements";
        return "<color=" + ColorFor(chatType) + ">" + who + ": " + (text ?? "") + "</color>";
    }

    public static string FormatSystem(string text)
    {
        return "<color=" + SystemColor + ">" + (text ?? "") + "</color>";
    }
}
