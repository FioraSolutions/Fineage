/// <summary>
/// Interlude <c>RequestActionUse</c> / ActionWnd ids (interface.xdat ActionWnd + ActionName).
/// Icons live under <c>Data/UI/Window/Action/Row/button_img</c>.
/// </summary>
public enum ActionType : int
{
    Sit = 0,
    WalkRun = 1,
    Attack = 2,
    Trade = 3,
    NextTarget = 4,
    Pickup = 5,
    Assist = 6,
    PartyInvite = 7,
    PartyLeave = 8,
    PartyKick = 9,
    PrivateStoreSell = 10,
    PartyMatching = 11,
    SocialGreeting = 12,
    SocialVictory = 13,
    SocialAdvance = 14,
    /// <summary>Interlude RequestActionUse 24 → SocialAction.YES. 15 is level-up, not this emote.</summary>
    SocialYes = 24,
    SocialNo = 25,
    SocialBow = 26,
    SocialUnaware = 29,
    SocialWaiting = 30,
    SocialLaugh = 31,
    SocialApplaud = 33,
    SocialDance = 34,
    SocialSorrow = 35,
    SocialCharm = 62,
    SocialShyness = 66,
    PrivateStoreBuy = 28,
    DwarvenManufacture = 37,
    Mount = 38,
    Recommend = 40,
    /// <summary>Interlude ActionWnd party row: change party leader.</summary>
    PartyLeader = 50,

    /// <summary>Legacy Fineage id — prefer <see cref="Mount"/> (Interlude 38).</summary>
    RideStrider = 38
}
