using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public partial class TargetWindow
{
    void OnExpandPressed(MouseDownEvent evt)
    {
        evt.StopPropagation();
        SetExpanded(!_expanded);
    }

    void SetExpanded(bool expanded)
    {
        _expanded = expanded;
        if (_windowEle == null)
            return;

        if (_expandButton != null)
        {
            if (_expanded)
                _expandButton.AddToClassList("expanded");
            else
                _expandButton.RemoveFromClassList("expanded");
        }

        if (_propertiesRow != null)
            _propertiesRow.style.display = _expanded ? DisplayStyle.Flex : DisplayStyle.None;

        // 46px SmallWnd normally; 76px SmallWindow2 plate while the NpcInfo tree is open.
        RefreshPlateHeight();
    }

    void BindProperties(TargetData targetData)
    {
        if (targetData == null || targetData.Identity == null)
        {
            ClearProperties();
            return;
        }

        int objectId = targetData.Identity.Id;
        int npcId = targetData.Identity.NpcId;
        if (objectId == _boundObjectId && npcId == _boundNpcId)
            return;

        _boundObjectId = objectId;
        _boundNpcId = npcId;

        if (_propertiesRow != null)
            _propertiesRow.Clear();

        Npcgrp npcgrp = npcId > 0 && TablePacketServices.NpcGrps != null
            ? TablePacketServices.NpcGrps.GetNpcgrp(npcId)
            : null;
        int[] skillIds = npcgrp != null ? npcgrp.PropertySkillIds : null;
        int[] skillLevels = npcgrp != null ? npcgrp.PropertySkillLevels : null;

        bool hasProps = skillIds != null && skillLevels != null
            && skillIds.Length > 0 && skillIds.Length == skillLevels.Length;

        if (!hasProps || _propertiesRow == null)
        {
            if (_expandButton != null)
                _expandButton.style.display = DisplayStyle.None;
            SetExpanded(false);
            return;
        }

        int iconsAdded = 0;
        for (int i = 0; i < skillIds.Length; i++)
        {
            if (TryAddPropertyIcon(skillIds[i], skillLevels[i]))
                iconsAdded++;
        }

        if (iconsAdded == 0)
        {
            if (_expandButton != null)
                _expandButton.style.display = DisplayStyle.None;
            SetExpanded(false);
            return;
        }

        if (_expandButton != null)
            _expandButton.style.display = DisplayStyle.Flex;

        // Keep prior expand preference when retargeting another mob with properties.
        if (_expanded)
            _propertiesRow.style.display = DisplayStyle.Flex;
        else
            _propertiesRow.style.display = DisplayStyle.None;

        RefreshPlateHeight();
    }
}
