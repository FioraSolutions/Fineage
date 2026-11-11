using System;
using System.Collections.Generic;
using VContainer;
using System.IO;
using UnityEngine;

public sealed partial class MobSkillAnimgrpTable
{
    private static MobSkillAnimgrpTable _instance;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static MobSkillAnimgrpTable Instance
    {
        get
        {
            if (_instance == null)
                _instance = new MobSkillAnimgrpTable();
            return _instance;
        }
    }

    private readonly Dictionary<long, string> _sequences = new Dictionary<long, string>();



}
