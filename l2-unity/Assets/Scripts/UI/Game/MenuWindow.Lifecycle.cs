using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MenuWindow : L2Window
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireHudCorePacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void Start()
    {
        WireHudCorePacketServices();
    }

    private void WireHudCorePacketServices()
    {
        HudCorePacketServices.Bind(menu: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/MenuWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        ApplyInterludeMenuTextures();
        DockToBottomRight();

        // Interlude MenuWnd: CharInfo / Inventory / Map / SystemMenu only.
        WireMenuButton("CharacterButton", () => HudCorePacketServices.CharacterInfo.ToggleHideWindow());
        WireMenuButton("InventoryButton", () => InventoryPacketServices.Window.ToggleHideWindowManual());
        WireMenuButton("MapButton", () => HudCorePacketServices.Map.ToggleHideWindow());
        WireMenuButton("SystemMenuButton", () => SystemUIPacketServices.Menu.ToggleHideWindow());

        yield return new WaitForEndOfFrame();
        DockToBottomRight();
    }
}
