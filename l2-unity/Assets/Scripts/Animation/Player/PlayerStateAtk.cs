using UnityEngine;


public class PlayerStateAtk : PlayerStateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        if (!_enabled)
        {
            return;
        }

        AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(0);
        if (clipInfos == null || clipInfos.Length == 0)
        {
            clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        }

        AnimationPacketServices.PlayerAnimation?.UpdateAnimatorAtkSpdMultiplier(clipInfos[0].clip.length);

        SetBool("atk01", true, false, false);

        if (_audioHandler == null)
        {
            int entityId = _entity != null && _entity.Identity != null ? _entity.Identity.Id : 0;
            FineageLog.Error(
                FineageLogCategory.World,
                "PlayerStateAtk: CharacterAnimationAudioHandler missing entityId=" + entityId
                + " event=Atk_1H");
            if (_entity is PlayerEntity player)
                CombatSwingAudio.PlayLocalPlayerWeaponSwing(player);
            return;
        }

        PlaySoundAtRatio(CharacterSoundEvent.Atk_1H, _audioHandler.AtkRatio);
        PlayEquippedWeaponItemSounds();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_enabled)
        {
            return;
        }

        SetBool("atkwait", true, false, false);
        SetBool("atk01", true, false, false);
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_enabled)
        {
            return;
        }
    }
}
