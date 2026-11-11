using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

public partial class SkillgrpTable : ISkillgrpLookup
{
    private static SkillgrpTable _instance;

    [Obsolete("Use [Inject] SkillgrpTable or TablePacketServices.SkillGrps instead")]
    public static SkillgrpTable Instance
    {
        get
        {
            if (_instance == null && App.HasContainer)
            {
                try
                {
                    _instance = App.Resolve<SkillgrpTable>();
                }
                catch
                {
                }
            }

            if (_instance == null)
            {
                _instance = new SkillgrpTable();
            }

            return _instance;
        }
    }


}
