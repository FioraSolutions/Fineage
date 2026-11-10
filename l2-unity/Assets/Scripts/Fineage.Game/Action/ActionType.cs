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
    SocialYes = 15,
    SocialNo = 16,
    SocialBow = 17,
    SocialUnaware = 18,
    SocialWaiting = 19,
    SocialLaugh = 20,
    SocialApplaud = 21,
    SocialDance = 22,
    SocialSorrow = 23,
    SocialCharm = 24,
    SocialShyness = 25,
    PrivateStoreBuy = 28,
    DwarvenManufacture = 37,
    Mount = 38,
    Recommend = 40,

    /// <summary>Legacy Fineage id — prefer <see cref="Mount"/> (Interlude 38).</summary>
    RideStrider = 38
}
