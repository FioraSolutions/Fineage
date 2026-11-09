using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class SkillbarWindow : L2PopupWindow
{
    const string PrefLeft = "fineage.ui.skillbar.left";
    const string PrefTop = "fineage.ui.skillbar.top";
    /// <summary>Interlude MenuWnd width: 4×2 @ 34px pitch 37 (see .menu-window).</summary>
    const float MenuDockWidth = 152f;

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

    /// <summary>Restore last drag from PlayerPrefs, else dock flush left of the bottom-right menu.</summary>
    public void ApplyPosition()
    {
        if (_windowEle == null)
            return;

        _windowEle.style.translate = StyleKeyword.Null;
        _windowEle.style.right = StyleKeyword.Null;

        float parentWidth = ResolveParentWidth();
        float parentHeight = ResolveParentHeight();
        float barWidth = _windowEle.resolvedStyle.width;
        if (barWidth <= 1f)
            barWidth = 500f;
        float barHeight = _windowEle.resolvedStyle.height;
        if (barHeight <= 1f)
            barHeight = 46f;

        if (PlayerPrefs.HasKey(PrefLeft) && PlayerPrefs.HasKey(PrefTop))
        {
            float left = PlayerPrefs.GetFloat(PrefLeft);
            float top = PlayerPrefs.GetFloat(PrefTop);
            // Stale prefs (e.g. top past panel bottom after resize) hide the bar entirely.
            if (IsOnScreen(left, top, barWidth, barHeight, parentWidth, parentHeight))
            {
                _windowEle.style.bottom = StyleKeyword.Null;
                _windowEle.style.left = left;
                _windowEle.style.top = top;
                return;
            }

            PlayerPrefs.DeleteKey(PrefLeft);
            PlayerPrefs.DeleteKey(PrefTop);
            PlayerPrefs.Save();
        }

        DockDefault(parentWidth, barWidth);
    }

    static bool IsOnScreen(float left, float top, float barWidth, float barHeight, float parentWidth, float parentHeight)
    {
        if (float.IsNaN(left) || float.IsNaN(top) || parentWidth <= 1f || parentHeight <= 1f)
            return false;
        // Keep at least half the bar inside the panel so it stays findable/draggable.
        float visibleW = Mathf.Min(left + barWidth, parentWidth) - Mathf.Max(left, 0f);
        float visibleH = Mathf.Min(top + barHeight, parentHeight) - Mathf.Max(top, 0f);
        return visibleW >= barWidth * 0.5f && visibleH >= barHeight * 0.5f;
    }

    void DockDefault(float parentWidth, float barWidth)
    {
        _windowEle.style.left = Mathf.Max(0f, parentWidth - MenuDockWidth - barWidth);
        _windowEle.style.top = StyleKeyword.Null;
        _windowEle.style.bottom = 0;
    }

    float ResolveParentWidth()
    {
        if (_windowEle.parent != null && _windowEle.parent.resolvedStyle.width > 1f)
            return _windowEle.parent.resolvedStyle.width;
        return Screen.width;
    }

    float ResolveParentHeight()
    {
        if (_windowEle.parent != null && _windowEle.parent.resolvedStyle.height > 1f)
            return _windowEle.parent.resolvedStyle.height;
        return Screen.height;
    }

    public void PersistPosition()
    {
        if (_windowEle == null)
            return;

        float left = _windowEle.resolvedStyle.left;
        float top = _windowEle.resolvedStyle.top;
        if (float.IsNaN(left) || float.IsNaN(top))
            return;

        float parentWidth = ResolveParentWidth();
        float parentHeight = ResolveParentHeight();
        float barWidth = _windowEle.resolvedStyle.width > 1f ? _windowEle.resolvedStyle.width : 500f;
        float barHeight = _windowEle.resolvedStyle.height > 1f ? _windowEle.resolvedStyle.height : 46f;
        if (!IsOnScreen(left, top, barWidth, barHeight, parentWidth, parentHeight))
            return;

        PlayerPrefs.SetFloat(PrefLeft, left);
        PlayerPrefs.SetFloat(PrefTop, top);
        PlayerPrefs.Save();
    }


    private void InitializeSkillbars()
    {
        for (int x = 0; x < 2; x++)
        {
            for (int i = 0; i < PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT; i++)
            {
                bool horizontalBar = x == 0;
                var skillbar = CreateSkillbar(i, horizontalBar);
                StartCoroutine(BuildSkillbarWindow(skillbar, horizontalBar));
                _skillbars.Add(skillbar);
            }
        }
    }

    private AbstractSkillbar CreateSkillbar(int index, bool horizontalBar)
    {
        return index == 0 ?
            new SkillbarMain(_windowEle, index, index, horizontalBar) :
            new SkillbarMin(_windowEle, index, index, horizontalBar);
    }

    private IEnumerator BuildSkillbarWindow(AbstractSkillbar skillbar, bool horizontalBar)
    {
        var template = horizontalBar ? _skillbarHorizontalTemplate : _skillbarVerticalTemplate;
        var container = horizontalBar ? _skillbarContainerHorizontal : _skillbarContainerVertical;
        return skillbar.BuildWindow(template, container);
    }

    public void AddSkillbar()
    {
        for (int x = 0; x < 2; x++)
        {
            if (_expandedSkillbarCount >= PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT - 2)
            {
                ((SkillbarMain)_skillbars[x * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT]).UpdateExpandInput(1);
            }
        }

        _expandedSkillbarCount++;

        ExpandSkillbar();
    }

    public void ResetSkillbar()
    {
        _expandedSkillbarCount = 0;

        for (int x = 0; x < 2; x++)
        {
            ((SkillbarMain)_skillbars[x * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT]).UpdateExpandInput(0);
        }

        MinimizeSkillbar();
    }

    private void ExpandSkillbar()
    {
        // Stop all skillbar minimize animations
        _minimizeCoroutines.ForEach((c) => StopCoroutine(c));
        _minimizeCoroutines.Clear();

        for (int x = 0; x < 2; x++)
        {
            // Expand the next skillbar
            SkillbarMin skillbar = (SkillbarMin)_skillbars[_expandedSkillbarCount + PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT * x];
            _expandCoroutines.Add(StartCoroutine(skillbar.Expand()));

            // Hide skillbars that are not supposed to be visible
            for (int i = _expandedSkillbarCount + 1; i < PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT; i++)
            {
                _skillbars[i + PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT * x].HideBar();
            }
        }
    }

    private void MinimizeSkillbar()
    {
        // Stop all skillbar expand animations
        _expandCoroutines.ForEach((c) => StopCoroutine(c));
        _expandCoroutines.Clear();

        // Minimize all skillbars
        for (int x = 0; x < 2; x++)
        {
            for (int i = 1; i < PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT; i++)
            {
                SkillbarMin skillbar = (SkillbarMin)_skillbars[i + PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT * x];
                _minimizeCoroutines.Add(StartCoroutine(skillbar.Minimize()));
            }
        }
    }

    public void ToggleRotate()
    {
        _vertical = !_vertical;

        if (_vertical)
        {
            _skillbarContainerHorizontal.style.display = DisplayStyle.None;
            _skillbarContainerVertical.style.display = DisplayStyle.Flex;
        }
        else
        {
            _skillbarContainerHorizontal.style.display = DisplayStyle.Flex;
            _skillbarContainerVertical.style.display = DisplayStyle.None;
        }
    }

    public void OnPageChanged(int skillbarIndex, int page)
    {
        for (int x = 0; x < 2; x++)
        {
            _skillbars[skillbarIndex + PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT * x].ChangePage(page);
        }

        Shortcuts.UpdatePageMapping(skillbarIndex, page);

        UpdateAllShortcuts();
    }

    public void ToggleLockSkillBar()
    {
        Locked = !Locked;

        for (int x = 0; x < 2; x++)
        {
            ((SkillbarMain)_skillbars[x * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT]).ToggleLockSkillBar();
        }
    }

    public void ToggleDisableTooltip()
    {
        TooltipDisabled = !TooltipDisabled;

        for (int x = 0; x < 2; x++)
        {
            ((SkillbarMain)_skillbars[x * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT]).ToggleDisableTooltip();
        }
    }

    public void UpdateAllShortcuts()
    {
        UpdateAllShortcuts(Shortcuts.Shortcuts);
    }

    public void UpdateAllShortcuts(List<Shortcut> shortcuts)
    {
        _skillbars.ForEach((skillbar) => skillbar.ResetShortcuts());

        if (shortcuts == null)
        {
            return;
        }

        shortcuts.ForEach((shortcut) =>
        {
            UpdateBarShortcut(shortcut);
        });
      
    }

    private void UpdateBarShortcut(Shortcut shortcut)
    {
        if (shortcut.Slot < 12)
        {
            _skillbars.ForEach((skillbar) =>
            {
                if (skillbar.Page == shortcut.Page)
                {
                   // Debug.Log($"Add shortcut at page {shortcut.Page} in slot {shortcut.Slot}.");
                    skillbar.UpdateShortcut(shortcut, shortcut.Slot);
                }
            });
        }
        else
        {
            // Debug.LogError($"Slot value {shortcut.Slot} is too high.");
        }
    }

    public void AddShortcut(Shortcut shortcut)
    {
        UpdateBarShortcut(shortcut);
    }

    /// <summary>
    /// Repaints every shot slot after the zone confirms an auto-shot toggle (or clears one on a
    /// weapon swap / empty stack). Called from the <c>AutoSoulShot</c> apply path.
    /// </summary>
    public void RefreshAutoShotMarks()
    {
        if (_skillbars == null)
            return;
        _skillbars.ForEach(skillbar => skillbar.RefreshSlotMarks());
    }

    public void RemoveShortcut(int oldSlot)
    {
        int slot = oldSlot % PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;
        int page = oldSlot / PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;

        _skillbars.ForEach((skillbar) =>
        {
            if (skillbar.Page == page)
            {
                skillbar.DeleteShortcut(oldSlot);
            }
        });
    }

  
    public Shortcut GetShortcutByPosition(int skillbarListPosition)
    {
        if(skillbarListPosition > PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT)
        {
            return null;
        }

        int slotPosition = skillbarListPosition % PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;
        int pagePosition = skillbarListPosition / PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;

        AbstractSkillbar skillbar = _skillbars[pagePosition];
        //backup
        //foreach (var slot in skillbar.BarSlots)
        //{
        //  if (slot.Position == slotPosition)
        // {
        //     return slot.Shortcut;
        // }
        //}

       
        return GetSlotByPosition(skillbar, slotPosition)?.Shortcut;

        //Debug.LogError($"No skill found at position {skillbarListPosition}");
        //return null;
    }

    private SkillbarSlot GetSlotBySkillId(List<AbstractSkillbar> skillbars, int skillId, int type)
    {
        foreach (var skillbar in skillbars)
        {
            foreach (var slot in skillbar.BarSlots)
            {
                if (slot.Shortcut?.Type == type && slot.Shortcut?.Id == skillId)
                {
                    return slot;
                }
            }
        }
        return null;
    }

    //backup
    //private SkillbarSlot GetSlotBySkillId(List<AbstractSkillbar> skillbars , int skilId , int type)
    //{
    //for(int i =0; i < skillbars.Count; i++)
    //{
    // AbstractSkillbar skillBar = skillbars[0];

    //for (int b = 0; b < skillBar.BarSlots.Count; b++)
    //{
    // SkillbarSlot slot = skillBar.BarSlots[b];

    //if(slot.Shortcut != null)
    // {
    // if (slot.Shortcut.Type == type && slot.Shortcut.Id == skilId)
    //{
    //   return slot;
    //}
    //}

    //}
    // }

    //return null;
    // }

    private SkillbarSlot GetSlotByPosition(AbstractSkillbar skillbar, int slotPosition)
    {
        foreach (var slot in skillbar.BarSlots)
        {
            if (slot.Position == slotPosition)
            {
                return slot;
            }
        }
        return null;
    }

    public void ShowCooldown(int skillId , int shortcutType , int reuseDelay)
    {
        SkillbarSlot skillbarSlot = GetSlotBySkillId(_skillbars, skillId, shortcutType);

        if(skillbarSlot != null)
        {
            StartCoroutine(_cooldownAnimationService.CooldownCoroutine(skillbarSlot.GetReuseElement(), skillbarSlot.GetRechargeMaskElement(), reuseDelay));
        }


    }

}
