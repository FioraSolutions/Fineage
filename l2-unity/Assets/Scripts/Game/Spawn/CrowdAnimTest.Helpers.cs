using UnityEngine;

public static partial class CrowdAnimTest
{
    public static void Apply(NetworkAnimationController controller)
    {
        EnsureTimeCap();
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
            // CullCompletely freezes the Animator state machine when the mesh is
            // briefly off-camera (common for CharInfo at screen edge / behind
            // camera). Result: run clip stuck mid-stride (normT frozen) while
            // MoveAllCharacters still translates the pawn. CullUpdateTransforms
            // keeps the state machine advancing and only skips writing bones.
            animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        }
    }
}
