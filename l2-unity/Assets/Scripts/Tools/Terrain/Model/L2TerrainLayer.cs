#if (UNITY_EDITOR) 
using UnityEngine;
using System;

[System.Serializable]
public class L2TerrainLayer {
    /// <summary>Unreal Texture='Package.Name' reference from TerrainInfo (for bake error messages).</summary>
    public string diffuseTextureRef { get; set; }
    public Texture2D texture { get; set; }
    public Texture2D alphaMap { get; set; }
    public float uScale { get; set; }
    public float vScale { get; set; }
    public float uOffset { get; set; }
    public float vOffset { get; set; }
    /// <summary>Official TextureRotation in Unreal rotator units (65536 = 360°).</summary>
    public float textureRotation { get; set; }
    /// <summary>0 = TEXMAPAXIS_XY, 1 = XZ, 2 = YZ.</summary>
    public int textureMapAxis { get; set; }
    public bool useAlpha { get; set; }
    public Vector3 layerScale { get; set; }
}
#endif
