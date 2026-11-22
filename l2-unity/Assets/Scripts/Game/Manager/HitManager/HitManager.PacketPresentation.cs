using UnityEngine;

public partial class HitManager
{
    /// <summary>
    /// One row of the Attack hit list: per-hit soulshot, audio, optional impact FX, target reaction.
    /// Local player passes <paramref name="playColliderImpactFx"/> false — sword AttackShot drives impact FX.
    /// </summary>
    bool TryPresentMeleeHitFromAttackPacket(
        Entity attacker,
        Entity target,
        AttackDto packet,
        int hitIndex,
        bool playColliderImpactFx)
    {
        if (attacker == null || packet == null)
            return false;

        Hit hit = AttackHitPresentation.ResolveHit(packet, hitIndex);
        if (hit == null)
            return false;

        if (attacker is PlayerEntity player)
            player.SetSelfHit(hit);

        if (hit.isMiss())
            return true;

        if (target == null)
            target = ResolveEntityFromWorld(packet.TargetId);

        if (target == null || target == attacker)
            return true;

        target.SetDamage(hit.Damage);

        if (hit.hasSoulshot())
        {
            attacker.IsSoulshotCharged = true;
            WeaponChargePresentation.PlayMeleeSoulshotConsume(attacker);
        }
        else
            attacker.IsSoulshotCharged = false;

        CombatAudioHitContext.Set(attacker, hit.isCrit(), hit.isShielded());

        if (playColliderImpactFx)
        {
            Vector3 hitPoint = ResolveTargetImpactCenter(target);
            HandleHitCollider(attacker, attacker.transform, target, hitPoint, Vector3.zero);
        }

        target.PlayHitReaction(hit.isCrit());
        return true;
    }
}