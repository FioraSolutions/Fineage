/// <summary>
/// Local player ready to enter the world scene (after CharSelected / scene load).
/// </summary>
public sealed class DomainPlayerSpawned
{
    public int ObjectId;
    public EntityIdentity Identity;
    public PlayerStatus Status;
    public PlayerStats Stats;
    public PlayerAppearance Appearance;
}
