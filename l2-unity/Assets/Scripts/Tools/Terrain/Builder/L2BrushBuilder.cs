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
    /// CSG is not evaluated: additive brush hulls are drawn as-is (L2 BSP brushes are mostly decks/floors/stairs).
    /// </summary>

    static Material _fallback;
}
#endif