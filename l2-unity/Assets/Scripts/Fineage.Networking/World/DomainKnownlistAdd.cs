/// <summary>
/// Someone entered AOI / knownlist. Users and NPCs carry identity plus
/// status, stats, and appearance. The sink rebuilds the wire DTO at apply time.
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

    public EntityIdentity NpcIdentity;
    public NpcStatusInterlude NpcStatus;
    public Stats NpcStats;
    public Appearance NpcAppearance;
}
