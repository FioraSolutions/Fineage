using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static partial class L2ActorPointLights
{
    public const int Max = 5;

    // Снимок комнаты применил один цвет. Слоты 1–4 остаются нулём. Шейдер их тоже не складывает.
    const int PassCount = 1;
    const float RefreshSec = 1f;

    static readonly List<L2ActorLocalLight> Lights = new List<L2ActorLocalLight>(32);
    static readonly List<ActorGroup> Groups = new List<ActorGroup>(32);
    static readonly Dictionary<int, ActorGroup> GroupsByRoot = new Dictionary<int, ActorGroup>(64);
    static readonly int[] Chosen = new int[Max];

    static readonly Vector4[] GlobalPos = new Vector4[Max];
    static readonly Vector4[] GlobalColor = new Vector4[Max];
    static readonly Vector4[] GlobalAtten = new Vector4[Max];

    static readonly int PosId = Shader.PropertyToID("_L2ActorPointPos");
    static readonly int ColorId = Shader.PropertyToID("_L2ActorPointColor");
    static readonly int AttenId = Shader.PropertyToID("_L2ActorPointAtten");

    static float _nextRefresh = -1f;
    static float _nextLog = -1f;
    static bool _rewriteActors;
    const float MoveEpsilonSqr = 0.25f;

    // Captured indoor fire, monster search 20260929_165643.
    static readonly Vector3 ClientC227 = new Vector3(0.130504623f, 0.105057523f, 0.0745209977f);






}
