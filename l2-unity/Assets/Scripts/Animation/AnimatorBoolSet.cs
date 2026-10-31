using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animator get/set only when the parameter exists on the controller.
/// Village/monster/pawn controllers often omit player floats (run_speed) or bools (wait_1HS);
/// Unity logs a warning on every missing access.
/// </summary>
public static class AnimatorBoolSet
{
    static readonly Dictionary<long, Dictionary<int, AnimatorControllerParameterType>> Params =
        new Dictionary<long, Dictionary<int, AnimatorControllerParameterType>>();

    public static bool HasBool(Animator animator, string name) =>
        Has(animator, name, AnimatorControllerParameterType.Bool);

    public static bool HasFloat(Animator animator, string name) =>
        Has(animator, name, AnimatorControllerParameterType.Float);

    public static bool Has(Animator animator, string name, AnimatorControllerParameterType type)
    {
        if (animator == null || string.IsNullOrEmpty(name) || animator.runtimeAnimatorController == null)
            return false;

        Dictionary<int, AnimatorControllerParameterType> map = MapOf(animator);
        int hash = Animator.StringToHash(name);
        return map.TryGetValue(hash, out AnimatorControllerParameterType found) && found == type;
    }

    public static void SetIfExists(Animator animator, string name, bool value)
    {
        if (!HasBool(animator, name))
            return;
        if (animator.GetBool(name) == value)
            return;
        animator.SetBool(name, value);
    }

    static Dictionary<int, AnimatorControllerParameterType> MapOf(Animator animator)
    {
        long key = KeyOf(animator);
        if (Params.TryGetValue(key, out Dictionary<int, AnimatorControllerParameterType> map))
            return map;

        map = new Dictionary<int, AnimatorControllerParameterType>();
        AnimatorControllerParameter[] parameters = animator.parameters;
        for (int i = 0; i < parameters.Length; i++)
            map[parameters[i].nameHash] = parameters[i].type;

        Params[key] = map;
        return map;
    }

    static long KeyOf(Animator animator)
    {
        int animatorId = animator.GetInstanceID();
        int controllerId = animator.runtimeAnimatorController.GetInstanceID();
        return ((long)animatorId << 32) ^ (uint)controllerId;
    }
}
