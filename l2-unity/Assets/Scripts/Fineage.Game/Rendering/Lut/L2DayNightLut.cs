using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2DayNightLut
{
    public struct Sample
    {
        public float tHours;
        public int phase;
        public Color sky;
        public Color sun;
        public Color actorAmbient;
    }

    const string MetaFileName = "L2DayNightLUT.csv";

    static readonly List<Sample> Samples = new List<Sample>(320);
    // high_elf_soon.rdc EID 3067: midday raw #427CB0 -> final #62ADEA.
    // Stored as an additive display-LUT offset so brighter dawn colors do not
    // clip as aggressively as another RGB multiply.
    static readonly Color PostSkyDayOffset =
        new Color((0x62 - 0x42) / 255f, (0xAD - 0x7C) / 255f, (0xEA - 0xB0) / 255f, 0f);
    static bool _triedLoad;

    public static bool IsReady => Samples.Count >= 2;





    /// <summary>
    /// Applies the display-referred L2 post color to the sky material only.
    /// Night is unchanged; correction fades in 06–08 and out 22–24.
    /// No pow/gamma operation is used.
    /// </summary>



}
