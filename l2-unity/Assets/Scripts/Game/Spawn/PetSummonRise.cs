using System.Collections;
using UnityEngine;

/// <summary>
/// Official APawn.SpawnEnterEvent. The packet byte 2 only starts it when
/// entereventgrp has a row for this npc. aCis writes 2 for every NpcInfo.
/// </summary>
public sealed class PetSummonRise : MonoBehaviour
{
    const float RiseSeconds = 1.8f;
    const float EffectLifeSeconds = 12f;

    bool _active;

    public static bool IsRising(GameObject go)
    {
        if (go == null)
            return false;
        PetSummonRise rise = go.GetComponent<PetSummonRise>();
        return rise != null && rise._active;
    }

    public static void TryBegin(Entity entity)
    {
        if (entity == null || entity.Identity == null || entity.Identity.SummonState != 2)
            return;
        if (!TablePacketServices.EnterEvents.TryGet(entity.Identity.NpcId, out EnterEventTable.Row row))
            return;
        if (row.SpawnType == 0)
            return;

        PetSummonRise rise = entity.GetComponent<PetSummonRise>();
        if (rise == null)
            rise = entity.gameObject.AddComponent<PetSummonRise>();
        if (rise._active)
            return;

        rise.Begin(entity, row);
    }

    void Begin(Entity entity, EnterEventTable.Row row)
    {
        float height = L2NameplateAnchor.CollisionHeightToUnityMeters(
            entity.Appearance != null ? entity.Appearance.CollisionHeight : 0f);
        Vector3 end = entity.transform.position;
        bool moves = row.IsRise == 1 || row.IsRise == 2;
        Vector3 start = end;
        if (row.IsRise == 1)
            start = end - Vector3.up * (2f * height);
        else if (row.IsRise == 2)
            start = end + Vector3.up * (2f * height);

        Debug.Log(
            "[ENTER_EVENT] table npc=" + entity.Identity.NpcId +
            " class=" + row.EffectClass +
            " rise=" + row.IsRise +
            " anim=" + row.AnimName +
            " rows=" + TablePacketServices.EnterEvents.Count);
        SpawnGroundEffect(row.EffectClass, end, entity.transform.rotation, entity);
        PlayEnterAnim(entity, row.AnimName);
        EntitySpawnShared.LogStriderAppear("enter", entity, "asked=" + row.AnimName);

        if (!moves)
        {
            entity.Identity.SummonState = 0;
            return;
        }

        _active = true;
        SetControllersEnabled(entity.gameObject, false);
        entity.transform.position = start;
        entity.Identity.Position = start;
        StartCoroutine(Rise(entity, start, end));
    }

    IEnumerator Rise(Entity entity, Vector3 start, Vector3 end)
    {
        float elapsed = 0f;
        while (elapsed < RiseSeconds)
        {
            if (entity == null)
                yield break;

            elapsed += Time.deltaTime;
            float u = Mathf.Clamp01(elapsed / RiseSeconds);
            Vector3 pos = Vector3.Lerp(start, end, u);
            entity.transform.position = pos;
            if (entity.Identity != null)
                entity.Identity.Position = pos;
            yield return null;
        }

        if (entity != null)
        {
            entity.transform.position = end;
            if (entity.Identity != null)
            {
                entity.Identity.Position = end;
                entity.Identity.SummonState = 0;
            }
            SetControllersEnabled(entity.gameObject, true);
            EntitySpawnShared.LogStriderAppear("risen", entity);
        }

        _active = false;
    }

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
