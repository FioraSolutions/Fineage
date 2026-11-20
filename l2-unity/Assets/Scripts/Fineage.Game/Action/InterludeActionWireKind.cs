/// <summary>How the Fineage client handles a RequestActionUse id today.</summary>
public enum InterludeActionWireKind
{
    /// <summary>Sit/stand, walk/run, attack, next target.</summary>
    PlayerMovementCombat,
    Social,
    PartyCommand,
    TradeOrStore,
    PetSummonCommand,
    /// <summary>Known id but not implemented; use logs on click.</summary>
    NotImplemented,
}