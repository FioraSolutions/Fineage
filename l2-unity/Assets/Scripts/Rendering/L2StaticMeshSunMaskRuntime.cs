using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Installs remapped per-instance L2 sun visibility bytes into TEXCOORD3.
/// The byte contains one visibility bit for each of the eight three-hour slots.
/// </summary>
public sealed partial class L2StaticMeshSunMaskRuntime : MonoBehaviour
{
    [Serializable]
    public sealed class MapData
    {
        public string map;
        public ActorData[] actors;
    }

    [Serializable]
    public sealed class ActorData
    {
        public string actor;
        public string mesh;
        public string location;
        public int vertexCount;
        public string visibility;
        public bool hasUnityPosition;
        public Vector3 unityPosition;

        public Vector3 UnityPosition =>
            hasUnityPosition
                ? unityPosition
                : L2StaticMeshVertexLight.ToUnityPosition(
                    L2StaticMeshSunMaskDatabase.ParseVector(location));
    }

    const bool ShowSunMaskDebug = false;
    // TEMPORARY. Per-mesh sun masks are not applied while the Unity sun is tuned.
    static bool LoadSunMasks = false;
    static readonly int HasMaskId = Shader.PropertyToID("_L2HasSunMask");
    static readonly int ForceSunVisibleId =
        Shader.PropertyToID("_L2StaticMeshForceSunVisible");
    static readonly int SunMaskDebugId = Shader.PropertyToID("_L2SunMaskDebug");
    static bool _installed;

    Mesh _streamMesh;
    MeshRenderer _renderer;
    bool _ownsStream;

    sealed class CombinedStreamData
    {
        public readonly Mesh mesh;
        public readonly float[] packed;
        public readonly List<MeshFilter> filters = new List<MeshFilter>();

        public CombinedStreamData(Mesh source)
        {
            mesh = source;
            packed = new float[source.vertexCount];
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (_installed)
            return;
        _installed = true;
        if (!LoadSunMasks)
        {
            // Debug.Log("[L2StaticMeshSunMask] load paused");
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        for (int i = 0; i < SceneManager.sceneCount; i++)
            ApplyScene(SceneManager.GetSceneAt(i));
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyScene(scene);
    }

}
