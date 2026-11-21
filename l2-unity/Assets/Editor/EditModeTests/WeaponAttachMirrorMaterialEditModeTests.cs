using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class WeaponAttachMirrorMaterialEditModeTests
{
    [Test]
    public void MirrorApply_leaves_no_single_sided_cull_slots()
    {
        var grp = new Weapongrp
        {
            Model = "LineageWeapons.demon_splinter_m00_wp",
            ModelLeft = "LineageWeapons.demon_splinter_m00_wp",
            WeaponType = WeaponType.dualfist,
        };
        Assert.IsTrue(WeapongrpHandMeshes.ShouldMirrorLeftHand(grp));

        GameObject prefab = Resources.Load<GameObject>("Data/Animations/LineageWeapons/demon_splinter_m00_wp");
        if (prefab == null)
        {
            Assert.Inconclusive("demon_splinter_m00_wp prefab missing in workspace");
            return;
        }

        GameObject instance = Object.Instantiate(prefab);
        var mount = new WeaponAttachMount(WeaponAttachSocket.LeftHand, grp.Model, mirrorScaleX: true);
        var socket = new GameObject("Weapon_L_Bone").transform;
        WeaponAttachApplicator.Apply(instance, socket, mount);

        IList<string> offenders = CollectFlatMirrorSlots(instance);
        Assert.AreEqual(
            0,
            offenders.Count,
            "Mirrored attach must double-side every submesh; offenders: " + string.Join(", ", offenders));
        Object.DestroyImmediate(socket.gameObject);
        Object.DestroyImmediate(instance);
    }

    [Test]
    public void Audit_all_LineageWeapons_prefabs_that_mirror_in_weapongrp()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Data/Animations/LineageWeapons");
        if (prefabs == null || prefabs.Length == 0)
        {
            Assert.Inconclusive("No LineageWeapons prefabs in workspace");
            return;
        }

        var mirrorKeys = CollectMirrorModelKeysFromMeta();
        int audited = 0;
        var failures = new List<string>();
        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject prefab = prefabs[i];
            if (prefab == null)
                continue;
            string key = "LineageWeapons." + prefab.name;
            if (!mirrorKeys.Contains(key))
                continue;
            audited++;

            GameObject instance = Object.Instantiate(prefab);
            Transform socket = new GameObject("socket").transform;
            var mount = new WeaponAttachMount(WeaponAttachSocket.LeftHand, key, mirrorScaleX: true);
            WeaponAttachApplicator.Apply(instance, socket, mount);
            IList<string> offenders = CollectFlatMirrorSlots(instance);
            if (offenders.Count > 0)
                failures.Add(key + " -> " + string.Join(", ", offenders));
            Object.DestroyImmediate(socket.gameObject);
            Object.DestroyImmediate(instance);
        }

        Assert.Greater(audited, 0, "Expected at least one mirror-profile weapon prefab in meta");
        Assert.AreEqual(
            0,
            failures.Count,
            "Weapon mirror audit failures (" + failures.Count + "): " + string.Join(" | ", failures));
    }

    static HashSet<string> CollectMirrorModelKeysFromMeta()
    {
        var keys = new HashSet<string>(System.StringComparer.Ordinal);
        LogAssert.ignoreFailingMessages = true;
        WeapongrpTable table = WeapongrpTable.Instance;
        table.Initialize();
        if (table.WeaponGrps == null)
            return keys;
        foreach (KeyValuePair<int, Weapongrp> pair in table.WeaponGrps)
        {
            Weapongrp grp = pair.Value;
            if (grp == null || !WeapongrpHandMeshes.ShouldMirrorLeftHand(grp))
                continue;
            foreach (string model in WeapongrpHandMeshes.EnumerateModelKeys(grp))
                keys.Add(model);
        }
        LogAssert.ignoreFailingMessages = false;
        return keys;
    }

    static IList<string> CollectFlatMirrorSlots(GameObject root)
    {
        var offenders = new List<string>();
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        for (int r = 0; r < renderers.Length; r++)
        {
            Renderer renderer = renderers[r];
            if (renderer == null)
                continue;
            Material[] mats = renderer.sharedMaterials;
            for (int m = 0; m < mats.Length; m++)
            {
                Material mat = mats[m];
                if (mat == null)
                    continue;
                if (!WeaponAttachMirrorMaterials.IsMirrorSafeAfterRuntimeFix(mat))
                {
                    offenders.Add(renderer.gameObject.name + "[submesh " + m + "] material=" + mat.name);
                }
                else if (mat.HasProperty("_Cull") && mat.GetFloat("_Cull") > 0.5f)
                {
                    offenders.Add(renderer.gameObject.name + "[submesh " + m + "] cull=" + mat.GetFloat("_Cull"));
                }
            }
        }
        return offenders;
    }
}