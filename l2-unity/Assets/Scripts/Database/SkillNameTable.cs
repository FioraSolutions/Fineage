using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public partial class SkillNameTable
{
    private static SkillNameTable _instance;
    private static Dictionary<int, Dictionary<int, SkillNameData>> _names;
    public Dictionary<int, Dictionary<int, SkillNameData>> Names { get { return _names; } }

    [Obsolete("Use [Inject] SkillNameTable instead")]
    public static SkillNameTable Instance
    {
        get
        {
            if (_instance == null && App.HasContainer)
            {
                try
                {
                    _instance = App.Resolve<SkillNameTable>();
                }
                catch
                {
                }
            }

            if (_instance == null)
            {
                _instance = new SkillNameTable();
            }

            return _instance;
        }
    }

}
