using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CharacterCreator : MonoBehaviour
{
    private void Update() {
        if (currentPawn == null) {
            _pawnRotating = false;
            return;
        }

        if(_pawnRotating) {
            if (_pawnRotatingRight) {
                currentPawn.transform.eulerAngles = new Vector3(0, currentPawn.transform.eulerAngles.y + Time.deltaTime * 69f, 0);
            } else {
                currentPawn.transform.eulerAngles = new Vector3(0, currentPawn.transform.eulerAngles.y - Time.deltaTime * 69f, 0);
            }
        }
    }

    GameObject EnsureCharCreatePawn(int index)
    {
        if (index < 0 || index >= pawns.Length)
            return null;
        if (pawns[index] != null)
            return pawns[index];
        List<Logongrp> pawnData = LogonGrps;
        if (pawnData == null || index >= pawnData.Count)
            return null;
        if (_pawnContainer == null) {
            _pawnContainer = new GameObject("Pawns");
            LobbyPresentation.Host(_pawnContainer);
        }

        Logongrp logonGrp = pawnData[index];
        GameObject pawnObject = null;
        try {
            pawnObject = CreatePawn(logonGrp.RaceId, new PlayerAppearance());
        } catch (System.Exception e) {
            Debug.LogWarning("[CharCreate] pawn " + index + " " + logonGrp.RaceId + " failed: " + e.Message);
        }
        if (pawnObject == null) {
            try { pawnObject = FallbackPawn(); } catch (System.Exception e) { Debug.LogWarning("[CharCreate] fallback failed: " + e.Message); }
        }
        if (pawnObject == null)
            return null;

        pawns[index] = pawnObject;
        PlacePawn(pawnObject, logonGrp, "Pawn" + index, _pawnContainer);
        return pawnObject;
    }

    void EnsurePawnContainer()
    {
        DestroyAllPawns();
        _pawnContainer = new GameObject("Pawns");
        LobbyPresentation.Host(_pawnContainer);
    }

    void BindLobbyPawnPresentation(GameObject pawnObject)
    {
        UserGear gear = pawnObject.GetComponent<UserGear>();
        BaseAnimationController animController = pawnObject.GetComponent<BaseAnimationController>();
        if (animController != null)
            animController.Initialize();

        if (gear != null && Animations != null && animController != null)
        {
            string waitState = AnimationNames.WAIT.ToString() + gear.WeaponAnim;
            Animations.PlayLobbyLocomotion(animController, waitState);
        }

        SkinnedMeshSync sync = pawnObject.GetComponentInChildren<SkinnedMeshSync>(true);
        sync?.SyncMesh();
    }
}
