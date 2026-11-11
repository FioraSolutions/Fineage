using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using VContainer;

public partial class SkillNameTable : ISkillNameTable
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

    private Dictionary<int, SkillNameData> CreateDict()
    {
        return new Dictionary<int, SkillNameData>();
    }

    private const int IndexName = 2;
    private const int IndexDescription = 3;

}
