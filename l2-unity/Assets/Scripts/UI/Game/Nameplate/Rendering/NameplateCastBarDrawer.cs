using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateCastBarDrawer
{
    // Same Target/ tree as HeadDisplay bubbles.
    const string FillResourcePath = "Data/UI/Target/HeadDisplay_DF_Gauge_Cast/HeadDisplay_DF_Gauge_Cast";
    const string BgResourcePath = "Data/UI/Target/HeadDisplay_DF_Gauge_Cast/HeadDisplay_DF_Gauge_Cast_Bg";

    /// <summary>Source atlas width. Opaque fill ink spans <see cref="ContentU0"/>..<see cref="ContentU1"/>.</summary>
    const float AtlasWidth = 128f;
    /// <summary>Inclusive-exclusive opaque columns in the fill PNG (left-packed ink).</summary>
    const float ContentU0 = 2f;
    const float ContentU1 = 78f;

    /// <summary>Drawn size — stretched from the cropped ink to Interlude-like proportions.</summary>
    const float GaugeWidth = 104f;
    const float GaugeHeight = 4f;
    const float GapBelowNamePx = 6f;

    readonly L2NameplateScreenBatch _batchBg = new L2NameplateScreenBatch();
    readonly L2NameplateScreenBatch _batchFill = new L2NameplateScreenBatch();

    Texture2D _texBg;
    Texture2D _texFill;



}
