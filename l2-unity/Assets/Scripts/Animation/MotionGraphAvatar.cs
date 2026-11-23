using System;
using UnityEngine;

/// <summary>
/// Imported L2 rigs have an Animator controller and no Avatar. The shared motion graph
/// binds stream handles, which need a valid generic avatar. Building that avatar is part
/// of binding the graph.
/// </summary>
public sealed class MotionGraphAvatar : MonoBehaviour
{
    Avatar _owned;

    public static void Ensure(Animator animator)
    {
        if (animator == null)
            return;
        if (animator.avatar != null && animator.avatar.isValid)
            return;

        bool enabled = animator.enabled;
        animator.enabled = false;
        Avatar built;
        try
        {
            built = AvatarBuilder.BuildGenericAvatar(animator.gameObject, "");
        }
        finally
        {
            animator.enabled = enabled;
        }

        if (built == null || !built.isValid)
        {
            DestroyAvatar(built);
            throw new InvalidOperationException(
                "[MotionPipeline] Animator '" + animator.name +
                "' has no Avatar — AvatarBuilder.BuildGenericAvatar did not produce a valid avatar, so the shared motion graph was not bound.");
        }

        built.hideFlags = HideFlags.HideAndDontSave;
        animator.avatar = built;
        animator.Rebind();

        MotionGraphAvatar host = animator.GetComponent<MotionGraphAvatar>();
        if (host == null)
            host = animator.gameObject.AddComponent<MotionGraphAvatar>();
        host._owned = built;
    }

    void OnDestroy()
    {
        if (_owned == null)
            return;
        Avatar owned = _owned;
        _owned = null;
        DestroyAvatar(owned);
    }

    static void DestroyAvatar(Avatar avatar)
    {
        if (avatar == null)
            return;
        if (Application.isPlaying)
            UnityEngine.Object.Destroy(avatar);
        else
            UnityEngine.Object.DestroyImmediate(avatar);
    }
}
