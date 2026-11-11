using UnityEngine;

public partial class NpcEntity : NetworkEntity
{
    public void StartRunAnim(bool run)
    {
        PlayNpcLocomotion(run ? _run : _walk);
    }

    public void OnStartL2jMoving(bool walking)
    {
        PlayNpcLocomotion(walking ? _walk : _run);
    }

    private void StartWalkAnim(bool walking)
    {
        PlayNpcLocomotion(walking ? _walk : _run);
    }

    void PlayNpcLocomotion(string state)
    {
        if (_networkAnimationReceive == null || string.IsNullOrEmpty(state))
            return;

        Animator animator = _networkAnimationReceive.GetAnimator();
        int hash = Animator.StringToHash(state);
        if (animator != null && animator.HasState(0, hash))
        {
            // Npc_Basic still has bool-driven wait/walk/run transitions.
            // Clear them before direct CrossFade so a stale true bool cannot
            // pull the Animator back to the previous locomotion state.
            _networkAnimationReceive.SetBoolDisabledOther(state, false, allAnim);
            PlayerLocomotionCrossFade.TryPlay(_networkAnimationReceive, state);
            return;
        }

        _networkAnimationReceive.SetBoolDisabledOther(state, true, allAnim);
    }

    public override void OnStartMoving(bool walking)
    {
        base.OnStartMoving(walking);
        _networkAnimationReceive.SetAnimationProperty(walking ? (int)NpcAnimationEvent.Walk : (int)NpcAnimationEvent.Run, 1f);
    }

    public CharacterController GetCharacterController()
    {
        return _characterController;
    }
}
