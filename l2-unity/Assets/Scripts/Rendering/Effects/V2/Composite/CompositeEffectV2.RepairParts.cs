using System.Collections.Generic;
using UnityEngine;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    /// <summary>
    /// Hub-split #1209 broke SerializeReference load: _v2Parts empty at runtime while
    /// authored YAML on disk is fine. Rebuild from YAML exactly — do NOT rewrite
    /// spawnTiming / launchMode / attachment (those are the tuned cast→shoot→hit flow).
    /// </summary>
    protected void RepairEmptyV2PartsIfNeeded()
    {
        int currentSpawnable = CountSpawnableParts(_v2Parts);

        CompositePart[] fromYaml;
        if (CompositeEffectPrefabYamlParser.TryBuildPartsFromYaml(name, out fromYaml) &&
            fromYaml != null &&
            fromYaml.Length > 0)
        {
            int yamlSpawnable = CountSpawnableParts(fromYaml);
            if (yamlSpawnable > 0)
            {
                _v2Parts = fromYaml;
                Debug.Log(
                    "[ShotOrb] RepairParts from YAML go=" + name +
                    " parts=" + _v2Parts.Length +
                    " spawnable=" + yamlSpawnable +
                    " was=" + currentSpawnable);
                return;
            }
        }

        if (HasSpawnableV2Parts)
        {
            return;
        }

        if (!TryRepairFromResourcesFolder())
        {
            Debug.LogWarning(
                "[ShotOrb] RepairParts failed go=" + name +
                " — YAML/Resources had no rebuildable parts");
        }
    }

    static int CountSpawnableParts(CompositePart[] parts)
    {
        if (parts == null)
        {
            return 0;
        }

        int n = 0;
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] != null && parts[i].IsSpawnable)
            {
                n++;
            }
        }

        return n;
    }

    /// <summary>
    /// Last-resort only. Match common authored magician pattern:
    /// fl = OnAnimationShoot spawn + OnAnimationShoot launch + CasterCenter.
    /// </summary>
    bool TryRepairFromResourcesFolder()
    {
        string stem = ResolveEffectStemName();
        if (string.IsNullOrEmpty(stem))
        {
            return false;
        }

        string folder = "Data/Effects/" + stem;
        GameObject[] loaded = Resources.LoadAll<GameObject>(folder);
        if (loaded == null || loaded.Length == 0)
        {
            return false;
        }

        BaseEffect ca = null;
        BaseEffect pr = null;
        BaseEffect cs = null;
        BaseEffect fl = null;
        BaseEffect ta = null;
        List<BaseEffect> extraTa = new List<BaseEffect>();
        List<BaseEffect> extras = new List<BaseEffect>();

        for (int i = 0; i < loaded.Length; i++)
        {
            GameObject go = loaded[i];
            if (go == null || go.name.IndexOf("_composite", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                continue;
            }

            BaseEffect effect = go.GetComponent<BaseEffect>();
            if (effect == null)
            {
                effect = go.GetComponentInChildren<BaseEffect>(true);
            }

            if (effect == null)
            {
                continue;
            }

            string n = go.name;
            if (EndsWithToken(n, "_fl"))
                fl = effect;
            else if (EndsWithToken(n, "_ca"))
                ca = effect;
            else if (EndsWithToken(n, "_pr"))
                pr = effect;
            else if (EndsWithToken(n, "_cs"))
                cs = effect;
            else if (EndsWithToken(n, "_ta"))
            {
                if (ta == null)
                    ta = effect;
                else
                    extraTa.Add(effect);
            }
            else
                extras.Add(effect);
        }

        List<CompositePart> rebuilt = new List<CompositePart>(8);
        if (ca != null)
        {
            rebuilt.Add(new StationaryPart
            {
                name = ca.name,
                prefab = ca,
                scale = 1f,
                spawnTiming = CompositePartSpawnTiming.Immediate,
                spawnAttachmentPoint = EffectAttachmentPoint.CasterRoot,
                follow = true
            });
        }

        if (pr != null)
        {
            rebuilt.Add(new StationaryPart
            {
                name = pr.name,
                prefab = pr,
                scale = 1f,
                spawnTiming = CompositePartSpawnTiming.Immediate,
                spawnAttachmentPoint = EffectAttachmentPoint.CasterCenter,
                follow = true
            });
        }

        if (cs != null)
        {
            rebuilt.Add(new IndependentEffectPart
            {
                name = cs.name,
                prefab = cs,
                scale = 1f,
                spawnTiming = CompositePartSpawnTiming.OnAnimationShoot,
                spawnAttachmentPoint = EffectAttachmentPoint.CasterCenter,
                follow = true
            });
        }

        if (fl != null)
        {
            // Authored Wind/Prominence: spawn on anim shoot, launch on anim shoot, chest.
            rebuilt.Add(new ShotProjectilePart
            {
                name = fl.name,
                prefab = fl,
                scale = 1f,
                spawnTiming = CompositePartSpawnTiming.OnAnimationShoot,
                spawnAttachmentPoint = EffectAttachmentPoint.CasterCenter,
                follow = true,
                launchMode = ProjectileLaunchMode.OnAnimationShoot,
                impactType = ProjectileImpactType.EffectOnly,
                speed = 10f
            });
        }

        if (ta != null)
        {
            rebuilt.Add(new ImpactPart
            {
                name = ta.name,
                prefab = ta,
                scale = 1f,
                spawnTiming = CompositePartSpawnTiming.OnHitCollider,
                spawnAttachmentPoint = EffectAttachmentPoint.WorldHitPoint,
                follow = true
            });
        }

        for (int i = 0; i < extraTa.Count; i++)
        {
            rebuilt.Add(new IndependentEffectPart
            {
                name = extraTa[i].name,
                prefab = extraTa[i],
                scale = 1f,
                spawnTiming = CompositePartSpawnTiming.OnAnimationShoot,
                spawnAttachmentPoint = EffectAttachmentPoint.TargetCenter,
                follow = true
            });
        }

        for (int i = 0; i < extras.Count; i++)
        {
            rebuilt.Add(new IndependentEffectPart
            {
                name = extras[i].name,
                prefab = extras[i],
                scale = 1f,
                spawnTiming = CompositePartSpawnTiming.OnHitTime,
                spawnAttachmentPoint = EffectAttachmentPoint.TargetCenter,
                follow = true
            });
        }

        if (rebuilt.Count == 0)
        {
            return false;
        }

        _v2Parts = rebuilt.ToArray();
        Debug.Log(
            "[ShotOrb] RepairParts from Resources folder=" + folder +
            " parts=" + _v2Parts.Length +
            " fl=" + (fl != null));
        return true;
    }

    static bool EndsWithToken(string name, string token)
    {
        return name != null &&
               name.EndsWith(token, System.StringComparison.OrdinalIgnoreCase);
    }

    string ResolveEffectStemName()
    {
        string stem = name != null ? name.Replace("(Clone)", string.Empty).Trim() : string.Empty;
        const string suffix = "_composite";
        if (stem.EndsWith(suffix))
        {
            stem = stem.Substring(0, stem.Length - suffix.Length);
        }

        return stem;
    }
}
