using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

public partial class SkillgrpTable
{
    private void TryAdd(Skillgrp skillGrp)
    {

        if (_skills.ContainsKey(skillGrp.Id) == true)
        {
            Dictionary<int, Skillgrp> dataGrp = _skills[skillGrp.Id];
            dataGrp.TryAdd(skillGrp.Level, skillGrp);
        }
        else
        {
            Dictionary<int, Skillgrp> dataGrp = CreateDict();
            AddDict(dataGrp, skillGrp);
            _skills.TryAdd(skillGrp.Id, dataGrp);
        }
    }

    private static string NormalizeToken(string value)
    {
        if (string.IsNullOrEmpty(value) || string.Equals(value, "None", StringComparison.OrdinalIgnoreCase))
            return string.Empty;
        return value;
    }

    /// <summary>
    /// Copies the columns Interlude skillgrp.dat does not carry onto an existing Interlude row.
    /// Rows for skills Interlude does not know are ignored; animation, visual effect, icon,
    /// timing and cost always stay Interlude.
    /// </summary>
    private void SupplementNonVisualColumns(Skillgrp supplement)
    {
        if (supplement == null)
            return;

        if (!_skills.TryGetValue(supplement.Id, out Dictionary<int, Skillgrp> byLevel)
            || byLevel == null
            || !byLevel.TryGetValue(supplement.Level, out Skillgrp existing)
            || existing == null)
        {
            return;
        }

        existing.SubLevel = supplement.SubLevel;
        existing.Icon_type = supplement.Icon_type;
        existing.MagicType = supplement.MagicType;
        existing.CoolTime = supplement.CoolTime;
        existing.ReuseDelay = supplement.ReuseDelay;
        existing.EffectPoint = supplement.EffectPoint;
        existing.IsDouble = supplement.IsDouble;
        existing.IconPanel = supplement.IconPanel;
        existing.Debuff = supplement.Debuff;
        existing.ResistCast = supplement.ResistCast;
        existing.EnchantSkillLevel = supplement.EnchantSkillLevel;
        existing.EnchantIcon = supplement.EnchantIcon;
    }

    private Dictionary<int, Skillgrp> CreateDict()
    {
        return  new Dictionary<int, Skillgrp>();
    }
    private void AddDict(Dictionary<int, Skillgrp> dataGrp , Skillgrp skillGrp)
    {
        dataGrp.TryAdd(skillGrp.Level, skillGrp);
    }

    public AnimationCombo GetAnimComboBySkillId(int skillId , int level)
    {
        Skillgrp grp = GetSkill(skillId, level);
        if (grp == null || string.IsNullOrEmpty(grp.Animation))
            return null;
        if (_animationsCombo != null && _animationsCombo.TryGetValue(grp.Animation, out AnimationCombo combo))
            return combo;
        return null;
    }


    public void ReadAnimationCombo_Interlude()
    {
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/AnimationCombo_Interlude.txt");

        if (!File.Exists(dataPath))
        {
            Debug.LogError("[SkillgrpTable] animation combo table not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            int index = 0;
            while ((line = reader.ReadLine()) != null)
            {
                //string[] test = line.Split('\t');
                if (index != 0)
                {
                    string[] ids = line.Split('\t');
                    string tag = ids[0];

                  if (!_animationsCombo.ContainsKey(tag))
                    {
                        string anim_name1 = ClearCombo(ids[1]);
                        string anim_name2 = ClearCombo(ids[2]);
                        string anim_name3 = ClearCombo(ids[3]);
                        var arr = new string[3] { anim_name1  , anim_name2 , anim_name3 };
                        string unk0 = ids[4];
                        var combo  = new AnimationCombo(tag, arr , unk0);
                        _animationsCombo.Add(tag , combo);
                    }
                }

                index++;
            }

        }

 
    }

    private string ClearCombo(string anim_name)
    {
        string anim_name1 = anim_name.Replace("a," , "");
        string anim_name2 = anim_name1.Replace("\\0", "");
        string anim_name3 = anim_name2.Replace("AnimName", "");
        return anim_name3;
    }
}
