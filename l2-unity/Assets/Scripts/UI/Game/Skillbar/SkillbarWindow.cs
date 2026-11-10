using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class SkillbarWindow : L2PopupWindow
{
    const string PrefLeft = "fineage.ui.skillbar.left";
    const string PrefTop = "fineage.ui.skillbar.top";
    /// <summary>Interlude MenuWnd width flush to corner; see .menu-window.</summary>
    const float MenuDockWidth = 165f;

    [Inject] PlayerShortcuts _shortcuts;
    [Inject] PlayerInventory _inventory;

    private PlayerShortcuts Shortcuts => SkillbarPacketServices.Shortcuts;

    private bool _locked = false;
    private bool _vertical = true;
    private bool _tooltipDisabled = false;
    [SerializeField] private int _defaultSkillbarCount;
    [SerializeField] private int _expandedSkillbarCount;

    private List<Coroutine> _expandCoroutines;
    private List<Coroutine> _minimizeCoroutines;
    private VisualElement _skillbarContainerHorizontal;
    private VisualElement _skillbarContainerVertical;
    private VisualTreeAsset _skillbarHorizontalTemplate;
    private VisualTreeAsset _skillbarVerticalTemplate;
    private VisualTreeAsset _barSlotTemplate;
    private List<AbstractSkillbar> _skillbars;
    private static SkillbarWindow _instance;
    private CooldownAnimationService _cooldownAnimationService;
    public bool Locked { get { return _locked; } set { _locked = value; } }
    public bool Vertical { get { return _vertical; } set { _vertical = value; } }
    public bool TooltipDisabled { get { return _tooltipDisabled; } set { _tooltipDisabled = value; } }
    public VisualTreeAsset BarSlotTemplate { get { return _barSlotTemplate; } }

    /// <summary>
    /// True once every bar has finished its BuildWindow coroutine and created 12 slots.
    /// ShortCutInit often arrives before this; paint must wait or the init is lost.
    /// </summary>
    public bool IsReady
    {
        get
        {
            if (_skillbars == null || _skillbars.Count == 0)
                return false;
            for (int i = 0; i < _skillbars.Count; i++)
            {
                AbstractSkillbar bar = _skillbars[i];
                if (bar == null || bar.BarSlots == null ||
                    bar.BarSlots.Count < PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR)
                    return false;
            }
            return true;
        }
    }
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SkillbarWindow Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _cooldownAnimationService = new CooldownAnimationService();
            WireSkillbarPacketServices();
        }
        else
        {
            Destroy(this);
        }

        _expandCoroutines = new List<Coroutine>();
        _minimizeCoroutines = new List<Coroutine>();

    }

    private void Start()
    {
        WireSkillbarPacketServices();
    }

    private void WireSkillbarPacketServices()
    {
        SkillbarPacketServices.Bind(skillbar: this, shortcuts: _shortcuts, inventory: _inventory);
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Skillbar/SkillbarWindow");
        _skillbarHorizontalTemplate = LoadAsset("Data/UI/_Elements/Game/Skillbar/SkillBarHorizontal");
        _skillbarVerticalTemplate = LoadAsset("Data/UI/_Elements/Game/Skillbar/SkillBarVertical");
        _barSlotTemplate = LoadAsset("Data/UI/_Elements/Template/SkillbarSlot");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        RegisterClickWindowEvent(_windowEle, null);
        _skillbarContainerHorizontal = GetElementById("SkillbarContainerHorizontal");
        _skillbarContainerVertical = GetElementById("SkillbarContainerVertical");
        ToggleRotate();

        _skillbars = new List<AbstractSkillbar>();

        InitializeSkillbars();

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < _defaultSkillbarCount; i++)
        {
            AddSkillbar();
        }

        yield return new WaitForEndOfFrame();
        ApplyPosition();
        // Prefs / dock need resolved parent size — re-clamp after first layout pass.
        _windowEle.RegisterCallback<GeometryChangedEvent>(OnSkillbarGeometryChanged);
    }

    void OnSkillbarGeometryChanged(GeometryChangedEvent evt)
    {
        if (_windowEle == null)
            return;
        _windowEle.UnregisterCallback<GeometryChangedEvent>(OnSkillbarGeometryChanged);
        ApplyPosition();
    }
}
