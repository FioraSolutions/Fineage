using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// G writes the actor light numbers the monster shader uses.
/// Same names as the client hook constants: c249 direction, c250 sun color, c251 ambient half.
/// The same key also writes the live Katerina materials against the RenderDoc capture.
/// </summary>
public static partial class L2ActorLightCapture
{
    public const string Root =
        @"C:\games\Lineage II HighElfes\ADEU-P464-D20240703-P-240117-240724-1\system\logs\MonsterSearch\unity";

    // monster_008, game hour 16.976. Direction is L2 axes, Z up.
    static readonly Vector4 ClientC249 = new Vector4(0.342226118f, 0.469668895f, -0.813813388f, 0f);
    static readonly Vector4 ClientC250 = new Vector4(0.248003364f, 0.248003364f, 0.248003364f, 1f);
    static readonly Vector4 ClientC251 = new Vector4(0.235294119f, 0.235294119f, 0.235294119f, 0f);


    const int KaterinaNpcId = 30004;
    const string EnvShader = "L2/World/ActorEnvAdd";
    const float ExpectedCutoff = 0.627451f;

    static string AppendKaterina(StringBuilder text)
    {
        Entity[] entities = UnityEngine.Object.FindObjectsByType<Entity>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var hits = new List<Entity>();
        for (int i = 0; i < entities.Length; i++)
        {
            Entity entity = entities[i];
            if (entity == null || entity.Identity == null)
                continue;
            if (!IsKaterina(entity.Identity))
                continue;
            hits.Add(entity);
        }

        text.AppendLine("katerina_count=" + hits.Count);
        if (hits.Count == 0)
        {
            text.AppendLine("katerina_match=missing");
            text.AppendLine("katerina_note=npc 30004 is not spawned. Stand where Katerina is loaded, Game view focused, press G again.");
            return "Katerina нет в сцене";
        }

        Camera camera = Camera.main;
        Vector3 cameraPos = camera != null ? camera.transform.position : Vector3.zero;
        Entity nearest = hits[0];
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < hits.Count; i++)
        {
            float distance = Vector3.Distance(cameraPos, hits[i].transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = hits[i];
            }
        }

        bool slot0 = false;
        bool slot1 = false;
        for (int i = 0; i < hits.Count; i++)
        {
            Entity entity = hits[i];
            bool nearestOne = entity == nearest;
            float distance = Vector3.Distance(cameraPos, entity.transform.position);
            text.AppendLine("npc id=" + entity.Identity.Id +
                " npcId=" + entity.Identity.NpcId +
                " name=" + entity.Identity.Name +
                " title=" + entity.Identity.Title +
                " class=" + entity.Identity.NpcClass +
                " dist=" + Num(distance) +
                " nearest=" + (nearestOne ? "1" : "0"));
            if (!nearestOne)
                continue;

            SkinnedMeshRenderer[] renderers = entity.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            for (int r = 0; r < renderers.Length; r++)
            {
                SkinnedMeshRenderer renderer = renderers[r];
                if (renderer == null)
                    continue;
                Material[] slots = renderer.sharedMaterials;
                text.AppendLine("renderer=" + renderer.name + " slots=" + (slots == null ? 0 : slots.Length));
                if (slots == null)
                    continue;
                for (int s = 0; s < slots.Length; s++)
                {
                    int verdict = DescribeSlot(text, s, slots[s]);
                    if (s == 0 && verdict == 1)
                        slot0 = true;
                    if (s == 1 && verdict == 1)
                        slot1 = true;
                }
            }
        }

        bool allMatch = slot0 && slot1;
        text.AppendLine("katerina_slot0=" + (slot0 ? "1" : "0"));
        text.AppendLine("katerina_slot1=" + (slot1 ? "1" : "0"));

        text.AppendLine("katerina_match=" + (allMatch ? "yes" : "no"));
        text.AppendLine("expected_shader=" + EnvShader);
        text.AppendLine("expected_cutoff=" + Num(ExpectedCutoff));
        text.AppendLine("expected_cull=0");
        text.AppendLine("expected_alphaClip=1");
        text.AppendLine("expected_slot0=t00_ori t00_sp n3gold01");
        text.AppendLine("expected_slot1=t01_ori t01_sp n3gold01");
        return "Katerina match=" + (allMatch ? "yes" : "no") + " count=" + hits.Count;
    }

    static bool IsKaterina(EntityIdentity identity)
    {
        if (identity.NpcId == KaterinaNpcId)
            return true;
        string name = identity.Name;
        if (string.IsNullOrEmpty(name))
            return false;
        return name.IndexOf("katerina", StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("ekaterin", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static int DescribeSlot(StringBuilder text, int slot, Material material)
    {
        if (material == null)
        {
            text.AppendLine("slot" + slot + "=null");
            return 0;
        }

        string shader = material.shader != null ? material.shader.name : "";
        Texture baseMap = material.HasProperty("_BaseMap") ? material.GetTexture("_BaseMap") : null;
        Texture spec = material.HasProperty("_SpecMask") ? material.GetTexture("_SpecMask") : null;
        Texture env = material.HasProperty("_EnvMap") ? material.GetTexture("_EnvMap") : null;
        float cutoff = material.HasProperty("_Cutoff") ? material.GetFloat("_Cutoff") : -1f;
        float cull = material.HasProperty("_Cull") ? material.GetFloat("_Cull") : -1f;
        float alphaClip = material.HasProperty("_AlphaClip") ? material.GetFloat("_AlphaClip") : -1f;
        bool keyword = material.IsKeywordEnabled("_ALPHATEST_ON");
        string baseName = baseMap != null ? baseMap.name : "";
        string specName = spec != null ? spec.name : "";
        string envName = env != null ? env.name : "";

        bool envSlot = shader == EnvShader;
        bool textures;
        if (slot == 0)
            textures = Contains(baseName, "t00_ori") && Contains(specName, "t00_sp") && Contains(envName, "n3gold01");
        else if (slot == 1)
            textures = Contains(baseName, "t01_ori") && Contains(specName, "t01_sp") && Contains(envName, "n3gold01");
        else
            textures = true;

        bool flags = envSlot
            && keyword
            && alphaClip > 0.5f
            && Mathf.Abs(cull) < 0.01f
            && Mathf.Abs(cutoff - ExpectedCutoff) < 0.002f;
        bool ok = envSlot && textures && flags;

        text.AppendLine("slot" + slot +
            " material=" + material.name +
            " shader=" + shader +
            " keyword=" + (keyword ? "1" : "0") +
            " cutoff=" + Num(cutoff) +
            " cull=" + Num(cull) +
            " alphaClip=" + Num(alphaClip) +
            " queue=" + material.renderQueue +
            " base=" + baseName +
            " spec=" + specName +
            " env=" + envName +
            " ok=" + (ok ? "1" : "0"));
        if (!envSlot && slot > 1)
            return 1;
        return ok ? 1 : 0;
    }

}
