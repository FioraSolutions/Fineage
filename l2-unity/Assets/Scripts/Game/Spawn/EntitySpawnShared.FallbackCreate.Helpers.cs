using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static partial class EntitySpawnShared
{
    static string ReplaceIgnoreCase(string text, string from, string to)
    {
        int idx = text.IndexOf(from, System.StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return text;
        return text.Substring(0, idx) + to + text.Substring(idx + from.Length);
    }

    static bool IsMonsterMesh(string mesh) =>
        !string.IsNullOrEmpty(mesh) &&
        mesh.StartsWith("LineageMonster", System.StringComparison.OrdinalIgnoreCase);

    static bool TryResolveFallbackMonsterPrefab(
        ModelTable models,
        string wantedMesh,
        out GameObject prefab,
        out string usedMesh)
    {
        prefab = null;
        usedMesh = null;
        if (models == null || string.IsNullOrEmpty(wantedMesh))
            return false;

        int dot = wantedMesh.IndexOf('.');
        if (dot <= 0 || dot == wantedMesh.Length - 1)
            return false;

        string package = wantedMesh.Substring(0, dot);
        string asset = wantedMesh.Substring(dot + 1);

        // Exact rig first, so a mesh added to the art pack later wins over the stand-in.
        prefab = models.GetNpc(package + "." + asset);
        if (prefab != null)
        {
            usedMesh = package + "." + asset;
            return true;
        }

        // Missing monster art stays missing. A goblin/orc stand-in was showing on every
        // unimported mob (Langk lizardmen, keltirs, the rest of the field).
        // Treasure chests / mimics are an exception: Interlude ids 18265–18298 and 21801–21822
        // use box/mimic rigs the art pack does not ship, and without a stand-in the NpcInfo is dropped.
        if (IsChestMeshAsset(asset) && TryCreateChestStandIn(out prefab, out usedMesh))
            return true;
        return false;
    }

    static bool IsChestMeshAsset(string asset)
    {
        if (string.IsNullOrEmpty(asset))
            return false;
        if (asset.IndexOf("boxer", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return false;
        if (asset.Equals("box_m00", System.StringComparison.OrdinalIgnoreCase))
            return true;
        if (asset.IndexOf("box_m00", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return true;
        return asset.IndexOf("mimic", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static GameObject _chestStandIn;

    /// <summary>
    /// Runtime chest prop with <see cref="MonsterEntity"/> so MonsterSpawner will keep it.
    /// Visual is the dropped-item crate if the art pack has it, otherwise a two-cube lid chest.
    /// </summary>
    static bool TryCreateChestStandIn(out GameObject prefab, out string usedMesh)
    {
        usedMesh = "FineageStandIn.treasure_chest";
        if (_chestStandIn != null)
        {
            prefab = _chestStandIn;
            return true;
        }

        var root = new GameObject("treasure_chest_standin");
        root.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(root);

        GameObject dropBox = Resources.Load<GameObject>("Data/StaticMeshes/dropitems/drop_box_m00");
        if (dropBox != null)
        {
            GameObject child = UnityEngine.Object.Instantiate(dropBox, root.transform);
            child.name = "drop_box_m00";
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            FitVisualHeight(child, 0.35f);
        }
        else
        {
            BuildPrimitiveChest(root.transform);
        }

        var controller = root.AddComponent<CharacterController>();
        controller.height = 0.35f;
        controller.radius = 0.18f;
        controller.center = new Vector3(0f, 0.175f, 0f);
        controller.slopeLimit = 45f;
        controller.stepOffset = 0.05f;
        root.AddComponent<MonsterEntity>();

        _chestStandIn = root;
        prefab = root;
        return true;
    }

    static void FitVisualHeight(GameObject visual, float targetHeight)
    {
        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0 || targetHeight <= 0f)
            return;
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);
        if (bounds.size.y <= 0.0001f)
            return;
        float scale = targetHeight / bounds.size.y;
        visual.transform.localScale = visual.transform.localScale * scale;
        visual.transform.localPosition = new Vector3(0f, -bounds.min.y * scale, 0f);
    }

    static void BuildPrimitiveChest(Transform parent)
    {
        var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "chest_body";
        body.transform.SetParent(parent, false);
        body.transform.localScale = new Vector3(0.36f, 0.22f, 0.24f);
        body.transform.localPosition = new Vector3(0f, 0.11f, 0f);
        ApplyChestColor(body, new Color(0.45f, 0.28f, 0.12f));

        var lid = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lid.name = "chest_lid";
        lid.transform.SetParent(parent, false);
        lid.transform.localScale = new Vector3(0.38f, 0.08f, 0.26f);
        lid.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        ApplyChestColor(lid, new Color(0.55f, 0.36f, 0.14f));

        var band = GameObject.CreatePrimitive(PrimitiveType.Cube);
        band.name = "chest_band";
        band.transform.SetParent(parent, false);
        band.transform.localScale = new Vector3(0.06f, 0.24f, 0.245f);
        band.transform.localPosition = new Vector3(0f, 0.13f, 0f);
        ApplyChestColor(band, new Color(0.72f, 0.58f, 0.18f));
    }

    static void ApplyChestColor(GameObject go, Color color)
    {
        var renderer = go.GetComponent<Renderer>();
        if (renderer == null)
            return;
        Shader shader = Shader.Find("Universal Render Pipeline/Lit")
            ?? Shader.Find("Unlit/Color")
            ?? Shader.Find("Standard");
        Material material = shader != null
            ? new Material(shader)
            : (renderer.sharedMaterial != null ? new Material(renderer.sharedMaterial) : null);
        if (material == null)
            return;
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color"))
            material.color = color;
        renderer.sharedMaterial = material;
        var collider = go.GetComponent<Collider>();
        if (collider != null)
            UnityEngine.Object.DestroyImmediate(collider);
    }
}
