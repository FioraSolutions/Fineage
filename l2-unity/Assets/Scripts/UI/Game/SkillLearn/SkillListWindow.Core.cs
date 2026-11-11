using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillListWindow : L2PopupWindow
{
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
}
