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
        VisualElement host = new VisualElement();
        Assert.IsTrue(InterludeWindowLayout.Apply(host, "NCLobbyFunctionWnd"));
        Assert.AreEqual(120f, host.style.width.value.value, 0.01f);
        Assert.AreEqual(138f, host.style.height.value.value, 0.01f);
        Assert.AreEqual(34f, host.style.right.value.value, 0.01f);
        Assert.AreEqual(10f, host.style.bottom.value.value, 0.01f);
        ExpectRect(host, "btnCreate", 13f, 20f, 96f, 23f);
        ExpectRect(host, "btnDelete", 13f, 45f, 96f, 23f);
        ExpectRect(host, "btnReLogin", 13f, 95f, 96f, 23f);

        VisualElement start = new VisualElement();
        Assert.IsTrue(InterludeWindowLayout.Apply(start, "NCLobbyStartWnd"));
        Assert.AreEqual(140f, start.style.width.value.value, 0.01f);
        Assert.AreEqual(49f, start.style.height.value.value, 0.01f);
        Assert.AreEqual(10f, start.style.bottom.value.value, 0.01f);
        ExpectRect(start, "btnStart", 13f, 10f, 116f, 31f);

        VisualElement card = new VisualElement();
        Assert.IsTrue(InterludeWindowLayout.Apply(card, "NCLobbyWnd_CharInfo"));
        Assert.AreEqual(24f, card.style.left.value.value, 0.01f);
        Assert.AreEqual(27f, card.style.top.value.value, 0.01f);
        Assert.AreEqual(256f, card.style.width.value.value, 0.01f);
        Assert.AreEqual(108f, card.style.height.value.value, 0.01f);
        ExpectRect(card, "HPBar", 35f, 55f, 85f, 12f);
        ExpectRect(card, "MPBar", 35f, 69f, 85f, 12f);
        ExpectRect(card, "EXPBar", 35f, 83f, 206f, 12f);
    }

    [Test]
    public void Login_windows_match_extracted_anchors()
    {
        // LoginLogoWnd, NCAuthWnd, and NCLoginFunctionWnd come from the nwindow dump.
        VisualElement auth = new VisualElement();
        Assert.IsTrue(InterludeWindowLayout.Apply(auth, "NCAuthWnd"));
        Assert.AreEqual(256f, auth.style.width.value.value, 0.01f);
        Assert.AreEqual(108f, auth.style.height.value.value, 0.01f);
        ExpectRect(auth, "IDInput", 84f, 20f, 128f, 17f);
        ExpectRect(auth, "PasswordInput", 84f, 42f, 128f, 17f);
        ExpectRect(auth, "LoginButton", 31f, 72f, 96f, 23f);
        ExpectRect(auth, "ExitButton", 131f, 72f, 96f, 23f);

        VisualElement menu = new VisualElement();
        Assert.IsTrue(InterludeWindowLayout.Apply(menu, "NCLoginFunctionWnd"));
        Assert.AreEqual(120f, menu.style.width.value.value, 0.01f);
        Assert.AreEqual(138f, menu.style.height.value.value, 0.01f);
        Assert.AreEqual(34f, menu.style.right.value.value, 0.01f);
        Assert.AreEqual(62f, menu.style.bottom.value.value, 0.01f);
        ExpectRect(menu, "NewAccountButton", 13f, 8f, 96f, 23f);
        ExpectRect(menu, "ReplayButton", 13f, 107f, 96f, 23f);

        VisualElement logo = new VisualElement();
        Assert.IsTrue(InterludeWindowLayout.Apply(logo, "LoginLogoWnd"));
        Assert.AreEqual(256f, logo.style.width.value.value, 0.01f);
        Assert.AreEqual(256f, logo.style.height.value.value, 0.01f);
        Assert.AreEqual(50f, logo.style.top.value.value, 0.01f);
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
}
