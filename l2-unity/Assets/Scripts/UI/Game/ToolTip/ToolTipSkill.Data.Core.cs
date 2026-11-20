using System;
using System.Reflection.Emit;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ToolTipSkill
{
    private void SetData(Skillgrp skillgrp, SkillNameData skillNameData)
    {
        string hp = skillgrp.HpConsume.ToString();
        string mp = skillgrp.MpConsume.ToString();
        string radius = skillgrp._cast_range.ToString();
        string desc = skillNameData.Desc;
        string time = skillgrp.ReuseDelay.ToString();
        string name = skillNameData.Name;
        string level = skillgrp.Level.ToString();
        string nameActiveSkill = GetActive(skillgrp.OperateType);

        if (desc.Equals(""))
        {
            if (SkillNames == null)
            {
                Debug.LogError(
                    "[ToolTipSkill] SkillNameTable missing — cannot resolve description skill id="
                    + skillgrp.Id + " level=" + skillgrp.Level + ".");
            }
            else if (skillgrp.Level != 1)
            {
                SkillNameData levelOne = SkillNames.GetName(skillgrp.Id, 1);
                if (levelOne != null && !string.IsNullOrEmpty(levelOne.Desc))
                {
                    Debug.LogError(
                        "[ToolTipSkill] missing skillname description skill id=" + skillgrp.Id
                        + " level=" + skillgrp.Level + " — using level=1 desc as degrade.");
                    desc = levelOne.Desc;
                }
                else
                {
                    Debug.LogError(
                        "[ToolTipSkill] missing skillname description skill id=" + skillgrp.Id
                        + " level=" + skillgrp.Level + " (no level=1 row either).");
                }
            }
            else
            {
                Debug.LogError(
                    "[ToolTipSkill] missing skillname description skill id=" + skillgrp.Id + " level=1.");
            }
        }


        SetDataTooTip(DataEmpty(hp), DataEmpty(mp), DataEmpty(radius), desc, DataEmpty(time), nameActiveSkill , name, level);
        L2SlotIconView.SetNamedIcon(_icon, skillgrp.Icon);
        HideEmptyData(DataEmpty(hp), DataEmpty(mp), DataEmpty(radius), desc, DataEmpty(time), nameActiveSkill, name, level);
        ShowHidenData(DataEmpty(hp), DataEmpty(mp), DataEmpty(radius), desc, DataEmpty(time), nameActiveSkill, name, level);
    }

    private string GetActive(int operate_type)
    {
        if(operate_type == 1)
        {
            return "Active Skill";
        }
        else if (operate_type == 2)
        {
            return "Passive Skill";
        }
        return "";
    }

    private string DataEmpty(string data)
    {
        if(data.Equals("0") | data.Equals("-1"))
        {
            return "";
        }
        return data;
    }
}
