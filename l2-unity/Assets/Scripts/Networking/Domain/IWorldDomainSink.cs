/// <summary>
/// Wire-agnostic entry into the live Unity world. Interlude Incoming packets and a
/// future v2 ZoneServer adapter both publish here; <see cref="WorldDomainSink"/>
/// still drives today's <see cref="WorldPacketApply"/> / <see cref="World"/>.
/// </summary>
public interface IWorldDomainSink
{
    void ApplyMoveTo(DomainMoveTo move);
    void ApplyStopMove(DomainStopMove stop);
    void ApplyKnownlistAdd(DomainKnownlistAdd add);
    void ApplyPlayerSpawned(DomainPlayerSpawned spawned);
    void ApplyStatsChanged(DomainStatsChanged changed);
    void FlushKnownlist();
}
