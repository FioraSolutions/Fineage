using UnityEngine;

public partial class HitManager
{
    /// <summary>
    /// One row of the Attack hit list: label, soulshot, audio, optional impact FX, target reaction.
    /// Every row of the packet uses this path, at that row's hit time.
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
        {
            Debug.LogError(
                "[Attack] missing hit row index=" + hitIndex +
                " attackerId=" + (attacker.Identity != null ? attacker.Identity.Id.ToString() : "0"));
            return false;
        }

        if (attacker is PlayerEntity player)
            player.SetSelfHit(hit);

        if (target == null)
            target = ResolveEntityFromWorld(hit.TargetId);
        if (target == null)
            target = ResolveEntityFromWorld(packet.TargetId);

        int hitCount = AttackHitPresentation.CountHits(packet);
        AttackHitCue cue = AttackHitWireRows.CueFor(hit, hitIndex, hitCount > 0 ? hitCount : 1);
        if (string.IsNullOrEmpty(cue.Label))
        {
            Debug.LogError(
                "[Attack] missing hit label index=" + hitIndex +
                " attackerId=" + (attacker.Identity != null ? attacker.Identity.Id.ToString() : "0"));
        }
        else if (target != null)
            AttackHitLabel.Show(target, cue);
        else
        {
            Debug.LogError(
                "[Attack] hit text missing target id=" + hit.TargetId +
                " index=" + hitIndex);
        }

        if (hit.hasSoulshot())
        {
            attacker.IsSoulshotCharged = true;
            WeaponChargePresentation.PlayMeleeSoulshotConsume(attacker);
        }
        else
            attacker.IsSoulshotCharged = false;

        if (hit.isMiss())
            return true;

        if (target == null || target == attacker)
            return true;

        target.SetDamage(hit.Damage);

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