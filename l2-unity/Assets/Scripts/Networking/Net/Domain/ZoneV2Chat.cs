using System;

/// <summary>
/// Zone chat helpers that match Fineage-Backend <c>ChatProtocol</c> and the client <c>ChatTypes</c> colors.
/// Say is opcode 8. CreatureSay is 1012. SystemMessage is 1040.
/// </summary>
public static class ZoneV2Chat
{
    public const int All = 0;
    public const int Shout = 1;
    public const int Tell = 2;
    public const int Party = 3;
    public const int Clan = 4;
    public const int Trade = 8;
    public const int Alliance = 9;
    public const int Announcement = 10;
    public const int CriticalAnnounce = 18;

    /// <summary>Server rejects raw text longer than this. See <c>ChatProtocol.MaxTextLength</c>.</summary>
    public const int MaxTextLength = 100;

    /// <summary>Above <see cref="MaxTextLength"/> so the zone can answer an over-long say with <c>bad_request</c>.</summary>
    public const int InputLimit = 128;

    public const string SystemColor = "#E6D38A";

    /// <summary>
    /// A leading Interlude prefix selects the type and is removed. No prefix keeps <paramref name="fallbackType"/>
    /// (the open chat tab; All is 0). Tell keeps the name in the text: Say has no whisper-target key.
    /// </summary>
    public static void Split(string raw, int fallbackType, out string text, out int chatType)
    {
        text = raw ?? "";
        chatType = fallbackType;
        if (text.Length == 0)
            return;

        int typed = TypeForPrefix(text[0]);
        if (typed < 0)
            return;

        chatType = typed;
        text = text.Substring(1);
    }

    public static int TypeForPrefix(char prefix)
    {
        switch (prefix)
        {
            case '!':
                return Shout;
            case '"':
                return Tell;
            case '#':
                return Party;
            case '@':
                return Clan;
            case '+':
                return Trade;
            case '$':
                return Alliance;
            default:
                return -1;
        }
    }

    /// <summary>All tab also shows shout, tell, and announcements. Those types have no tab of their own.</summary>
    public static bool ShowsOnAllTab(int chatType)
    {
        return chatType == All
            || chatType == Shout
            || chatType == Tell
            || chatType == Announcement
            || chatType == CriticalAnnounce;
    }

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

    /// <summary><c>Name: text</c> in the Interlude color for <paramref name="chatType"/>.</summary>
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
