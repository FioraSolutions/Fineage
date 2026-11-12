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
    public static void Bind(SkillgrpTable table)
    {
        if (table != null)
            _instance = table;
    }

    public static void Unbind() => _instance = null;

    public Skillgrp GetSkill(int id , int level)
    {
       if (_skills == null || !_skills.TryGetValue(id, out Dictionary<int, Skillgrp> byLevel) || byLevel == null)
           return null;

       if (byLevel.TryGetValue(level, out Skillgrp exact) && exact != null)
           return exact;

       // Server level may sit above Classic skillgrp rows — use nearest lower, else any.
       Skillgrp best = null;
       int bestLevel = int.MinValue;
       foreach (var pair in byLevel)
       {
           if (pair.Value == null)
               continue;
           if (pair.Key <= level && pair.Key > bestLevel)
           {
               best = pair.Value;
               bestLevel = pair.Key;
           }
       }

       if (best != null)
           return best;

       foreach (var pair in byLevel)
       {
           if (pair.Value != null)
               return pair.Value;
       }

       return null;
    }
}
