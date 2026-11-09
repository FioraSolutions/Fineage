using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class MenuWindow : L2Window
{
    private static MenuWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static MenuWindow Instance { get { return _instance; } }

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

    /// <summary>
    /// USS right/bottom alone was still inset in play mode — pin like Skillbar.
    /// </summary>
    void DockToBottomRight()
    {
        if (_windowEle == null)
            return;

        _windowEle.style.left = StyleKeyword.Null;
        _windowEle.style.top = StyleKeyword.Null;
        _windowEle.style.right = 0;
        _windowEle.style.bottom = 0;
        _windowEle.style.position = Position.Absolute;
    }

    void WireMenuButton(string buttonName, Action onClick)
    {
        Button btn = _windowEle.Q<Button>(buttonName);
        if (btn == null)
            return;
        btn.AddManipulator(new ButtonClickSoundManipulator(btn));
        btn.RegisterCallback<ClickEvent>(_ => onClick());
    }

    /// <summary>
    /// UITK resource() often misses Interlude sprites — stamp Texture2D after build.
    /// </summary>
    void ApplyInterludeMenuTextures()
    {
        if (_windowEle == null)
            return;

        PaintNamed("MenuWndBackTexLeft", "Data/UI/Menu/Interlude/Menu_Interlude_Smallwindow_Back1");
        PaintNamed("MenuWndBackTexMiddle", "Data/UI/Menu/Interlude/Menu_Interlude_Smallwindow_Back2");
        PaintNamed("MenuWndBackTexRight", "Data/UI/Menu/Interlude/Menu_Interlude_Smallwindow_Back3");

        PaintButtonStates(
            "CharacterButton",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton1",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton1_Over",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton1_Down");
        PaintButtonStates(
            "InventoryButton",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton2",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton2_Over",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton2_Down");
        PaintButtonStates(
            "MapButton",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton3",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton3_Over",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton3_Down");
        PaintButtonStates(
            "SystemMenuButton",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton4",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton4_Over",
            "Data/UI/Menu/Interlude/Menu_Interlude_MenuButton4_Down");
    }

    void PaintNamed(string elementName, string resourcesPath)
    {
        VisualElement ele = _windowEle.Q(elementName);
        if (ele == null)
            return;
        L2SlotIconView.TrySetResourcesBackground(ele, resourcesPath);
    }

    void PaintButtonStates(string buttonName, string normalPath, string overPath, string downPath)
    {
        Button btn = _windowEle.Q<Button>(buttonName);
        if (btn == null)
            return;

        Texture2D normal = L2SlotIconView.LoadResourcesTexture(normalPath);
        Texture2D over = L2SlotIconView.LoadResourcesTexture(overPath);
        Texture2D down = L2SlotIconView.LoadResourcesTexture(downPath);
        if (normal != null)
            btn.style.backgroundImage = new StyleBackground(normal);

        bool hovered = false;
        btn.RegisterCallback<MouseEnterEvent>(_ =>
        {
            hovered = true;
            if (over != null)
                btn.style.backgroundImage = new StyleBackground(over);
        });
        btn.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            hovered = false;
            if (normal != null)
                btn.style.backgroundImage = new StyleBackground(normal);
        });
        btn.RegisterCallback<MouseDownEvent>(_ =>
        {
            if (down != null)
                btn.style.backgroundImage = new StyleBackground(down);
        }, TrickleDown.TrickleDown);
        btn.RegisterCallback<MouseUpEvent>(_ =>
        {
            Texture2D tex = hovered && over != null ? over : normal;
            if (tex != null)
                btn.style.backgroundImage = new StyleBackground(tex);
        }, TrickleDown.TrickleDown);
    }

    public Vector2 GetWindowPosition()
    {
        return _windowEle.worldBound.position;
    }
}
