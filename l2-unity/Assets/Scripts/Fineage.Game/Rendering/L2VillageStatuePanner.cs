using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Dark Elf village center. O_De_02 is the streak leaving the statue hand
/// (<c>De_Statue004</c>, a vertical beam). O_De_01's second slot is
/// <c>DE_statue002_s</c>, whose original specular is TexPanner <c>DE_panner_01</c>
/// (PanRate 0.05 along +U, texture <c>DE_V_pan</c>).
/// </summary>
public sealed class L2VillageStatuePanner : MonoBehaviour
{
    struct Slot
    {
        public Material Material;
        public Vector2 Rate;
    }

    Slot[] _slots = System.Array.Empty<Slot>();
    float _time;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Rescan();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Rescan();

    void Rescan()
    {
        var found = new System.Collections.Generic.List<Slot>();
        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] shared = renderers[i].sharedMaterials;
            bool touch = false;
            for (int m = 0; m < shared.Length; m++)
            {
                if (RateFor(shared[m] != null ? shared[m].name : null) != Vector2.zero)
                {
                    touch = true;
                    break;
                }
            }
            if (!touch)
                continue;

            Material[] instanced = renderers[i].materials;
            for (int m = 0; m < instanced.Length; m++)
            {
                Material mat = instanced[m];
                Vector2 rate = RateFor(mat != null ? mat.name : null);
                if (rate == Vector2.zero)
                    continue;
                found.Add(new Slot { Material = mat, Rate = rate });
            }
        }

        _slots = found.ToArray();
    }

    static Vector2 RateFor(string name)
    {
        if (name == "De_Statue004_shader")
            return new Vector2(0f, 1.2f);
        if (name == "DE_statue002_s")
            return new Vector2(0.05f, 0f);
        return Vector2.zero;
    }

    void Update()
    {
        if (_slots.Length == 0)
            return;
        _time += Time.deltaTime;
        for (int i = 0; i < _slots.Length; i++)
        {
            Material mat = _slots[i].Material;
            if (mat == null)
                continue;
            Vector2 offset = _slots[i].Rate * _time;
            if (mat.HasProperty("_BaseMap"))
                mat.SetTextureOffset("_BaseMap", offset);
            if (mat.HasProperty("_EmissionMap"))
                mat.SetTextureOffset("_EmissionMap", offset);
            if (mat.HasProperty("_MainTex"))
                mat.SetTextureOffset("_MainTex", offset);
        }
    }
}
