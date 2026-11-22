using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

/// <summary>
/// The runtime layout asset places controls from the extracted window documents.
/// </summary>
public sealed class InterludeWindowLayoutEditModeTests
{
    [SetUp]
    public void SetUp()
    {
        InterludeWindowLayout.ResetForEditModeTests();
    }

    [Test]
    public void Status_bars_match_extracted_rects()
    {
        LogAssert.Expect(LogType.Error, new Regex("StatusWnd_LevelTextBox.*fontType 6"));
        LogAssert.Expect(LogType.Error, new Regex("UserName.*height"));
        VisualElement host = new VisualElement();
        Assert.IsTrue(InterludeWindowLayout.Apply(host, "StatusWnd"));
        Assert.AreEqual(176f, host.style.width.value.value, 0.01f);
        Assert.AreEqual(84f, host.style.height.value.value, 0.01f);
        Assert.AreEqual(0f, host.style.left.value.value, 0.01f);
        Assert.AreEqual(0f, host.style.top.value.value, 0.01f);

        ExpectRect(host, "CPBar", 16f, 27f, 150f, 12f);
        ExpectRect(host, "HPBar", 16f, 41f, 150f, 12f);
        ExpectRect(host, "MPBar", 16f, 55f, 150f, 12f);
        ExpectRect(host, "EXPBar", 16f, 69f, 150f, 12f);
        ExpectRect(host, "StatusWndRightTex", 168f, 0f, 4f, 84f);
        Assert.IsNotNull(host.Q("CPBar__fill"));
        Assert.IsNotNull(host.Q("StatusWnd_LevelTextBox"));
    }

    [Test]
    public void Abnormal_status_is_at_x_176()
    {
        VisualElement host = new VisualElement();
        Assert.IsTrue(InterludeWindowLayout.Apply(host, "AbnormalStatusWnd"));
        Assert.AreEqual(176f, host.style.left.value.value, 0.01f);
        Assert.AreEqual(0f, host.style.top.value.value, 0.01f);
        ExpectRect(host, "StatusIcon", 12f, 0f, 26f, 26f);
    }

    [Test]
    public void Target_hp_bar_and_close_match_extracted_rects()
    {
        VisualElement host = new VisualElement();
        Assert.IsTrue(InterludeWindowLayout.Apply(host, "TargetStatusWnd"));
        Assert.AreEqual(176f, host.style.width.value.value, 0.01f);
        Assert.AreEqual(46f, host.style.height.value.value, 0.01f);
        Assert.AreEqual(LengthUnit.Percent, host.style.left.value.unit);
        ExpectRect(host, "barHP", 16f, 26f, 150f, 6f);
        ExpectRect(host, "barMP", 16f, 33f, 150f, 6f);
        ExpectRect(host, "btnClose", 151f, 6f, 15f, 15f);
    }

    [Test]
    public void Chat_tabs_are_64_by_23_and_edit_box_matches()
    {
        VisualElement host = new VisualElement();
        Assert.IsTrue(InterludeWindowLayout.Apply(host, "ChatWnd"));
        Assert.AreEqual(348f, host.style.width.value.value, 0.01f);
        Assert.AreEqual(187f, host.style.height.value.value, 0.01f);
        VisualElement all = host.Q("ChatTabCtrl_NormalChat");
        Assert.IsNotNull(all);
        Assert.AreEqual(64f, all.style.width.value.value, 0.01f);
        Assert.AreEqual(23f, all.style.height.value.value, 0.01f);
        Assert.AreEqual("All", ((Button)all).text);
        Assert.AreEqual("+Trade", ((Button)host.Q("ChatTabCtrl_TradeChat")).text);
        Assert.AreEqual("#Party", ((Button)host.Q("ChatTabCtrl_PartyChat")).text);
        Assert.AreEqual("@Clan", ((Button)host.Q("ChatTabCtrl_ClanChat")).text);
        Assert.AreEqual("$Alliance", ((Button)host.Q("ChatTabCtrl_AllyChat")).text);
        ExpectRect(host, "ChatEditBox", 39f, 165f, 303f, 16f);
    }

    [Test]
    public void Radar_is_102_square_at_the_top_right()
    {
        VisualElement host = new VisualElement();
        Assert.IsTrue(InterludeWindowLayout.Apply(host, "RadarContainerWnd"));
        Assert.AreEqual(102f, host.style.width.value.value, 0.01f);
        Assert.AreEqual(102f, host.style.height.value.value, 0.01f);
        Assert.AreEqual(0f, host.style.right.value.value, 0.01f);
        Assert.AreEqual(0f, host.style.top.value.value, 0.01f);
        ExpectRect(host, "Radar", 0f, 0f, 102f, 102f);
    }

    [Test]
    public void Lobby_menu_buttons_match_extracted_rects()
    {
        InterludeLayoutFile file = LoadLayout();
        InterludeLayoutWindow menu = RequireWindow(file, "NCLobbyFunctionWnd");
        ExpectLocal(menu, "NCLobbyFunctionWnd", 0f, 0f, 120f, 138f);
        ExpectLocal(menu, "btnCreate", 13f, 20f, 96f, 23f);
        ExpectLocal(menu, "btnDelete", 13f, 45f, 96f, 23f);
        ExpectLocal(menu, "btnReLogin", 13f, 95f, 96f, 23f);

        InterludeLayoutWindow start = RequireWindow(file, "NCLobbyStartWnd");
        ExpectLocal(start, "NCLobbyStartWnd", 0f, 0f, 140f, 49f);
        ExpectLocal(start, "btnStart", 13f, 10f, 116f, 31f);

        InterludeLayoutWindow card = RequireWindow(file, "NCLobbyWnd_CharInfo");
        ExpectLocal(card, "NameField", 13f, 46f, 138f, 17f);
        ExpectLocal(card, "HPBar", 35f, 55f, 85f, 12f);
        ExpectLocal(card, "MPBar", 35f, 69f, 85f, 12f);
        ExpectLocal(card, "EXPBar", 35f, 83f, 206f, 12f);
        ExpectLocalPoint(card, "labelA", 15f, 16f);
        ExpectLocalPoint(card, "labelB", 15f, 39f);
    }

    [Test]
    public void Minimap_viewport_matches_extracted_rect()
    {
        VisualElement host = new VisualElement();
        Assert.IsTrue(InterludeWindowLayout.Apply(host, "MinimapWnd"));
        Assert.AreEqual(334f, host.style.width.value.value, 0.01f);
        Assert.AreEqual(413f, host.style.height.value.value, 0.01f);
        ExpectRect(host, "Minimap", 3f, 51f, 328f, 328f);
        ExpectRect(host, "MyLocButton", 10f, 385f, 76f, 23f);
    }

    [Test]
    public void Missing_window_is_rejected()
    {
        LogAssert.Expect(LogType.Error, new Regex("LoginWnd"));
        VisualElement host = new VisualElement();
        Assert.IsFalse(InterludeWindowLayout.Apply(host, "LoginWnd"));
    }

    static void ExpectRect(VisualElement host, string name, float left, float top, float width, float height)
    {
        VisualElement element = host.Q(name);
        Assert.IsNotNull(element, name);
        Assert.AreEqual(left, element.style.left.value.value, 0.01f, name + " left");
        Assert.AreEqual(top, element.style.top.value.value, 0.01f, name + " top");
        Assert.AreEqual(width, element.style.width.value.value, 0.01f, name + " width");
        Assert.AreEqual(height, element.style.height.value.value, 0.01f, name + " height");
    }

    static InterludeLayoutFile LoadLayout()
    {
        TextAsset asset = Resources.Load<TextAsset>(InterludeWindowLayout.ResourcesKey);
        Assert.IsNotNull(asset, InterludeWindowLayout.ResourcesKey);
        InterludeLayoutFile file = JsonUtility.FromJson<InterludeLayoutFile>(asset.text);
        Assert.IsNotNull(file);
        Assert.IsNotNull(file.windows);
        return file;
    }

    static InterludeLayoutWindow RequireWindow(InterludeLayoutFile file, string name)
    {
        for (int i = 0; i < file.windows.Length; i++)
        {
            if (file.windows[i].name == name)
                return file.windows[i];
        }

        Assert.Fail("layout asset is missing window " + name);
        return null;
    }

    static void ExpectLocal(InterludeLayoutWindow window, string name, float x, float y, float width, float height)
    {
        InterludeLayoutControl control = RequireControl(window, name);
        Assert.AreEqual(x, control.x, 0.01f, name + " x");
        Assert.AreEqual(y, control.y, 0.01f, name + " y");
        Assert.AreEqual(width, control.width, 0.01f, name + " width");
        Assert.AreEqual(height, control.height, 0.01f, name + " height");
    }

    static void ExpectLocalPoint(InterludeLayoutWindow window, string name, float x, float y)
    {
        InterludeLayoutControl control = RequireControl(window, name);
        Assert.AreEqual(x, control.x, 0.01f, name + " x");
        Assert.AreEqual(y, control.y, 0.01f, name + " y");
    }

    static InterludeLayoutControl RequireControl(InterludeLayoutWindow window, string name)
    {
        Assert.IsNotNull(window.controls, window.name);
        for (int i = 0; i < window.controls.Length; i++)
        {
            if (window.controls[i].name == name)
                return window.controls[i];
        }

        Assert.Fail(window.name + " is missing control " + name);
        return null;
    }
}
