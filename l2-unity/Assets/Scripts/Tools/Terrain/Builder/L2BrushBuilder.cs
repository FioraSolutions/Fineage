#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public partial class L2BrushBuilder
{



    /// <summary>
    /// Interlude BSP (tools/interlude/BrushDump.java -> Brush actors in the tile t3d): one merged Mesh asset
    /// (submesh per material, UVs from UE TextureU/V, both windings so decks render + collide from either side)
    /// with a MeshCollider on layer Brush. The old ProBuilder path left meshes unsaved in the prefab and no colliders.
    /// CSG_Subtract brushes carve only CSG_Add brushes that precede them in the dump (UE2 brush order).
    /// </summary>

    static Material _fallback;
}
#endif