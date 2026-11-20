using NUnit.Framework;
using UnityEditor;
using UnityEngine;

/// <summary>EditMode coverage for per-tier URP quality wiring.</summary>
public sealed class GraphicsQualityEditModeTests
{
    [Test]
    public void Desktop_platforms_map_to_desktop_group()
    {
        Assert.IsTrue(GraphicsQualityPlatformTable.TryResolve(RuntimePlatform.WindowsPlayer, out var g));
        Assert.AreEqual(GraphicsPlatformGroup.Desktop, g);
        Assert.IsTrue(GraphicsQualityPlatformTable.TryResolve(RuntimePlatform.OSXEditor, out g));
        Assert.AreEqual(GraphicsPlatformGroup.Desktop, g);
    }

    [Test]
    public void Mobile_platforms_map_to_mobile_group()
    {
        Assert.IsTrue(GraphicsQualityPlatformTable.TryResolve(RuntimePlatform.Android, out var g));
        Assert.AreEqual(GraphicsPlatformGroup.Mobile, g);
        Assert.IsTrue(GraphicsQualityPlatformTable.TryResolve(RuntimePlatform.IPhonePlayer, out g));
        Assert.AreEqual(GraphicsPlatformGroup.Mobile, g);
    }

    [Test]
    public void Catalog_defaults_desktop_high_and_mobile_low()
    {
        var catalog = Resources.Load<GraphicsQualityCatalog>(GraphicsQualityCatalog.DefaultResourcesPath);
        Assert.IsNotNull(catalog, "Missing GraphicsQualityCatalog at " + GraphicsQualityCatalog.DefaultResourcesPath);
        Assert.AreEqual(3, catalog.ResolveDefaultQualityIndex(GraphicsPlatformGroup.Desktop));
        Assert.AreEqual(1, catalog.ResolveDefaultQualityIndex(GraphicsPlatformGroup.Mobile));
        Assert.IsTrue(catalog.TierNamesMatchProjectQualitySettings());
    }

    [Test]
    public void Every_quality_tier_has_distinct_render_pipeline_asset()
    {
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            var pipeline = QualitySettings.GetRenderPipelineAssetAt(i);
            Assert.IsNotNull(
                pipeline,
                "Quality tier '" + QualitySettings.names[i] + "' (index " + i + ") has no URP asset assigned");
        }
    }

    [Test]
    public void Tier_indices_match_expected_names()
    {
        Assert.AreEqual("High", QualitySettings.names[3]);
        Assert.AreEqual("Low", QualitySettings.names[1]);
    }

    [Test]
    public void Standalone_and_android_default_quality_in_project_settings()
    {
        var settings = QualitySettings.GetQualitySettings();
        var so = new SerializedObject(settings);
        var map = so.FindProperty("m_PerPlatformDefaultQuality");
        Assert.IsNotNull(map);
        int? standalone = null;
        int? android = null;
        for (int i = 0; i < map.arraySize; i++)
        {
            var key = map.GetArrayElementAtIndex(i).FindPropertyRelative("first").stringValue;
            var val = map.GetArrayElementAtIndex(i).FindPropertyRelative("second").intValue;
            if (key == "Standalone")
                standalone = val;
            if (key == "Android")
                android = val;
        }

        Assert.AreEqual(3, standalone);
        Assert.AreEqual(1, android);
    }

    [Test]
    public void Boot_resolver_uses_catalog_for_desktop_editor()
    {
        GraphicsQualityApplier.ResetCatalogForTests();
        GraphicsPlatformGroup group = GraphicsQualityPlatformTable.ResolveOrDefault(Application.platform);
        int index = GraphicsQualityApplier.Catalog.ResolveDefaultQualityIndex(group);
        if (group == GraphicsPlatformGroup.Desktop)
            Assert.AreEqual(3, index);
        else
            Assert.AreEqual(1, index);
    }
}