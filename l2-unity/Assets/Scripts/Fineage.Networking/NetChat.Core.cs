using System;

public static partial class NetChat
{
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

    public static bool ShowsOnAllTab(int chatType)
    {
        return chatType == All
            || chatType == Shout
            || chatType == Tell
            || chatType == Announcement
            || chatType == CriticalAnnounce;
    }
}
