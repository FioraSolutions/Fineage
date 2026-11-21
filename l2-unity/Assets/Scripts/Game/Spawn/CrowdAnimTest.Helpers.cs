using UnityEngine;

public static partial class CrowdAnimTest
{
    public static void Apply(NetworkAnimationController controller)
    {
        if (controller == null)
            return;
        if (controller.GetComponent<UserEntity>() == null)
            return;
        ApplyGo(controller.gameObject);
    }

    static void ApplyGo(GameObject go)
    {
        if (go == null)
            return;

        CharacterController[] controllers = go.GetComponentsInChildren<CharacterController>(true);
        for (int i = 0; i < controllers.Length; i++)
        {
            CharacterController cc = controllers[i];
            if (cc != null)
                cc.enabled = false;
        }

        Animator[] animators = go.GetComponentsInChildren<Animator>(true);
        for (int i = 0; i < animators.Length; i++)
        {
            Animator animator = animators[i];
            if (animator == null)
                continue;
            animator.enabled = true;
            animator.speed = 1f;
        }

        KeepAnimatorUpdating(go);
    }

    // Prefabs ship with CullCompletely. That freezes the state machine (and the
    // clip time) whenever the skinned bounds leave the camera, including a bad
    // import bound that is off-screen while the mesh is still in view. The pawn
    // keeps its last pose until something else CrossFades. Players already get
    // this in Apply; monsters and NPCs were left on the prefab default.
    public static void KeepAnimatorUpdating(GameObject go)
    {
        if (go == null)
            return;

        Animator[] animators = go.GetComponentsInChildren<Animator>(true);
        for (int i = 0; i < animators.Length; i++)
        {
            Animator animator = animators[i];
            if (animator == null)
                continue;
            animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        }
    }
}
