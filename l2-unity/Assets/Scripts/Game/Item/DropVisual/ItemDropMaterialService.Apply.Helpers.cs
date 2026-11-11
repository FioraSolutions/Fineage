using UnityEngine;

public sealed partial class ItemDropMaterialService
{
    static void ApplyDropMaskedTextures(GameObject visual, Texture2D tex0, Texture2D tex1)
    {
        if (visual == null)
            return;

        Shader shader = Shader.Find(DropMaskedShaderName);
        if (shader == null || tex0 == null)
            return;

        MeshRenderer[] renderers = visual.GetComponentsInChildren<MeshRenderer>(true);
        for (int r = 0; r < renderers.Length; r++)
        {
            MeshRenderer renderer = renderers[r];
            int slots = renderer.sharedMaterials != null ? renderer.sharedMaterials.Length : 1;
            if (slots < 1)
                slots = 1;

            Material[] mats = new Material[slots];
            for (int i = 0; i < slots; i++)
            {
                mats[i] = new Material(shader);
                mats[i].name = "DropCoin_" + i;
                mats[i].SetTexture("_MainTex", i == 0 || tex1 == null ? tex0 : tex1);
                mats[i].SetTextureScale("_MainTex", Vector2.one);
                mats[i].SetTextureOffset("_MainTex", Vector2.zero);
                mats[i].SetVector("_MainTex_ST", new Vector4(1f, 1f, 0f, 0f));
                mats[i].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }

            renderer.sharedMaterials = mats;
            renderer.enabled = true;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.allowOcclusionWhenDynamic = false;
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            renderer.localBounds = new Bounds(Vector3.zero, Vector3.one * 2f);
        }
    }

    static void ApplyUrpLit(GameObject visual, Texture2D albedo)
    {
        if (visual == null || albedo == null)
            return;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            return;

        Material source = new Material(shader);
        source.name = "DropItemLit";
        if (source.HasProperty("_BaseMap"))
            source.SetTexture("_BaseMap", albedo);
        if (source.HasProperty("_MainTex"))
            source.SetTexture("_MainTex", albedo);
        if (source.HasProperty("_BaseColor"))
            source.SetColor("_BaseColor", Color.white);
        if (source.HasProperty("_Color"))
            source.SetColor("_Color", Color.white);
        if (source.HasProperty("_Cull"))
            source.SetFloat("_Cull", 0f);
        ApplyClonedMaterials(visual, source);
        Object.Destroy(source);
    }

    static void ApplyClonedMaterials(GameObject visual, Material source)
    {
        if (visual == null || source == null)
            return;

        MeshRenderer[] renderers = visual.GetComponentsInChildren<MeshRenderer>(true);
        for (int r = 0; r < renderers.Length; r++)
        {
            MeshRenderer renderer = renderers[r];
            int slots = renderer.sharedMaterials != null ? renderer.sharedMaterials.Length : 1;
            if (slots < 1)
                slots = 1;

            Material[] mats = new Material[slots];
            for (int i = 0; i < slots; i++)
            {
                mats[i] = new Material(source);
                mats[i].name = source.name + "_" + i;
                if (mats[i].HasProperty("_Cull"))
                    mats[i].SetFloat("_Cull", 0f);
            }

            renderer.sharedMaterials = mats;
            renderer.enabled = true;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.allowOcclusionWhenDynamic = false;
            renderer.localBounds = new Bounds(Vector3.zero, Vector3.one * 2f);
        }
    }
}
