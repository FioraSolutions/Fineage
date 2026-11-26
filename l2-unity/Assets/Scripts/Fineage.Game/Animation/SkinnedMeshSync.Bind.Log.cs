using UnityEngine;

public partial class SkinnedMeshSync : MonoBehaviour
{
    static string MeshLabel(SkinnedMeshRenderer renderer)
    {
        if (renderer == null)
            return "(null-renderer)";
        Mesh mesh = renderer.sharedMesh;
        return mesh != null ? mesh.name : renderer.name;
    }

    static void LogUnresolvedBone(
        string raceContext,
        SkinnedMeshRenderer renderer,
        string boneName,
        string parentBoneName,
        string reason)
    {
        Debug.LogError(
            "[SkinnedMeshSync] unresolved bone race=" + (raceContext ?? "unknown") +
            " mesh=" + MeshLabel(renderer) +
            " smr=" + (renderer != null ? renderer.name : "(null)") +
            " bone=" + (boneName ?? "(null)") +
            " parent=" + (parentBoneName ?? "(null)") +
            " reason=" + reason);
    }

    static void LogUnresolvedRootBone(string raceContext, SkinnedMeshRenderer renderer, string rootBoneName)
    {
        Debug.LogError(
            "[SkinnedMeshSync] unresolved rootBone race=" + (raceContext ?? "unknown") +
            " mesh=" + MeshLabel(renderer) +
            " smr=" + (renderer != null ? renderer.name : "(null)") +
            " rootBone=" + (rootBoneName ?? "(null)"));
    }

    static void LogZeroBoneHits(string raceContext, SkinnedMeshRenderer renderer)
    {
        Debug.LogError(
            "[SkinnedMeshSync] no skeleton bones matched race=" + (raceContext ?? "unknown") +
            " mesh=" + MeshLabel(renderer) +
            " smr=" + (renderer != null ? renderer.name : "(null)"));
    }
}