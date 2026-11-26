using NUnit.Framework;

public class InterfaceWindowSoundsEditModeTests
{
    [SetUp]
    public void SetUp()
    {
        InterfaceWindowSounds.ClearForTests();
        OfficialWaveTable.ClearForTests();
    }

    [TearDown]
    public void TearDown()
    {
        InterfaceWindowSounds.ClearForTests();
        OfficialWaveTable.ClearForTests();
    }

    [Test]
    public void Named_windows_use_official_interface_tokens()
    {
        InterfaceWindowSounds.LoadFrom(
            "window\topen\tclose\n"
            + "InventoryWindow\tInterfaceSound.inventory_open_01\tInterfaceSound.inventory_close_01\n"
            + "*\tItemSound.window_open\tItemSound.window_close\n",
            "test://interface-window-sounds.tsv");
        Assert.AreEqual("InterfaceSound.inventory_open_01", InterfaceWindowSounds.OpenToken("InventoryWindow"));
        Assert.AreEqual("InterfaceSound.inventory_close_01", InterfaceWindowSounds.CloseToken("InventoryWindow"));
        Assert.AreEqual("ItemSound.window_open", InterfaceWindowSounds.OpenToken("TargetWindow"));
        Assert.AreEqual("ItemSound.window_close", InterfaceWindowSounds.CloseToken("UnknownWindow"));
    }

    [Test]
    public void Official_table_resolves_interface_and_messenger_and_item_ui_tokens()
    {
        OfficialWaveTable.LoadFrom(
            "package\tgroup\tname\tresources_path\n"
            + "interfacesound\tInterface\tclick_01\tData/Sounds/InterfaceSound/click_01\n"
            + "interfacesound\tInterface\tinventory_open_01\tData/Sounds/InterfaceSound/inventory_open_01\n"
            + "messengersound\tMsn\tnewemail\tData/Sounds/MessengerSound/newemail\n"
            + "itemsound\tInterface\twindow_open\tData/Sounds/ItemSound/window_open\n"
            + "ItemSound3\tSystemMsg\tsys_impossible\tData/Sounds/ItemSound/sys_impossible\n",
            "test://official-waves.tsv");

        Assert.IsTrue(OfficialWaveTable.TryResolveDatToken("InterfaceSound.click_01", out string click, out _));
        Assert.AreEqual("Data/Sounds/InterfaceSound/click_01", click);
        Assert.IsTrue(OfficialWaveTable.TryResolve("window_open", out OfficialWaveTable.Entry entry, out _));
        Assert.AreEqual("Data/Sounds/ItemSound/window_open", entry.ResourcesPath);
        Assert.IsTrue(OfficialWaveTable.TryResolveDatToken("ItemSound3.sys_impossible", out string sys, out _));
        Assert.AreEqual("Data/Sounds/ItemSound/sys_impossible", sys);
        Assert.IsTrue(OfficialWaveTable.TryResolveDatToken("MessengerSound.newemail", out string mail, out _));
        Assert.AreEqual("Data/Sounds/MessengerSound/newemail", mail);
        Assert.IsFalse(OfficialWaveTable.TryResolve("click_03", out _, out string error));
        Assert.IsTrue(error.Contains("name=\"click_03\""), error);
    }
}
