using System;
using System.Collections;
using System.Collections.Generic;
using VContainer;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillLearnWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _creatorWindow = new CreatorVerticalScrollWindows();
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
        SkillLearnPacketServices.Bind(skillLearn: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SkillLearn/Learn/SkillLearnWindow");
        _rowsTemplate = LoadAsset("Data/UI/_Elements/Template/Skills/ItemSkillRow");
    }
}
