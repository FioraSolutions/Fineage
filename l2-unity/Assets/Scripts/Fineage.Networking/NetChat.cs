using System;

public static partial class NetChat
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


    /// <summary>All tab also shows shout, tell, and announcements. Those types have no tab of their own.</summary>


    /// <summary><c>Name: text</c> in the Interlude color for <paramref name="chatType"/>.</summary>

}
