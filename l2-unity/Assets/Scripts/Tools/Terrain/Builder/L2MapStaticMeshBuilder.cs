#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class L2MapStaticMeshBuilder : MonoBehaviour
{
    /// <summary>
    /// Interlude FBX (umodel .pskx -> Blender psk import -> FBX) keep UE local X on Unity X, while placement maps
    /// UE X -> Unity Z ((y,z,x)/52.5). Net effect: every mesh was turned -90 deg about Y around its own pivot
    /// (Mother Tree ring/house/bridge at 21_19). Interlude bake sets this to +90 = mesh-local pre-rotation.
    /// Original Shnok assets keep 0.
    /// </summary>
    public static float MeshLocalYawFix = 0f;

    [MenuItem("Shnok/[StaticMeshes] (JSON) Build static meshes")]
    static void BuildSoundsMenu() {
        string title = "Select ambient sound list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "json";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            // Debug.Log("Selected file: " + fileToProcess);
            L2JSONStaticMeshActorImporter meshActorParser = new L2JSONStaticMeshActorImporter();
            L2StaticMeshActor data = meshActorParser.GetL2StaticMeshActorFromFile(fileToProcess);
            GenerateStaticMeshes(data);
        }
    }

    public static void GenerateStaticMeshes(L2StaticMeshActor staticMeshActor) {
        float ueToUnityUnitScale = 1 / 52.5f;
        Vector3 basePosition = new Vector3(staticMeshActor.y, staticMeshActor.z, staticMeshActor.x) * ueToUnityUnitScale;
        GameObject staticMeshesGo = new GameObject("StaticMeshes");
        GameObject container = new GameObject("StaticMeshes");
        staticMeshesGo.transform.parent = container.transform;

        foreach (var staticMesh in staticMeshActor.staticMeshes) {
            BuildSingleStaticMesh(staticMesh, container);
        }
    }

    static void ApplyAxisScale(ref Vector3 s, Vector3 localAxis, float v) {
        Vector3 a = new Vector3(Mathf.Abs(localAxis.x), Mathf.Abs(localAxis.y), Mathf.Abs(localAxis.z));
        if (a.x >= a.y && a.x >= a.z) s.x *= v;
        else if (a.y >= a.z) s.y *= v;
        else s.z *= v;
    }

    public static void BuildSingleStaticMesh(L2StaticMesh staticMesh, GameObject container) {
        Vector3 basePosition = Vector3.zero;
        string meshPath = StaticMeshUtils.GetMeshPath(staticMesh.staticMesh);
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(meshPath);
        if (go != null) {
            Vector3 position = new Vector3(staticMesh.y, staticMesh.z, staticMesh.x) * (1/52.5f) * L2TerrainGeneratorTool.MAP_SCALE;
            Quaternion rot = Quaternion.Euler(VectorUtils.ConvertRotToUnity(staticMesh.eulerAngles)) *
                             Quaternion.Euler(0f, MeshLocalYawFix, 0f) * go.transform.rotation;

            float meshDataScaleMultiplier = staticMesh.scale != 0 ? staticMesh.scale : 1f;
            float meshDataScaleX = staticMesh.scaleX != 0 ? staticMesh.scaleX : 1f;
            float meshDataScaleY = staticMesh.scaleY != 0 ? staticMesh.scaleY : 1f;
            float meshDataScaleZ = staticMesh.scaleZ != 0 ? staticMesh.scaleZ : 1f;
            Vector3 meshDataScale = new Vector3(meshDataScaleX, meshDataScaleY, meshDataScaleZ);
            if (staticMesh.hasDrawScale3D) {
                // UE actor-local X/Y/Z == converted frame Z/X/Y (same swap as positions).
                // Map each onto the instance-local axis it lands on after MeshLocalYawFix * go.rotation.
                Quaternion inv = Quaternion.Inverse(Quaternion.Euler(0f, MeshLocalYawFix, 0f) * go.transform.rotation);
                Vector3 s3 = Vector3.one;
                ApplyAxisScale(ref s3, inv * Vector3.forward, staticMesh.drawScale3D.x);
                ApplyAxisScale(ref s3, inv * Vector3.right, staticMesh.drawScale3D.y);
                ApplyAxisScale(ref s3, inv * Vector3.up, staticMesh.drawScale3D.z);
                meshDataScale = Vector3.Scale(meshDataScale, s3);
            }

            GameObject instantiated = GameObject.Instantiate(go, position + basePosition, rot);
            instantiated.name = staticMesh.staticMesh;
            instantiated.transform.localScale = Vector3.Scale(instantiated.transform.localScale, meshDataScale) *
                meshDataScaleMultiplier *
                //ueToUnityUnitScale *
                L2TerrainGeneratorTool.MAP_SCALE;

            instantiated.transform.parent = container.transform;
        } else {
            // Debug.LogError("Can't find StaticMesh FBX " + staticMesh.staticMesh + " at path " + meshPath);
        }
    }
}
#endif
