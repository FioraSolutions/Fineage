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
    /// Overlays official Interlude skillgrp columns onto a Classic row, or inserts a new row
    /// for Interlude-only skills. Classic-only columns (cool_time, reuse_delay, effect_point,
    /// enchant/icon panel, ...) are kept when a Classic row already exists.
    /// </summary>
    private void OverlayOrAdd(Skillgrp incoming)
    {
        if (incoming == null)
            return;

        if (_skills.TryGetValue(incoming.Id, out Dictionary<int, Skillgrp> byLevel)
            && byLevel != null
            && byLevel.TryGetValue(incoming.Level, out Skillgrp existing)
            && existing != null)
        {
            existing.OperateType = incoming.OperateType;
            existing.MpConsume = incoming.MpConsume;
            existing.CastRange = incoming.CastRange;
            existing.CastStyle = incoming.CastStyle;
            existing.HitTime = incoming.HitTime;
            existing.IsMagic = incoming.IsMagic;
            existing.Animation = incoming.Animation;
            existing.SkillVisualEffect = incoming.SkillVisualEffect;
            existing.Icon = incoming.Icon;
            existing.HpConsume = incoming.HpConsume;
            existing.RumbleSelf = incoming.RumbleSelf;
            existing.RumbleTarget = incoming.RumbleTarget;
            return;
        }

        TryAdd(incoming);
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


    public void ReadAnimationCombo_Classic()
    {
        //_actionsInterlude = new Dictionary<int, ActionData>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/AnimationCombo_Classic.txt");

        if (!File.Exists(dataPath))
        {
            // Debug.LogWarning("File not found: " + dataPath);
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
