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
        // Slot layout is style-major: each style owns HAIR_COLOR_COUNT colour pairs.
        // The previous `hairStyle * 8` assumed 4 styles per block and ran past the end of
        // the table for style 6, so orcs asked for an unwritten slot and rendered the
        // wrong (or no) hair.
        byte index = (byte)(hairStyle * (HAIR_COLOR_COUNT * 2) + hairColor * 2);
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
}
