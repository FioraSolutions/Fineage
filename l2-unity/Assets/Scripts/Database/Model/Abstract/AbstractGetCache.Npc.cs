using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public abstract partial class AbstractGetCache
{
    public GameObject GetContainer(CharacterRaceAnimation raceId, EntityType entityType)
    {
        GameObject go = null;

        switch (entityType)
        {
            case EntityType.User:
                go = _userContainers[(byte)raceId];
                break;
            case EntityType.Player:
                go = _playerContainers[(byte)raceId];
                break;
            case EntityType.Pawn:
                go = _pawnContainers[(byte)raceId];
                break;
        }

        if (go == null)
        {
            // Debug.LogError($"Can't find container for race {raceId} and entity type {entityType}");
        }

        return go;
    }

    public GameObject GetFace(CharacterRaceAnimation raceId, byte face) {
        GameObject go = _faces[(byte)raceId, face];
        if (go == null) {
            // Debug.LogError($"Can't find face {face} for race {raceId} at index {raceId},{face}");
        }

        return go;
    }

    public GameObject GetHair(CharacterRaceAnimation raceId, byte hairStyle, byte hairColor, bool bh) {
        byte index = (byte)(hairStyle * 8 + hairColor * 2);
        index = GetRemappedIndex(raceId, index);
        if (bh) {
            index += 1;
        }

        //Debug.Log($"Loading hair[{index}] Race:{raceId} Model:{hairStyle}_{hairColor}_{(bh ? "bh" : "ah")}");

        GameObject go = _hair[(byte)raceId, index];
        if (go == null) {
            // Debug.LogError($"Can't find hairstyle {hairStyle} haircolor {hairColor} for race {raceId} at index {raceId},{index}");
        }

        return go;
    }

    private byte GetRemappedIndex(CharacterRaceAnimation raceId, byte index) {
        if (CharacterRaceAnimation.FFighter == raceId) {
            return index switch {
                8 => 6,
                10 => 8,
                12 => 10,
                _ => index
            };
        }
        return index;
    }

    public GameObject GetNpc(string meshname)
    {
        L2Npc npc = null;
        _npcs.TryGetValue(meshname, out npc);
        
        if (npc == null)
        {
            // Debug.LogWarning($"Can't find npc {meshname} model in ModelTable.");
            return null;
        }

        AddMaterial(npc);
        
        return npc.baseModel;
    }

    private void AddMaterial(L2Npc npc)
    {
        if (npc.allMaterials.Count == 0 || npc.baseModel == null)
            return;

        Material[] materials = npc.allMaterials.Values.First();
        if (materials == null || materials.Length == 0)
            return;

        SkinnedMeshRenderer[] renderers = npc.baseModel.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        SkinnedMeshRenderer fallback = null;
        int fallbackSlots = -1;
        bool assigned = false;
        int emptyCount = 0;
        for (int r = 0; r < renderers.Length; r++)
        {
            if (renderers[r] == null)
                continue;
            int slots = renderers[r].sharedMaterials != null ? renderers[r].sharedMaterials.Length : 0;
            if (slots == 0)
                emptyCount++;
        }

        int filledEmpty = 0;
        for (int r = 0; r < renderers.Length; r++)
        {
            SkinnedMeshRenderer renderer = renderers[r];
            if (renderer == null)
                continue;
            int slotCount = renderer.sharedMaterials != null ? renderer.sharedMaterials.Length : 0;
            if (slotCount > fallbackSlots)
            {
                fallback = renderer;
                fallbackSlots = slotCount;
            }
            // Fresh PSK→FBX imports often arrive with zero material slots
            // (materialImportMode=None). One mesh → dump the full npcgrp set;
            // several empty SMRs → one texture each in order.
            if (slotCount == 0)
            {
                if (emptyCount <= 1)
                    renderer.sharedMaterials = materials;
                else if (filledEmpty < materials.Length && materials[filledEmpty] != null)
                {
                    renderer.sharedMaterial = materials[filledEmpty];
                    filledEmpty++;
                }
                else
                    continue;
                renderer.enabled = true;
                assigned = true;
                continue;
            }
            if (slotCount >= materials.Length && ApplyNpcMaterials(renderer, materials))
                assigned = true;
        }

        if (!assigned && fallback != null)
            ApplyNpcMaterials(fallback, materials);
    }

    static bool ApplyNpcMaterials(SkinnedMeshRenderer renderer, Material[] materials)
    {
        if (renderer == null || materials == null || materials.Length == 0)
            return false;

        Material[] slots = renderer.sharedMaterials;
        if (slots == null || slots.Length == 0)
        {
            renderer.sharedMaterials = materials;
            renderer.enabled = true;
            return true;
        }

        int count = slots.Length < materials.Length ? slots.Length : materials.Length;
        Material[] next = new Material[slots.Length];
        bool changed = false;
        for (int i = 0; i < slots.Length; i++)
            next[i] = slots[i];
        for (int i = 0; i < count; i++)
        {
            if (materials[i] == null || next[i] == materials[i])
                continue;
            next[i] = materials[i];
            changed = true;
        }

        if (changed)
            renderer.sharedMaterials = next;
        return changed || count > 0;
    }
}
