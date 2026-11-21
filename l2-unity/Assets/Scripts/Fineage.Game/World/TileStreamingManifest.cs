using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileStreamingManifest", menuName = "Fineage/Tile Streaming Manifest")]
public sealed class TileStreamingManifest : ScriptableObject
{
    [Tooltip("Map tile ids (e.g. 16_25) with Resources/Data/Maps/{id}/… prefabs.")]
    public string[] prefabTileIds = System.Array.Empty<string>();

    HashSet<string> _set;
    static TileStreamingManifest _cached;

    public static bool Contains(string tileId)
    {
        if (string.IsNullOrEmpty(tileId))
            return false;

        TileStreamingManifest manifest = Load();
        if (manifest == null)
            return false;

        manifest.EnsureSet();
        return manifest._set.Contains(tileId);
    }

    static TileStreamingManifest Load()
    {
        if (_cached != null)
            return _cached;

        const string path = "Settings/TileStreamingManifest";
        _cached = Resources.Load<TileStreamingManifest>(path);
        if (_cached != null)
            return _cached;

        FineageLog.Error(
            FineageLogCategory.World,
            "TileStreamingManifest missing at Resources/" + path
                + "; prefab tile streaming disabled (build scenes or add manifest).");
        return null;
    }

    void EnsureSet()
    {
        if (_set != null)
            return;

        _set = new HashSet<string>(prefabTileIds.Length);
        for (int i = 0; i < prefabTileIds.Length; i++)
        {
            string id = prefabTileIds[i];
            if (!string.IsNullOrEmpty(id))
                _set.Add(id);
        }
    }
}