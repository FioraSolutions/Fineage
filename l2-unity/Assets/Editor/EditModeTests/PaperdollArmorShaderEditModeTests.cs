using System.IO;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Player race paperdoll armor under SysTextures/*/Materials must use ActorModulate2X (mechanism + CI).
/// </summary>
public sealed class PaperdollArmorShaderEditModeTests
{
    static readonly string[] Races =
    {
        "MFighter", "FFighter", "MMagic", "FMagic", "MElf", "FElf",
        "MShaman", "FShaman", "MOrc", "FOrc", "MDwarf", "FDwarf",
        "MDarkElf", "FDarkElf",
    };

    [Test]
    public void Every_shipped_paperdoll_armor_material_uses_actor_modulate2x()
    {
        int checkedCount = 0;
        for (int i = 0; i < Races.Length; i++)
        {
            string folder = "Data/SysTextures/" + Races[i] + "/Materials";
            Material[] mats = Resources.LoadAll<Material>(folder);
            for (int m = 0; m < mats.Length; m++)
            {
                Material mat = mats[m];
                if (mat == null || mat.shader == null)
                    continue;
                if (mat.name.Contains("_m00_ah") || mat.name.Contains("_m00_bh"))
                    continue;
                checkedCount++;
                Assert.AreEqual(
                    L2ActorMaterialSetup.ActorShaderName,
                    mat.shader.name,
                    "paperdoll mat " + Races[i] + "/" + mat.name);
            }
        }

        Assert.Greater(checkedCount, 32, "expected a scan across race armor materials");
    }

    [Test]
    public void Paperdoll_shader_ci_script_exists()
    {
        string script = Path.Combine(
            Application.dataPath,
            "..",
            "..",
            "scripts",
            "check-paperdoll-armor-shader.py");
        Assert.IsTrue(File.Exists(script));
    }
}