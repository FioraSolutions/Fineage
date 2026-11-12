using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CharacterCreator
{
    /// <summary>
    /// Legacy full-gallery spawn. Prefer <see cref="ShowRaceGallery"/> (lazy ≤4) — spawning all
    /// create pawns in one frame crashed Windows D3D12 playtests.
    /// </summary>
    [System.Obsolete("Use ShowRaceGallery / ShowClassGallery / SelectPawn — do not spawn all create pawns at once.")]
    public void SpawnAllPawns() {
        List<Logongrp> pawnData = LogonGrps;

        EnsurePawnContainer();

        for (var i = 8; i < pawnData.Count; i++) {
             Logongrp data = pawnData[i];
             GameObject pawnObject = CreatePawnInterlude(data.RaceId, new PlayerAppearance());
             pawns[i] = pawnObject;
             PlacePawn(pawnObject, data, "Pawn" + i, _pawnContainer);
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
