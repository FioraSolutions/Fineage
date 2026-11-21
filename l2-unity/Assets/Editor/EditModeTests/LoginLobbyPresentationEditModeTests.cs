using NUnit.Framework;
using UnityEngine;

public sealed class LoginLobbyPresentationEditModeTests
{
    [Test]
    public void CharSelect_is_account_lobby_camera()
    {
        var go = new GameObject("CharSelect");
        var cam = go.AddComponent<Camera>();
        Assert.IsTrue(LoginLobbyPresentation.IsAccountLobbyCamera(cam));
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Race_child_camera_is_account_lobby()
    {
        var human = new GameObject("Human");
        var male = new GameObject("Male");
        male.transform.SetParent(human.transform);
        var cam = male.AddComponent<Camera>();
        Assert.IsTrue(LoginLobbyPresentation.IsAccountLobbyCamera(cam));
        Object.DestroyImmediate(human);
    }

    [Test]
    public void Account_lobby_applies_render_settings()
    {
        RenderSettings.ambientLight = Color.black;
        RenderSettings.ambientMode = AmbientMode.Skybox;
        LobbyAccountRenderSettings.Apply();
        Assert.AreEqual(AmbientMode.Flat, RenderSettings.ambientMode);
        Assert.Greater(RenderSettings.ambientLight.grayscale, 0.35f);
        LobbyAccountRenderSettings.Release();
    }
}