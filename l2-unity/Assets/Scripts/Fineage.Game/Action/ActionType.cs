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
    RideStrider = 11,
    /// <summary>Mobius RequestActionUse social: Greeting.</summary>
    SocialGreeting = 12,
    /// <summary>Mobius RequestActionUse social: Victory.</summary>
    SocialVictory = 13,
    /// <summary>Mobius RequestActionUse social: Advance.</summary>
    SocialAdvance = 14
}