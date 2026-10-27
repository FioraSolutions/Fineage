using UnityEngine;

/// <summary>
/// Lightweight bootstrap that initialises data tables in the Preview scene
/// without login, networking, or VContainer DI.
/// Used by the Content Preview editor tool.
/// </summary>
public class PreviewSceneBootstrap : MonoBehaviour
{
    private bool _loaded = false;

    private void Awake()
    {
        EnsureTablesLoaded();
    }

    /// <summary>
    /// Idempotent — safe to call many times.
    /// Initialises the singleton data tables that CharacterBuilder / EntitySpawnShared need.
    /// </summary>
    public void EnsureTablesLoaded()
    {
        if (_loaded) return;

        // ItemTable must be first: ModelTable.CacheWeapons reads ItemTable.Weapons
        ItemTable.Instance.Initialize();
        ItemTable.Instance.CacheItems();

        NpcgrpTable.Instance.Initialize();
        NpcNameTable.Instance.Initialize();
        ModelTable.Instance.Initialize();

        // CharacterBuilder is a MonoBehaviour singleton. If none exists, create one.
        if (CharacterBuilder.Instance == null)
        {
            var builderGo = new GameObject("CharacterBuilder_Preview");
            builderGo.AddComponent<CharacterBuilder>();
        }

        _loaded = true;
        Debug.Log("[ContentPreview] Data tables loaded (offline mode).");
    }
}
