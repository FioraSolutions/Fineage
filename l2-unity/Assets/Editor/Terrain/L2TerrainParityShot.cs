#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Deterministic batchmode shot of Talking Island tile 17_25 at the obelisk pose.
/// GPU editor only. Do not pass -nographics.
///
/// FINEAGE_TERRAIN_SHOT=/tmp/17_25-obelisk.png \
/// "/Applications/Unity/Unity.app/Contents/MacOS/Unity" -batchmode -quit \
///   -projectPath l2-unity \
///   -executeMethod L2TerrainParityShot.RunTalkingIslandObelisk \
///   -logFile -
///
/// Pose is the LevelInfo CameraLocationDynamic used for the #1827 shots:
/// UE (-83488.5, 243203.719, -3373.358), pitch 62228, yaw -28184.
/// Unity position (4632.45, -64.25, -1590.26), euler (18.17, -154.83, 0).
/// </summary>
public static class L2TerrainParityShot
{
    const string TileId = "17_25";
    const string TileRoot = "Assets/Resources/Data/Maps/17_25/";
    const int Width = 1920;
    const int Height = 1080;
    const float Fov = 60f;
    const float Near = 0.3f;
    const float Far = 8000f;
    static readonly Vector3 Position = new Vector3(4632.45f, -64.25f, -1590.26f);
    static readonly Vector3 Euler = new Vector3(18.17f, -154.83f, 0f);

    public static void RunTalkingIslandObelisk()
    {
        string outPath = Environment.GetEnvironmentVariable("FINEAGE_TERRAIN_SHOT");
        if (string.IsNullOrEmpty(outPath))
        {
            Debug.LogError("[TerrainShot] FINEAGE_TERRAIN_SHOT is not set. Refusing to write a png into the repo.");
            EditorApplication.Exit(1);
            return;
        }

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        PlaceTileContent(TileId);
        var go = new GameObject("L2TerrainParityShot");
        Camera cam = go.AddComponent<Camera>();
        cam.transform.position = Position;
        cam.transform.rotation = Quaternion.Euler(Euler);
        cam.fieldOfView = Fov;
        cam.nearClipPlane = Near;
        cam.farClipPlane = Far;
        cam.allowHDR = false;
        cam.allowMSAA = false;
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.enabled = true;

        var rt = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 1;
        cam.targetTexture = rt;
        cam.Render();

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;
        var tex = new Texture2D(Width, Height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
        tex.Apply(false, false);
        RenderTexture.active = previous;
        cam.targetTexture = null;

        string dir = Path.GetDirectoryName(outPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllBytes(outPath, tex.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(tex);
        UnityEngine.Object.DestroyImmediate(rt);
        Debug.Log("[TerrainShot] wrote " + outPath
            + " tile=" + TileRoot + TileId + ".prefab"
            + " pos=" + Position.ToString("F2")
            + " euler=" + Euler.ToString("F2")
            + " fov=" + Fov
            + " " + Width + "x" + Height);
        EditorApplication.Exit(0);
    }

    static void PlaceTileContent(string tileId)
    {
        string[] parts =
        {
            tileId,
            tileId + "_DecoLayer",
            "StaticMeshes",
            "Brushes",
            tileId + "_AmbientSounds"
        };
        for (int i = 0; i < parts.Length; i++)
        {
            string path = "Assets/" + MapTileLoadPolicy.MapRoot + "/" + tileId + "/" + parts[i] + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                if (parts[i] == tileId)
                    throw MapTileLoadPolicy.Missing(tileId);
                continue;
            }

            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            go.transform.SetPositionAndRotation(prefab.transform.position, prefab.transform.rotation);
        }
    }
}
#endif
