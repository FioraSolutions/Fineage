/// <summary>
/// Someone entered AOI / knownlist. Users carry identity and player data.
/// NPCs still carry the wire DTO until that family moves.
/// </summary>
public sealed class DomainKnownlistAdd
{
    public KnownlistKind Kind;
    public int ObjectId;

    public EntityIdentity UserIdentity;
    public PlayerStatus UserStatus;
    public PlayerStats UserStats;
    public PlayerAppearance UserAppearance;
    public bool UserAlikeDead;

    public NpcInfoDto InterludeNpc;
}
