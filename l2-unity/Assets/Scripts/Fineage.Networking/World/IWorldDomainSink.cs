using UnityEngine;

/// <summary>
/// Wire-agnostic entry into the live Unity world. Interlude Incoming packets and
/// Fineage Net adapters both publish here. The Assembly-CSharp sink applies them.
/// </summary>
public interface IWorldDomainSink
{
    void ApplyMoveTo(DomainMoveTo move);
    void ApplyPositionCorrection(int objectId, Vector3 position, int heading = -1);
    void ApplyStopMove(DomainStopMove stop);
    void ApplyKnownlistAdd(DomainKnownlistAdd add);
    void ApplyPlayerSpawned(DomainPlayerSpawned spawned);
    void ApplyStatsChanged(DomainStatsChanged changed);
    void FlushKnownlist();
}
