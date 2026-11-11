using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class SkillListWindow : L2PopupWindow
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



   

    /// <summary>
    /// Interlude MagicSkillWnd under MainWnd Skill tab — reuse this window's content
    /// inside CharacterInfo SkillsHost instead of a second floating chrome.
    /// </summary>



}
