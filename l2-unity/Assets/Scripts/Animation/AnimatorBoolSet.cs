using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets an Animator bool only when that parameter exists.
/// Village controllers name a state "wait" without a matching bool, and SetBool logs every call.
/// </summary>
public static class AnimatorBoolSet
{
    static readonly Dictionary<long, HashSet<int>> Bools = new Dictionary<long, HashSet<int>>();

    public static bool HasBool(Animator animator, string name)
    {
        if (animator == null || string.IsNullOrEmpty(name) || animator.runtimeAnimatorController == null)
            return false;

        long key = KeyOf(animator);
        HashSet<int> names;
        if (!Bools.TryGetValue(key, out names))
        {
            names = new HashSet<int>();
            AnimatorControllerParameter[] parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].type == AnimatorControllerParameterType.Bool)
                    names.Add(parameters[i].nameHash);
            }

            Bools[key] = names;
        }

        return names.Contains(Animator.StringToHash(name));
    }

    static readonly Dictionary<long, HashSet<int>> Floats = new Dictionary<long, HashSet<int>>();

    public static bool HasFloat(Animator animator, string name)
    {
        if (animator == null || string.IsNullOrEmpty(name) || animator.runtimeAnimatorController == null)
            return false;

        long key = KeyOf(animator);
        HashSet<int> names;
        if (!Floats.TryGetValue(key, out names))
        {
            names = new HashSet<int>();
            AnimatorControllerParameter[] parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].type == AnimatorControllerParameterType.Float)
                    names.Add(parameters[i].nameHash);
            }

            Floats[key] = names;
        }

        return names.Contains(Animator.StringToHash(name));
    }

    public static void SetIfExists(Animator animator, string name, bool value)
    {
        if (!HasBool(animator, name))
            return;
        if (animator.GetBool(name) == value)
            return;
        animator.SetBool(name, value);
    }

    static long KeyOf(Animator animator)
    {
        int animatorId = animator.GetInstanceID();
        int controllerId = animator.runtimeAnimatorController.GetInstanceID();
        return ((long)animatorId << 32) ^ (uint)controllerId;
    }
}
