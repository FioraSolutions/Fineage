using UnityEngine;

public static partial class L2ActorMaterialSetup
{
    /// <summary>
    /// Runtime paperdoll/lobby armor: swap URP/Lit imports to ActorModulate2X with the same albedo.
    /// </summary>
    public static Material RemediatePaperdollMaterial(Material loaded, string textureContext)
    {
        if (loaded == null)
            return null;

        string shaderName = loaded.shader != null ? loaded.shader.name : string.Empty;
        if (shaderName.IndexOf("ActorModulate2X", System.StringComparison.Ordinal) >= 0)
            return loaded;

        if (shaderName.IndexOf("Universal Render Pipeline", System.StringComparison.OrdinalIgnoreCase) < 0
            && shaderName.IndexOf("Standard", System.StringComparison.OrdinalIgnoreCase) < 0)
        {
            ValidatePaperdollActorMaterial(loaded, textureContext);
            return loaded;
        }

        Shader actor = Shader.Find(ActorShaderName);
        if (actor == null)
        {
            Debug.LogError(
                "[L2ActorMaterial] cannot remediate paperdoll shader missing "
                + ActorShaderName
                + " material="
                + loaded.name
                + " context="
                + textureContext);
            return loaded;
        }

        Texture albedo = loaded.HasProperty("_BaseMap") ? loaded.GetTexture("_BaseMap") : null;
        if (albedo == null && loaded.HasProperty("_MainTex"))
            albedo = loaded.GetTexture("_MainTex");

        var inst = new Material(actor) { name = loaded.name + "_actor" };
        if (albedo != null)
        {
            if (inst.HasProperty("_BaseMap"))
                inst.SetTexture("_BaseMap", albedo);
            if (inst.HasProperty("_MainTex"))
                inst.SetTexture("_MainTex", albedo);
        }

        SetupResult resolve = Resolve(textureContext ?? loaded.name, null);
        inst.SetColor("_BaseColor", Color.white);
        inst.doubleSidedGI = true;
        if (inst.HasProperty("_Cull"))
            inst.SetFloat("_Cull", 0f);

        if (resolve.mode == SurfaceMode.AlphaCutout)
        {
            float cutoff = CutoffFromProps(textureContext ?? loaded.name, null);
            if (inst.HasProperty("_AlphaClip"))
                inst.SetFloat("_AlphaClip", 1f);
            if (inst.HasProperty("_Cutoff"))
                inst.SetFloat("_Cutoff", cutoff);
            inst.EnableKeyword("_ALPHATEST_ON");
        }

        Debug.LogWarning(
            "[L2ActorMaterial] remediated paperdoll material="
            + loaded.name
            + " from "
            + shaderName
            + " to "
            + ActorShaderName
            + " context="
            + textureContext);
        return inst;
    }
}