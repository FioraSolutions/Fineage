using UnityEngine;

public partial class NpcEntity : NetworkEntity
{
    public override void Initialize()
    {
        base.Initialize();
        _npcAnimationAudioHandler = GetComponent<CharacterAnimationAudioHandler>();
        _characterController = GetComponent<CharacterController>();

        EntityLoaded = true;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.Death, 1f, true);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        _npcAnimationAudioHandler.PlaySound(CharacterSoundEvent.Dmg);
    }

    public override void OnStopMoving()
    {
        if (_networkAnimationReceive.GetAnimationProperty((int)NpcAnimationEvent.Atk01) == 0f)
        {
            _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.Wait, 1f);
        }
        else
        {
            _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.Wait, 1f);
        }
    }

    public void OnStopL2jMoving()
    {
        PlayNpcLocomotion(_wait);
    }
}
