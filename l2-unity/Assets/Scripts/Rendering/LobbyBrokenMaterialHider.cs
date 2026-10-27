using UnityEngine;

/// <summary>
/// Lobby ships/FX often land with null slots or Hidden/InternalErrorShader (magenta on Metal/URP).
/// Call after additive l2_lobby load — earlier passes miss those renderers.
/// </summary>
public static class LobbyBrokenMaterialHider
{
    public static int HideBrokenRenderers(string reason = null)
    {
        var rends = Object.FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        int hidden = 0;
        for (int i = 0; i < rends.Length; i++)
        {
            Renderer r = rends[i];
            if (r == null || !r.enabled)
                continue;

            if (!IsBroken(r))
                continue;

            r.enabled = false;
            hidden++;
        }

        Debug.Log("[LobbyBrokenMaterialHider] hidden=" + hidden +
                  " scanned=" + rends.Length +
                  (string.IsNullOrEmpty(reason) ? "" : " reason=" + reason));
        return hidden;
    }

    static bool IsBroken(Renderer r)
    {
        var mats = r.sharedMaterials;
        if (mats == null || mats.Length == 0)
            return true;

        for (int m = 0; m < mats.Length; m++)
        {
            Material mat = mats[m];
            if (mat == null)
                return true;

            Shader shader = mat.shader;
            if (shader == null)
                return true;

            string name = shader.name;
            if (string.IsNullOrEmpty(name))
                return true;

            // Unity pink/magenta error shader + common missing-shader fallbacks.
            if (name == "Hidden/InternalErrorShader" ||
                name == "Hidden/InternalErrorShader (Obsolete)" ||
                name.IndexOf("InternalError", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                name == "Hidden/Pink" ||
                name.EndsWith("(Missing)", System.StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
