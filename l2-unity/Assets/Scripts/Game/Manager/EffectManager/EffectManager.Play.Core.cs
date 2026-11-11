using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public partial class EffectManager
{
    public void PlayEffect(int id, Transform target, MagicCastData castData = null)
    {
        bool windstrike = id == 4247;
        // PlayEffect parents under target — container is only for impact proxies.
        EnsureActiveEffectsContainer();

        if (windstrike)
        {
            Debug.Log(
                "[WINDSTRIKE] EffectManager registry=" + _registryBuilt +
                " count=" + _byId.Count +
                " container=" + (_activeEffectsContainer != null) +
                " target=" + (target != null ? target.name : "null"));
        }

        RuntimeEffect entry;
        bool found;
        using (MagicSkillUseTrace.Measure("effect.find"))
            found = TryGetRuntime(id, out entry);

        BaseEffect prefab;
        using (MagicSkillUseTrace.Measure("effect.prefab"))
            prefab = ResolvePrefab(found ? entry : default, found, target);

        if (!found || prefab == null || target == null)
        {
            Debug.LogWarning(
                "[EffectManager] abort id=" + id +
                " found=" + found +
                " prefab=" + (prefab != null ? prefab.name : "null") +
                " target=" + (target != null) +
                " comment=" + (found ? entry.Comment : ""));
            return;
        }

        if (windstrike)
            Debug.Log("[WINDSTRIKE] EffectManager spawn prefab=" + prefab.name);

        BaseEffect instance;
        using (MagicSkillUseTrace.Measure("effect.instantiate"))
            instance = Instantiate(prefab, target.position, target.rotation, target);
        using (MagicSkillUseTrace.Measure("effect.inject"))
            InjectSpawned(instance);

        instance.gameObject.SetActive(true);
        using (MagicSkillUseTrace.Measure("effect.setup"))
            instance.Setup(entry.Settings, castData, target);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // EffectDoublePlayLog.TrackManagerPlay(id, prefab != null ? prefab.name : "null");
#endif
        using (MagicSkillUseTrace.Measure("effect.play"))
            instance.Play();
        if (windstrike)
            Debug.Log("[WINDSTRIKE] EffectManager.Play returned instance=" + instance.name);
    }

    BaseEffect ResolvePrefab(RuntimeEffect entry, bool found, Transform target)
    {
        if (!found)
            return null;

        if (entry.UseWeaponShot)
        {
            Gear gear = target != null ? target.GetComponentInParent<Gear>() : null;
            BaseEffect listed = TablePacketServices.WeaponShots.ResolvePrefab(gear, entry.SpiritShot);
            if (listed != null)
                return listed;
        }

        return entry.Prefab;
    }
}
