using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2CloudLut
{
    public struct Sample
    {
        public float tHours;
        public Color sheet;
        public Color silhouette;
    }

    const string MetaFileName = "L2CloudLUT.csv";

    public static readonly Color SheetFallback =
        new Color(130f / 255f, 130f / 255f, 139f / 255f, 1f);
    public static readonly Color SilhouetteFallback =
        new Color(37f / 255f, 14f / 255f, 5f / 255f, 1f);

    static readonly List<Sample> Samples = new List<Sample>(320);
    static bool _triedLoad;

    public static bool IsReady => Samples.Count >= 2;




}
