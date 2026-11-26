using NUnit.Framework;
using UnityEngine;

public sealed class LobbyWorldClickEditModeTests
{
    [Test]
    public void Null_camera_is_not_a_world_click_view()
    {
        Assert.IsFalse(EntityPick.IsWorldView(null));
        Assert.IsFalse(EntityPick.IsLobbyPresentation(null));
    }

    [Test]
    public void Character_select_camera_is_not_a_world_click_view()
    {
        var go = new GameObject("CharSelect");
        Camera cam = go.AddComponent<Camera>();
        try
        {
            Assert.IsFalse(EntityPick.IsWorldView(cam));
        }
        finally
        {
            Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void Gameplay_camera_is_a_world_click_view_when_the_lobby_cameras_are_off()
    {
        var go = new GameObject("Gameplay");
        Camera cam = go.AddComponent<Camera>();
        try
        {
            Assert.IsTrue(EntityPick.IsWorldView(cam));
        }
        finally
        {
            Object.DestroyImmediate(go);
        }
    }
}
