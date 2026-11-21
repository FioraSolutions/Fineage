using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillListWindow : L2PopupWindow
{
    static List<SkillInstance> _orphanedSkillList;

    public static void StashOrphanedSkillList(SkillListDto packet)
    {
        if (packet?.Skills == null || packet.Skills.Count == 0)
            return;
        _orphanedSkillList = packet.Skills;
    }

#if UNITY_EDITOR
    public static void EditModeResetSingletonForTests()
    {
        _instance = null;
        SkillLearnPacketServices.EditModeClearForTests();
        _orphanedSkillList = null;
    }

    public static void EditModeAdoptSingletonForTests(SkillListWindow owner)
    {
        if (owner == null)
            return;

        SkillListWindow[] existing = UnityEngine.Object.FindObjectsByType<SkillListWindow>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        for (int i = 0; i < existing.Length; i++)
        {
            if (existing[i] != null && existing[i] != owner)
                UnityEngine.Object.DestroyImmediate(existing[i].gameObject);
        }

        _instance = owner;
        owner.EnsureEditModeSingletonInitializedForTests();
        SkillLearnPacketServices.Bind(skillList: owner);
    }

    /// <summary>EditMode does not run <c>Awake</c>; claim singleton state like Awake without duplicate destroy.</summary>
    public void EditorTestClaimSingletonForTests()
    {
        if (_instance != null && _instance != this)
            return;

        _instance = this;
        _builderTabs ??= new BuilderTabs();
        _supportActiveSkills ??= new ActiveSkillsHide();
        _supportPassiveSkills ??= new PassiveSkillsHide();
        _allSkills ??= new Dictionary<int, SkillInstance>();
    }
#endif

    void ClaimAsSkillListSingleton()
    {
        _instance = this;
        _builderTabs = new BuilderTabs();
        _supportActiveSkills = new ActiveSkillsHide();
        _supportPassiveSkills = new PassiveSkillsHide();
        _allSkills = new Dictionary<int, SkillInstance>();
        WireSkillLearnPacketServices();
        TryApplyOrphanedSkillList();
    }

    private void Awake()
    {
        if (_instance == null)
            ClaimAsSkillListSingleton();
        else if (_instance != this)
        {
            Debug.LogError(
                "[SkillList] Duplicate SkillListWindow destroyed on Awake: '" + gameObject.name + "' (id=" +
                GetInstanceID() + "). Singleton owner='" + _instance.gameObject.name + "' (id=" +
                _instance.GetInstanceID() + ").");
            Destroy(this);
        }
    }

    private void Start()
    {
        WireSkillLearnPacketServices();
        TryApplyOrphanedSkillList();
    }

    void TryApplyOrphanedSkillList()
    {
        if (_orphanedSkillList == null)
            return;
        List<SkillInstance> pending = _orphanedSkillList;
        _orphanedSkillList = null;
        UpdateSkillList(pending);
    }

    private void WireSkillLearnPacketServices()
    {
        SkillLearnPacketServices.Bind(skillList: this);
    }
}
