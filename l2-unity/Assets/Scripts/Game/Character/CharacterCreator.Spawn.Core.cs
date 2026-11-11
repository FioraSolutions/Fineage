using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

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
}
