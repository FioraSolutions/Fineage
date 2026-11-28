using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// The login frame keeps its extracted 256x108 size with no message, and grows to hold a wrapped
/// status line under Login/Exit. Remember me and the status line are part of the frame.
/// </summary>
public sealed class LoginFrameStatusEditModeTests
{
    [Test]
    public void Frame_keeps_extracted_height_without_a_message()
    {
        Assert.AreEqual(108f, InterludeLoginFrameMetrics.FrameHeight(0f), 0.01f);
        Assert.AreEqual(108f, InterludeLoginFrameMetrics.FrameHeight(-3f), 0.01f);
        Assert.AreEqual(108f, InterludeLoginFrameMetrics.FrameHeight(float.NaN), 0.01f);
    }

    [Test]
    public void Frame_grows_to_fit_one_and_two_lines()
    {
        // One 11px tahoma line is about 13px, two lines about 26px.
        Assert.AreEqual(99f + 13f + 9f, InterludeLoginFrameMetrics.FrameHeight(13f), 0.01f);
        Assert.AreEqual(99f + 26f + 9f, InterludeLoginFrameMetrics.FrameHeight(25.4f), 0.01f);
    }

    [Test]
    public void Status_line_starts_below_the_buttons_and_inside_the_frame_width()
    {
        Assert.Greater(InterludeLoginFrameMetrics.StatusTop, InterludeLoginFrameMetrics.ButtonsBottom);
        Assert.LessOrEqual(
            InterludeLoginFrameMetrics.StatusLeft * 2f + InterludeLoginFrameMetrics.StatusWidth,
            InterludeLoginFrameMetrics.FrameWidth + 0.01f);
    }

    [Test]
    public void Login_uxml_has_remember_toggle_and_status_label_inside_the_frame()
    {
        VisualTreeAsset asset = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Login/LoginWindow");
        Assert.IsNotNull(asset, "Data/UI/_Elements/Login/LoginWindow.uxml did not load");
        VisualElement root = asset.CloneTree();
        VisualElement frame = root.Q<VisualElement>("LoginFrame");
        Assert.IsNotNull(frame, "LoginFrame");

        Toggle remember = frame.Q<Toggle>("RememberToggle");
        Assert.IsNotNull(remember, "RememberToggle must be inside LoginFrame");
        Assert.IsTrue(remember.ClassListContains("il-login-remember"));

        Label status = frame.Q<Label>("LabelError");
        Assert.IsNotNull(status, "LabelError must be inside LoginFrame");
        Assert.IsTrue(status.ClassListContains("il-login-error"));
        Assert.AreEqual(PickingMode.Ignore, status.pickingMode);
    }
}
