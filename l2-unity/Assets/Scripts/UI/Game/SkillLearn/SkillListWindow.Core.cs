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
    }
#endif

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
            TryApplyOrphanedSkillList();
        }
        else
        {
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
