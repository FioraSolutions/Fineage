using UnityEngine;

/// <summary>
/// Wire-agnostic entry into the live Unity world. Interlude Incoming packets and
/// Fineage Net adapters both publish here; <see cref="WorldDomainSink"/> drives
/// <see cref="WorldPacketApply"/> / <see cref="World"/>.
/// </summary>
public interface IWorldDomainSink
{
    void ApplyMoveTo(DomainMoveTo move);
    void ApplyPositionCorrection(int objectId, Vector3 position);
    void ApplyStopMove(DomainStopMove stop);
    void ApplyKnownlistAdd(DomainKnownlistAdd add);
    void ApplyPlayerSpawned(DomainPlayerSpawned spawned);
    void ApplyStatsChanged(DomainStatsChanged changed);
    void FlushKnownlist();
}
