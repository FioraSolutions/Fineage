using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public sealed class WeaponMeshTopologyEditModeTests
{
    [Test]
    public void All_weapongrp_weapon_meshes_are_volumetric()
    {
        LogAssert.Expect(
            LogType.Error,
            "[World] MetaTable WeapongrpTable: using non-DI empty instance; check GameLifetimeScope table registration");
        WeapongrpTable table = WeapongrpTable.Instance;
        table.Initialize();
        Assert.IsNotNull(table.WeaponGrps, "Weapongrp table failed to initialize");
        Assert.Greater(table.WeaponGrps.Count, 0, "Expected weapongrp rows");

        var keys = new HashSet<string>(System.StringComparer.Ordinal);
        foreach (KeyValuePair<int, Weapongrp> pair in table.WeaponGrps)
        {
            Weapongrp grp = pair.Value;
            if (grp == null)
                continue;
            foreach (string model in WeapongrpHandMeshes.EnumerateModelKeys(grp))
            {
                if (string.IsNullOrEmpty(model))
                    continue;
                if (!model.StartsWith("LineageWeapons.", System.StringComparison.Ordinal))
                    continue;
                keys.Add(model);
            }
        }

        Assert.Greater(keys.Count, 0, "Expected LineageWeapons keys in weapongrp");
        var failures = new List<string>();
        foreach (string key in keys.OrderBy(k => k, System.StringComparer.Ordinal))
        {
            string shortName = key.Substring("LineageWeapons.".Length);
            string fbxPath = "Assets/Resources/Data/Animations/LineageWeapons/Models/" + shortName + ".fbx";
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
            if (assets == null || assets.Length == 0)
                continue;

            bool sawMesh = false;
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is not Mesh mesh || mesh.vertexCount == 0)
                    continue;
                sawMesh = true;
                WeaponMeshTopology.Report report = WeaponMeshTopology.Analyze(mesh, key);
                if (report.IsDegenerate)
                    failures.Add(key + ": " + report.FailureReason);
            }

            if (!sawMesh)
                failures.Add(key + ": FBX has no mesh " + fbxPath);
        }

        Assert.AreEqual(
            0,
            failures.Count,
            "Degenerate (cardboard) weapon meshes in weapongrp: " + string.Join(" | ", failures));
    }
}