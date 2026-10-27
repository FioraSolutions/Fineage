/// <summary>
/// Someone entered AOI / knownlist. Interlude DTOs stay as a bridge until spawn
/// paths accept wire-agnostic identity+stats payloads.
/// </summary>
public sealed class DomainKnownlistAdd
{
    public KnownlistKind Kind;
    public int ObjectId;

    public CharInfoDto InterludeChar;
    public NpcInfoDto InterludeNpc;
}
