using UnityEngine;

public partial class AnimationEventForwarder : MonoBehaviour
{
    [SerializeField] AnimationEventsBase _target;

    public void Bind(AnimationEventsBase target)
    {
        _target = target;
    }

    public static void BindAnimator(IAnimationController controller)
    {
        if (controller == null)
            return;

        Animator animator = controller.GetAnimator();
        AnimationEventsBase target = controller as AnimationEventsBase;
        if (animator == null || target == null)
            return;

        BindAnimator(animator, target);
    }

    public static void BindAnimator(Animator animator, AnimationEventsBase target)
    {
        if (animator == null || target == null)
            return;

        if (animator.gameObject == target.gameObject)
            return;

        if (animator.GetComponent<AnimationEventsBase>() != null)
            return;

        AnimationEventForwarder forwarder = animator.GetComponent<AnimationEventForwarder>();
        if (forwarder == null)
            forwarder = animator.gameObject.AddComponent<AnimationEventForwarder>();
        forwarder.Bind(target);
    }

    AnimationEventsBase Resolve()
    {
        if (_target == null)
            _target = GetComponentInParent<AnimationEventsBase>();
        return _target;
    }

    public void AttackShot(string animationName)
    {
        Resolve()?.AttackShot(animationName);
    }
}
