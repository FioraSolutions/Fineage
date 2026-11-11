using System.Collections;
using UnityEngine;

public sealed partial class PetSummonRise : MonoBehaviour
{
    static void SpawnGroundEffect(string effectClass, Vector3 feet, Quaternion rotation, Entity entity)
    {
        int npcId = entity != null && entity.Identity != null ? entity.Identity.NpcId : 0;
        EffectManager manager = GlobalEffectsPacketServices.Effects;
        int dbRows = manager != null ? manager.CountCommentOrPrefabMatches(effectClass) : 0;
        Debug.Log(
            "[ENTER_EVENT] database npc=" + npcId +
            " class=" + effectClass +
            " globalEffectRows=" + dbRows +
            " manager=" + (manager != null) +
            " registry=" + (manager != null ? manager.RegistryCount : 0));

        string comment = EnterEventTable.EffectCommentPrefix + effectClass;
        if (manager == null ||
            !manager.TryGetByComment(comment, out int effectId, out BaseEffect prefab, out EffectSettings settings))
        {
            Debug.Log(
                "[ENTER_EVENT] missing GlobalEffect comment=" + comment +
                " npc=" + npcId);
            return;
        }

        Debug.Log(
            "[ENTER_EVENT] GlobalEffect npc=" + npcId +
            " id=" + effectId +
            " comment=" + comment +
            " prefab=" + prefab.name);

        var anchor = new GameObject("summon-ground");
        anchor.transform.SetPositionAndRotation(feet, rotation);
        float radius = entity.Appearance != null ? entity.Appearance.CollisionRadius : 0f;
        float scale = radius * 52.5f / 10f;
        BaseEffect instance = Object.Instantiate(prefab, feet, rotation, anchor.transform);
        if (scale > 0.01f)
            instance.transform.localScale = Vector3.one * scale;
        instance.gameObject.SetActive(true);
        instance.Setup(settings, null, anchor.transform);
        instance.Play();
        int groups = anchor.GetComponentsInChildren<ParticleGroupV2>(true).Length;
        int renderers = anchor.GetComponentsInChildren<Renderer>(true).Length;
        Debug.Log(
            "[ENTER_EVENT] Play npc=" + npcId +
            " class=" + effectClass +
            " active=" + instance.gameObject.activeInHierarchy +
            " particleGroups=" + groups +
            " renderers=" + renderers +
            " pos=" + instance.transform.position +
            " scale=" + scale.ToString("0.###") +
            " settingsLife=" + (settings != null ? settings.defaultLifeTime.ToString("0.#") : "null"));
        Object.Destroy(anchor, EffectLifeSeconds);
    }

    static void PlayEnterAnim(Entity entity, string animName)
    {
        Animator animator = entity != null ? entity.GetComponentInChildren<Animator>() : null;
        if (animator == null)
            return;

        string state = ResolveState(animName);
        if (string.IsNullOrEmpty(state))
            return;
        if (!animator.HasState(0, Animator.StringToHash(state)))
            return;

        animator.Play(state, 0, 0f);
        animator.Update(0f);
    }

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
