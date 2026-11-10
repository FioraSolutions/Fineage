using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillListWindow : L2PopupWindow
{
    private BuilderTabs _builderTabs;
    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualTreeAsset _templateActiveSkill;
    private VisualTreeAsset _templatePassiveSkill;
    private VisualTreeAsset _templateLearnSkill;

    private VisualTreeAsset _templateBoxPanel;
    private VisualTreeAsset _templateSlotSkill;
    private VisualTreeAsset _templateSkillsRow8x1;

    private const string _activeName = "Active";
    private const string _passiveName = "Passive";
    private const string _learnName = "Learn Skill";
    private Dictionary<int, SkillInstance> _allSkills;
    private List<SkillInstance> _pendingSkills;

    private ActiveSkillsHide _supportActiveSkills;
    private PassiveSkillsHide _supportPassiveSkills;
    private static SkillListWindow _instance;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SkillListWindow Instance
    {
        get { return _instance; }
    }

    public bool IsReady
    {
        get { return _supportActiveSkills != null && _supportActiveSkills.IsReady; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _builderTabs = new BuilderTabs();
            _supportActiveSkills = new ActiveSkillsHide();
            _supportPassiveSkills = new PassiveSkillsHide();
            _allSkills = new Dictionary<int, SkillInstance>();
            WireSkillLearnPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireSkillLearnPacketServices();
    }

    private void WireSkillLearnPacketServices()
    {
        SkillLearnPacketServices.Bind(skillList: this);
    }

   
    public bool IsWindowContain(Vector2 vector2)
    {
        return  _windowEle.worldBound.Contains(vector2);
    }

    /// <summary>
    /// Interlude MagicSkillWnd under MainWnd Skill tab — reuse this window's content
    /// inside CharacterInfo SkillsHost instead of a second floating chrome.
    /// </summary>
    public void EmbedInto(VisualElement host)
    {
        if (host == null)
            return;
        if (_windowEle == null)
            ShowWindow();
        if (_windowEle == null)
            return;
        _windowEle.style.display = DisplayStyle.None;
        VisualElement body = GetElementById("content") ?? _windowEle;
        body.AddToClassList("skills-panel");
        body.AddToClassList("skills-embed");
        VisualElement cancelRow = body.Q("Buttons");
        if (cancelRow != null)
            cancelRow.style.display = DisplayStyle.None;
        if (body.parent != host)
        {
            body.RemoveFromHierarchy();
            host.Add(body);
        }
        body.style.position = Position.Relative;
        body.style.left = 0;
        body.style.top = 0;
        body.style.right = StyleKeyword.Auto;
        body.style.bottom = StyleKeyword.Auto;
        body.style.width = Length.Percent(100);
        body.style.height = Length.Percent(100);
        body.style.marginTop = 0;
        body.style.marginRight = 0;
        body.style.marginBottom = 0;
        body.style.marginLeft = 0;
        body.style.paddingTop = 2;
        body.style.paddingRight = 4;
        body.style.paddingBottom = 2;
        body.style.paddingLeft = 4;
        host.style.display = DisplayStyle.Flex;
        AudioPacketServices.Audio.PlayUISound("window_open");
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SkillLearn/SkillListWindow");
        _tabTemplate = LoadAsset(L2InventoryUiAssets.Tab);
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/SkillLearn/SkillTabHeader");

        _templateActiveSkill = LoadAsset("Data/UI/_Elements/Game/SkillLearn/TabActive");
        _templatePassiveSkill = LoadAsset("Data/UI/_Elements/Game/SkillLearn/TabPassive");
        _templateLearnSkill = LoadAsset("Data/UI/_Elements/Game/SkillLearn/TabLearn");

        //template panels 8x1 skills
        _templateBoxPanel = LoadAsset("Data/UI/_Elements/Template/Skills/SkillBoxRow");
        _templateSkillsRow8x1 = LoadAsset("Data/UI/_Elements/Template/Skills/SkillPanels/SkillsRow8x1");
        _templateSlotSkill = LoadAsset("Data/UI/_Elements/Template/Skills/SkillPanels/SlotSkill");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);


        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        var content = (VisualElement)GetElementById("content");
        content?.AddToClassList("skills-panel");
        VisualElement tabView = content?.Q("InventoryTabView") ?? content;

        _builderTabs.InitContentTabs(new string[3] { _activeName, _passiveName , _learnName });
        _builderTabs.CreateTabs(tabView, _tabTemplate, _tabHeaderTemplate);

        _supportActiveSkills.SetActiveSkillTemplate(_templateActiveSkill , _templateBoxPanel , _templateSkillsRow8x1 , _templateSlotSkill);
        _supportPassiveSkills.SetPassiveSkillTemplate(_templatePassiveSkill, _templateBoxPanel, _templateSkillsRow8x1, _templateSlotSkill);

        _builderTabs.EventSwitchOut += OnSwitchEventOut;

        VisualElement element = _builderTabs.GetActiveContent();
        _supportActiveSkills.GetOrCreateTab(element);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterCloseWindowEventByName("CloseButton");
        RegisterClickWindowEvent(_windowEle, dragArea);
        OnCenterScreen(_root);

        if (_pendingSkills != null)
        {
            SetSkillList(_pendingSkills);
            _pendingSkills = null;
        }
    }

    public void SetSkillList(List<SkillInstance> list)
    {
        if (list == null) return;
        CopyInDictionry(list);

        if (!_supportActiveSkills.IsReady)
        {
            _pendingSkills = list;
            return;
        }

        var activeSkills = list.Where(s => !s.IsPassive).ToList();
        var passiveSkills = list.Where(s => s.IsPassive).ToList();
        // MainWnd SkillsHost is 256px — 6 Interlude-sized slots fit; 7 forces a horizontal scroller.
        _supportActiveSkills.CreateSlots(activeSkills, 6);
        _supportPassiveSkills.CreateSlots(passiveSkills, 6);
    }

    public void UpdateSkillList(List<SkillInstance> list)
    {
        if (list == null) return;
        CopyInDictionry(list);

        if (!_supportActiveSkills.IsReady)
        {
            _pendingSkills = list;
            return;
        }

        var activeSkills = list.Where(s => !s.IsPassive).ToList();
        var passiveSkills = list.Where(s => s.IsPassive).ToList();
        if (_supportActiveSkills.HasSlots)
        {
            _supportActiveSkills.UpdateSlots(activeSkills);
        }
        else
        {
            _supportActiveSkills.CreateSlots(activeSkills, 6);
        }

        _supportPassiveSkills.CreateSlots(passiveSkills, 6);
    }

    public SkillInstance GetSkillInstanceBySkillId(int skillId)
    {
        if (_allSkills == null)
            return null;

        _allSkills.TryGetValue(skillId, out var skill);
        return skill; 
    }

    readonly CooldownAnimationService _cooldownAnimationService = new CooldownAnimationService();

    /// <summary>Reuse overlay on the skills window slot (same frames as the skillbar).</summary>
    public void ShowCooldown(int skillId, int reuseDelayMs)
    {
        if (reuseDelayMs <= 0 || !gameObject.activeInHierarchy)
            return;
        SkillSlot slot = _supportActiveSkills != null ? _supportActiveSkills.GetSlot(skillId) : null;
        if (slot == null || slot.GetReuseElement() == null)
            return;
        StartCoroutine(_cooldownAnimationService.CooldownCoroutine(
            slot.GetReuseElement(), slot.GetRechargeMaskElement(), reuseDelayMs));
    }

    private void CopyInDictionry(List<SkillInstance> list)
    {
        if (_allSkills.Count == 0) _allSkills.Clear();

        _allSkills = list.ToDictionary(si => si.SkillID);
    }


    // }

    // _activeName = "Active";
    //_passiveName = "Passive";
    // _learnName = "Learn Skill";
    private void OnSwitchEventOut(ITab tab, bool isTrade)
    {
        var tabName = tab.GetTabName();
        var element = _builderTabs.GetActiveContent();
        switch (tabName)
        {
            case _activeName:
                _supportActiveSkills.GetOrCreateTab(element);
                break;

            case _passiveName:
                _supportPassiveSkills.GetOrCreateTab(element);
                break;

            case _learnName:

                break;
        }


    }



 


}
