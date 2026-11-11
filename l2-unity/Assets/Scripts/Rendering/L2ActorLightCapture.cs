using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public static partial class L2ActorLightCapture
{
    public const string Root =
        @"C:\games\Lineage II HighElfes\ADEU-P464-D20240703-P-240117-240724-1\system\logs\MonsterSearch\unity";

    // monster_008, game hour 16.976. Direction is L2 axes, Z up.
    static readonly Vector4 ClientC249 = new Vector4(0.342226118f, 0.469668895f, -0.813813388f, 0f);
    static readonly Vector4 ClientC250 = new Vector4(0.248003364f, 0.248003364f, 0.248003364f, 1f);
    static readonly Vector4 ClientC251 = new Vector4(0.235294119f, 0.235294119f, 0.235294119f, 0f);


    const int KaterinaNpcId = 30004;
    const string EnvShader = "L2/World/ActorEnvAdd";
    const float ExpectedCutoff = 0.627451f;




}
