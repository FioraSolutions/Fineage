using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class ParticleGroupGpuDrawer
{
    public static bool TryBind(
        Renderer[] particles,
        out Mesh mesh,
        out Material[] materials,
        out int layer,
        out int rendererPriority)
    {
        mesh = null;
        materials = null;
        layer = 0;
        rendererPriority = 0;
        if (particles == null || particles.Length == 0)
            return false;

        Renderer first = particles[0];
        if (first == null || first is SkinnedMeshRenderer)
            return false;

        MeshFilter filter = first.GetComponent<MeshFilter>();
        if (filter == null || filter.sharedMesh == null)
            return false;

        Material[] shared = first.sharedMaterials;
        if (shared == null || shared.Length == 0)
            return false;

        for (int m = 0; m < shared.Length; m++)
        {
            if (!SupportsGpuInstancing(shared[m]))
                return false;
        }

        mesh = filter.sharedMesh;
        for (int i = 1; i < particles.Length; i++)
        {
            Renderer renderer = particles[i];
            if (renderer == null || renderer is SkinnedMeshRenderer)
                return false;

            MeshFilter other = renderer.GetComponent<MeshFilter>();
            if (other == null || other.sharedMesh != mesh)
                return false;
            Material[] otherMats = renderer.sharedMaterials;
            if (otherMats == null || otherMats.Length != shared.Length)
                return false;
            for (int m = 0; m < shared.Length; m++)
            {
                if (otherMats[m] != shared[m])
                    return false;
            }
        }

        materials = shared;
        layer = first.gameObject.layer;
        // Unity 6.0 RenderParams has no sortingLayerID/sortingOrder.
        // MeshRenderer.sortingOrder still applies to GO draws; GPU uses rendererPriority.
        rendererPriority = first.sortingOrder != 0 ? first.sortingOrder : first.rendererPriority;
        for (int m = 0; m < materials.Length; m++)
        {
            if (materials[m] != null)
                materials[m].enableInstancing = true;
        }

        return true;
    }
}
