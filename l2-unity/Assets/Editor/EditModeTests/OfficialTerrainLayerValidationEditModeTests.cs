#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class OfficialTerrainLayerValidationEditModeTests
{
    [Test]
    public void OverlayWithTextureButNoAlphaMapLogsError()
    {
        var layers = new List<L2TerrainLayer>
        {
            new L2TerrainLayer { diffuseTextureRef = "Texture=T_16_25.Height.16_25_R" },
            new L2TerrainLayer
            {
                diffuseTextureRef = "Texture=T_sland.WR_02",
                texture = Texture2D.blackTexture
            }
        };
        LogAssert.Expect(LogType.Error, new Regex(
            @"\[Interlude terrain\] 16_25 official layer 1 has Texture=T_sland\.WR_02 but no AlphaMap field"));
        L2TerrainInfoParser.ValidateOfficialUvLayers("16_25", layers);
    }

    [Test]
    public void BaseLayerWithoutAlphaMapDoesNotLog()
    {
        var layers = new List<L2TerrainLayer>
        {
            new L2TerrainLayer
            {
                diffuseTextureRef = "Texture=T_19_17.Height.19_17_R",
                texture = Texture2D.blackTexture
            }
        };
        L2TerrainInfoParser.ValidateOfficialUvLayers("19_17", layers);
        LogAssert.NoUnexpectedReceived();
    }
}
#endif
