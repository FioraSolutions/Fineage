using System;
using UnityEngine;
using VContainer;

public partial class PlayerEntity : Entity
{
    protected override void OnDeath()
    {
        base.OnDeath();
        PlayerStatePacketServices.PlayerState.NotifyEvent(Event.DEAD);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        _characterAnimationAudioHandler.PlaySound(CharacterSoundEvent.Dmg);
    }

    public string GetCurrentAnimName()
    {
        return _gear.WeaponAnim;
    }

    public GameObject GetGoEtcItem()
    {
        return _gear.GetGoEtcItem();
    }
}
