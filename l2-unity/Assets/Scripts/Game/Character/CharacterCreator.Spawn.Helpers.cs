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
        if (!LobbyCharCreateStageCatalog.TryRaceBaseIndex(race, out int baseIndex))
            return;

        int count = LobbyCharCreateStageCatalog.RaceGalleryCount(race);
        SetGalleryVisible(baseIndex, count, selectedIndex: -1);
        currentPawn = null;
        currentPawnIndex = -1;
    }

    /// <summary>Interlude class stage (#933): show Male + Female for the chosen class.</summary>
    public void ShowClassGallery(string race, string pawnClass)
    {
        if (race == "Dwarf" && pawnClass == "Mystic")
            return;

        int classBase = LobbyCharCreateStageCatalog.ClassGalleryBase(race, pawnClass);
        if (classBase < 0)
            return;

        SetGalleryVisible(classBase, 2, selectedIndex: -1);
        currentPawn = null;
        currentPawnIndex = -1;
    }

    public void SelectPawn(string race, string pawnClass, string gender) {
        if (!LobbyCharCreateStageCatalog.TryPawnIndex(race, pawnClass, gender, out int index))
            return;

        currentPawnIndex = index;
        currentPawn = EnsureCharCreatePawn(index);
        // Sex stage: keep only the selected pawn visible among the race gallery slots.
        if (LobbyCharCreateStageCatalog.TryRaceBaseIndex(race, out int raceBase))
        {
            int raceCount = LobbyCharCreateStageCatalog.RaceGalleryCount(race);
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
