using NUnit.Framework;

public sealed class InterludeSoundTokenPathEditModeTests
{
    [Test]
    public void Item_sound_token_maps_to_fmod_event_path()
    {
        Assert.IsTrue(InterludeSoundTokenPath.TryFromDatToken("ItemSound.sword_big_9", out string path));
        Assert.AreEqual("event:/ItemSound/sword_big_9", path);
        Assert.IsFalse(InterludeSoundTokenPath.IsUiOrInterfaceEvent(path));
    }

    [Test]
    public void Interface_token_is_rejected_for_combat()
    {
        Assert.IsFalse(InterludeSoundTokenPath.TryFromDatToken("InterfaceSound.window_open", out _));
        Assert.IsTrue(InterludeSoundTokenPath.IsUiOrInterfaceEvent("event:/InterfaceSound/window_open"));
        Assert.IsFalse(InterludeSoundTokenPath.IsCombatSfxEvent("event:/InterfaceSound/window_open"));
    }
}