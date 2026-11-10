using System;
using Object = UnityEngine.Object;

public partial class CharacterCreator
{
    public void SpawnAllPawns() {
        List<Logongrp> pawnData = LogonGrps;

        EnsurePawnContainer();

        for (var i = 8; i < pawnData.Count; i++) {
             GameObject pawnObject = CreatePawn(CharacterRaceAnimation.FDarkElf, new PlayerAppearance());
             pawns[i] = pawnObject;
             PlacePawn(pawnObject, pawnData[i], "Pawn" + i, _pawnContainer);
        }
    }

    public void SpawnAllCharCreatePawns()
    {
        EnsurePawnContainer();
    }

    public void DestroyAllPawns()
    {
        if (_pawnContainer != null)
        {
            Object.Destroy(_pawnContainer);
            _pawnContainer = null;
        }

        for (int i = 0; i < pawns.Length; i++)
            pawns[i] = null;

        currentPawn = null;
        currentPawnIndex = -1;
        _pawnRotating = false;
    }

    public void SpawnPawnWithId(int id) {
        Logongrp pawnData = LogonGrps[id];
        GameObject pawnObject = CreatePawn(pawnData.RaceId, new PlayerAppearance());
        if (pawnObject == null) {
            pawnObject = FallbackPawn();
        }

        PlacePawn(pawnObject, pawnData, "Pawn" + id, _pawnContainer);
    }

    public CharacterRaceAnimation GetRaceAnimator(int id) {
        return LogonGrps[id].RaceId;
    }

    public void SelectPawn(string race, string pawnClass, string gender) {
        int index = 0;
        switch(race) {
            case "Human":
                index = 8;
                break;
            case "Elf":
                index = 12;
                break;
            case "Dark Elf":
                index = 16;
                break;
            case "Orc":
                index = 20;
                break;
            case "Dwarf":
                index = 24;
                break;
        }

        if(pawnClass == "Mystic") {
            index += 2;
        }

        if(gender == "Female") {
            index += 1;
        }

        currentPawnIndex = index;
        currentPawn = EnsureCharCreatePawn(index);
    }

    public void ResetPawnSelection() {
        if (currentPawn != null) {
            // Restore pawn appearance and rotation
            // Restore lazily: next SelectPawn rebuilds a fresh pawn for this slot.
            Destroy(currentPawn);
            if (currentPawnIndex >= 0 && currentPawnIndex < pawns.Length)
                pawns[currentPawnIndex] = null;
        }

        currentPawn = null;
        currentPawnIndex = -1;
    }

}
