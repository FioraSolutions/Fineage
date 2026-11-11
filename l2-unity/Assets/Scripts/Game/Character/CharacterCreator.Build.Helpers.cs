using System;
using Object = UnityEngine.Object;
using UnityEngine;
using System.Collections.Generic;
using VContainer;

public partial class CharacterCreator
{
    public void UpdatePawnPosAndRot(GameObject pawnObject, Logongrp pawnData) {
        Vector3 pawnPosition = new Vector3(pawnData.X, pawnData.Y, pawnData.Z);
        pawnPosition = VectorUtils.ConvertPosToUnity(pawnPosition);
        pawnObject.transform.position = pawnPosition;
        pawnObject.transform.eulerAngles = new Vector3(0, 360.00f * pawnData.Yaw / 65536, 0);
        // Pin the slot as destination so a pawn never slides back to its Instantiate origin.
        SelectableCharacterEntity selectable = pawnObject.GetComponent<SelectableCharacterEntity>();
        if (selectable != null)
            selectable.SetDestination(pawnData);
    }

    public void RotatePawn(bool right) {
        _pawnRotating = true;
        _pawnRotatingRight = right;
    }

    public void StopRotatingPawn() {
        _pawnRotating = false;
    }

    public GameObject FallbackPawn() {
        return CreatePawn(CharacterRaceAnimation.FFighter, new PlayerAppearance());
    }
}
