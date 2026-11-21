using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// HUD chrome must resolve Interlude CH3 textures and must not reference HF frame packages.
/// </summary>
public sealed class InterludeHudSkinEditModeTests
{
    [Test]
    public void Catalog_textures_load_and_are_not_high_five_chrome()
    {
        foreach (InterludeHudWindowEntry entry in InterludeHudWindowCatalog.AllEntries())
        {
            Assert.IsFalse(string.IsNullOrEmpty(entry.XdatName));

            for (int i = 0; i < entry.TextureResourcePaths.Length; i++)
            {
                string path = entry.TextureResourcePaths[i];
                Assert.IsFalse(
                    InterludeHudWindowCatalog.IsHighFiveChromeResourcePath(path),
                    entry.XdatName + " must not reference HF chrome path '" + path + "'.");

                Texture2D tex = Resources.Load<Texture2D>(path);
                Assert.IsNotNull(
                    tex,
                    entry.XdatName + " missing Resources texture '" + path + "'.");
            }
        }
    }

    [Test]
    public void Skin_pipeline_bans_windows_df_on_hud_entries()
    {
        var offenders = new List<string>();
        foreach (InterludeHudWindowEntry entry in InterludeHudWindowCatalog.AllEntries())
        {
            for (int i = 0; i < entry.TextureResourcePaths.Length; i++)
            {
                string path = entry.TextureResourcePaths[i];
                if (path.Contains("Windows_DF") || path.Contains("Frame_DF"))
                    offenders.Add(entry.XdatName + ": " + path);
            }
        }

        CollectionAssert.IsEmpty(
            offenders,
            "HUD catalog still lists later-chronicle frame textures.");
    }

    [Test]
    public void Vertical_frame_constants_use_interlude_smallwnd_not_windows_df()
    {
        Assert.AreEqual(
            InterludeHudWindowCatalog.SmallWndBack1,
            L2InterludeHudVerticalFrame.TopResource);
        Assert.AreEqual(
            InterludeHudWindowCatalog.SmallWndBack2,
            L2InterludeHudVerticalFrame.CenterResource);
        Assert.AreEqual(
            InterludeHudWindowCatalog.SmallWndBack3Ex,
            L2InterludeHudVerticalFrame.BottomResource);
        Assert.AreEqual(
            InterludeHudWindowCatalog.SmallWndBack2,
            L2InterludeHudVerticalFrame.DragRailResource);

        Assert.IsFalse(
            InterludeHudWindowCatalog.IsHighFiveChromeResourcePath(
                L2InterludeHudVerticalFrame.DragRailResource));
    }

    [Test]
    public void Chat_input_rect_matches_xdat_bottom_strip()
    {
        InterludeHudControlRect bar = InterludeHudWindowCatalog.ChatInputBar;
        Assert.AreEqual(22f, bar.Left, 0.01f);
        Assert.AreEqual(6f, bar.Right, 0.01f);
        Assert.AreEqual(13f, bar.Bottom, 0.01f);
        Assert.AreEqual(18f, bar.Height, 0.01f);
    }
}