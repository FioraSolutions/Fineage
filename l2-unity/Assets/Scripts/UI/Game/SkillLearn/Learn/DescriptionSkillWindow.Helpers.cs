using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class DescriptionSkillWindow : L2PopupWindow
{
    public void AddData(AcquireSkillInfoDto packet)
    {
        _packet = packet;
        _dataProvider.AddDescriptionSkill(packet.GetId(), packet.GetSpCoast(), packet.GetLevel(), _content);
        _dataProvider.AddRequiredSkillInfo(packet.RequiredSkillInfo, _content);
    }

    public override void ShowWindow()
    {
        UserInfoDto user = StorageNpc.getInstance().GetFirstUser();
        _spLabel.text = user.PlayerInfoInterlude.Stats.Sp.ToString();
        base.ShowWindow();
    }

    private void OnBack(ClickEvent evt)
    {
        base.HideWindow();
        SkillLearnPacketServices.SkillLearn.ShowWindow();
    }

    private void OnLearnSkill(ClickEvent evt)
    {
        // 0 - GeneralSkills
        // 1 - Common skills.
        // 2 - Pledge skills
        if (_packet != null)
        {
            ZoneCommands.Send(new RequestAcquireSkillCommand(_packet.GetId(), _packet.GetLevel(), _packet.GetMode()));
        }
        base.HideWindow();
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}
