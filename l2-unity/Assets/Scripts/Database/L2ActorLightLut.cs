using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2ActorLightLut
{
    public struct Sample
    {
        public float tHours;
        public Color ambient;
        public Color ambientHalf;
        public Color sunPlane;
        public Color sunBase;
    }

    const string MetaFileName = "L2ActorLightLUT.csv";

    // Not applied by GetColorPlane. Kept here until a full-day c250/plane log says otherwise.
    public const float DirectionalScale = 0.7f;

    static readonly int ActorAmbientHalfId = Shader.PropertyToID("_L2ActorAmbientHalf");
    static readonly int HsvActorLightId = Shader.PropertyToID("_L2HsvActorLight");
    static readonly int HsvActorBaseId = Shader.PropertyToID("_L2HsvActorBase");
    static readonly int ActorDirectionalScaleId = Shader.PropertyToID("_L2ActorDirectionalScale");

    static readonly List<Sample> Samples = new List<Sample>(320);
    static bool _triedLoad;

    public static bool IsReady => Samples.Count >= 2;





}
