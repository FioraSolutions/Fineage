using System.Collections;
using UnityEngine;

public sealed partial class PetSummonRise : MonoBehaviour
{
    static string ResolveState(string animName)
    {
        if (string.IsNullOrEmpty(animName))
            return null;
        if (MonsterAnim.TryResolve(animName, out string state, out _))
            return state;

        string n = animName.ToLowerInvariant();
        if (n.StartsWith("spwait"))
            return "spwait";
        if (n.StartsWith("atkwait"))
            return MonsterAnim.AtkWait;
        if (n.StartsWith("spatk"))
            return MonsterAnim.SpAtk01;
        if (n.StartsWith("atk"))
            return MonsterAnim.Atk01;
        if (n.StartsWith("wait"))
            return MonsterAnim.Wait;
        return n;
    }

    static void SetControllersEnabled(GameObject go, bool enabled)
    {
        if (go == null)
            return;

        CharacterController[] controllers = go.GetComponentsInChildren<CharacterController>(true);
        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] != null)
                controllers[i].enabled = enabled;
        }
    }
}
