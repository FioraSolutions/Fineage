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







    sealed class ActorGroup
    {
        public Transform Root;
        public int RootId;
        public bool Applied;
        public int AppliedChosen;
        public Vector3 AppliedOrigin;
        public readonly List<Renderer> Renderers = new List<Renderer>(4);
        public readonly MaterialPropertyBlock Block = new MaterialPropertyBlock();
        public readonly Vector4[] Pos = new Vector4[Max];
        public readonly Vector4[] Color = new Vector4[Max];
        public readonly Vector4[] Atten = new Vector4[Max];

        public Vector3 Origin
        {
            get
            {
                Vector3 sum = Vector3.zero;
                int count = 0;
                for (int i = 0; i < Renderers.Count; i++)
                {
                    Renderer renderer = Renderers[i];
                    if (renderer == null)
                        continue;
                    sum += renderer.bounds.center;
                    count++;
                }

                return count > 0 ? sum / count : Root.position;
            }
        }
    }

}
