using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2HazeLut
{
    public struct Sample
    {
        public float tHours;
        public Color haze;
    }

    const string MetaFileName = "L2HazeLUT.csv";

    // HazeRing_Final Color=(B=224,G=203,R=188,A=255)
    public static readonly Color ColorModifierDefault =
        new Color(188f / 255f, 203f / 255f, 224f / 255f, 1f);

    static readonly List<Sample> Samples = new List<Sample>(320);
    static bool _triedLoad;

    public static bool IsReady => Samples.Count >= 2;




}
