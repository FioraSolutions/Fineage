using System;
using Object = UnityEngine.Object;
using UnityEngine;
using System.Collections.Generic;
using VContainer;

public partial class CharacterCreator
{
    public void SpawnPawnWithId(int id) {
        Logongrp pawnData = LogonGrps[id];
        GameObject pawnObject = CreatePawnInterlude(pawnData.RaceId, new PlayerAppearance());
        if (pawnObject == null) {
            pawnObject = FallbackPawn();
        }

        PlacePawn(pawnObject, pawnData, "Pawn" + id, _pawnContainer);
    }

    public CharacterRaceAnimation GetRaceAnimator(int id) {
        return LogonGrps[id].RaceId;
    }

    /// <summary>Interlude race stage (#933): show Fighter M/F + Mystic M/F (Dwarf: fighters only).</summary>
    public void ShowRaceGallery(string race)
    {
        int baseIndex;
        if (!TryRaceBaseIndex(race, out baseIndex))
            return;

        int count = race == "Dwarf" ? 2 : 4;
        SetGalleryVisible(baseIndex, count, selectedIndex: -1);
        currentPawn = null;
        currentPawnIndex = -1;
    }

    /// <summary>Interlude class stage (#933): show Male + Female for the chosen class.</summary>
    public void ShowClassGallery(string race, string pawnClass)
    {
        int baseIndex;
        if (!TryRaceBaseIndex(race, out baseIndex))
            return;

        if (race == "Dwarf" && pawnClass == "Mystic")
            return;

        int classBase = baseIndex + (pawnClass == "Mystic" ? 2 : 0);
        SetGalleryVisible(classBase, 2, selectedIndex: -1);
        currentPawn = null;
        currentPawnIndex = -1;
    }

    public void SelectPawn(string race, string pawnClass, string gender) {
        int index = 0;
        if (!TryRaceBaseIndex(race, out index))
            return;

        if(pawnClass == "Mystic") {
            index += 2;
        }

        if(gender == "Female") {
            index += 1;
        }

        currentPawnIndex = index;
        currentPawn = EnsureCharCreatePawn(index);
        // Sex stage: keep only the selected pawn visible among the race gallery slots.
        int raceBase;
        if (TryRaceBaseIndex(race, out raceBase))
        {
            int raceCount = race == "Dwarf" ? 2 : 4;
            SetGalleryVisible(raceBase, raceCount, selectedIndex: index);
        }
        else if (currentPawn != null)
        {
            currentPawn.SetActive(true);
        }
    }

    public void ResetPawnSelection() {
        HideAllCharCreatePawns();
        currentPawn = null;
        currentPawnIndex = -1;
    }

    static bool TryRaceBaseIndex(string race, out int baseIndex)
    {
        switch (race)
        {
            case "Human":
                baseIndex = 8;
                return true;
            case "Elf":
                baseIndex = 12;
                return true;
            case "Dark Elf":
                baseIndex = 16;
                return true;
            case "Orc":
                baseIndex = 20;
                return true;
            case "Dwarf":
                baseIndex = 24;
                return true;
            default:
                baseIndex = 0;
                return false;
        }
    }

    void SetGalleryVisible(int baseIndex, int count, int selectedIndex)
    {
        HideAllCharCreatePawns();
        for (int i = 0; i < count; i++)
        {
            int index = baseIndex + i;
            GameObject pawn = EnsureCharCreatePawn(index);
            if (pawn == null)
                continue;
            bool show = selectedIndex < 0 || index == selectedIndex;
            pawn.SetActive(show);
        }
    }

    void HideAllCharCreatePawns()
    {
        for (int i = 0; i < pawns.Length; i++)
        {
            if (pawns[i] != null)
                pawns[i].SetActive(false);
        }
    }
}
