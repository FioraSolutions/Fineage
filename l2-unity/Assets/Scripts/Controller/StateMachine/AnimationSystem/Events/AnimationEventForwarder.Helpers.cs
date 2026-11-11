using UnityEngine;

public partial class AnimationEventForwarder : MonoBehaviour
{
    public void OnAnimationHit(string animationName)
    {
        Resolve()?.OnAnimationHit(animationName);
    }

    public void OnAnimationAttackHitEnd(string animationName)
    {
        Resolve()?.OnAnimationAttackHitEnd(animationName);
    }

    public void OnAnimationComplete(string animationName)
    {
        Resolve()?.OnAnimationComplete(animationName);
    }

    public void OnAnimationShoot(string animationName)
    {
        Resolve()?.OnAnimationShoot(animationName);
    }

    public void OnAnimationLoadArrow(string animationName)
    {
        Resolve()?.OnAnimationLoadArrow(animationName);
    }
}
