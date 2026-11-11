using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public static partial class L2ActorLightCapture
{
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
}
