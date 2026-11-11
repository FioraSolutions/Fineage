using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2CelestialLut
{
    public struct Sample
    {
        public float tHours;
        public int phase;
        public bool isDay;
        public Vector3 sunDirUnity;
        public Vector3 moonDirUnity;
        public Vector3 sunOffsetUnity;
        public Vector3 moonOffsetUnity;
        public float sunElevDeg;
        public float moonElevDeg;
        public float sunDrawScale;
        public float moonDrawScale;
        public float sunScale;
        public float moonScale;
        public Color moonColor;
        public int sampleA;
        public int sampleB;
        public float lerpU;
    }

    const string MetaFileName = "L2CelestialLUT.csv";

    // Sweep camera from high_elf_moon/sun InverseView (same session).
    static readonly Vector3 SweepCameraUe = new Vector3(324.684f, 264435f, 24536f);

    static readonly List<Sample> Samples = new List<Sample>(320);
    static bool _triedLoad;

    public static bool IsReady => Samples.Count >= 2;







}
