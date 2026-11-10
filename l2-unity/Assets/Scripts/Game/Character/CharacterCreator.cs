using System;
using Object = UnityEngine.Object;

﻿using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CharacterCreator : MonoBehaviour
{
    [SerializeField] private GameObject[] pawns = new GameObject[26];
    [SerializeField] private int currentPawnIndex = -1;
    [SerializeField] private GameObject currentPawn = null;

    [Inject] private ModelTable _models;
    [Inject] private CharacterBuilder _characterBuilder;
    [Inject] private LogongrpTable _logonGrps;
    [Inject] private IAnimationManager _animations;

    private IAnimationManager Animations
    {
        get { return _animations != null ? _animations : CharacterPacketServices.Animations; }
    }

    private ModelTable Models
    {
        get { return _models != null ? _models : TablePacketServices.Models; }
    }

    private CharacterBuilder Builder
    {
        get { return _characterBuilder != null ? _characterBuilder : CharacterPacketServices.Builder; }
    }

    private List<Logongrp> LogonGrps => TablePacketServices.LogonGrps.LogonGrps;

    private bool _pawnRotating = false;
    private bool _pawnRotatingRight = true;

    private GameObject _pawnContainer;
    public int PawnIndex { get { return currentPawnIndex; } }

    private static CharacterCreator _instance;
    [System.Obsolete("Use LoginPacketServices.Creator instead")]
        public static CharacterCreator Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance == null) {
            _instance = this;
        } else if(_instance != this) {
            Destroy(gameObject);
        }

        WireCharacterPacketServices();
    }

    private void Start()
    {
        WireCharacterPacketServices();
    }

    private void WireCharacterPacketServices()
    {
        CharacterPacketServices.Bind(animations: _animations, builder: _characterBuilder);
        TablePacketServices.Bind(logonGrps: _logonGrps);
    }

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


    /// <summary>
    /// Char create: spawn pawns lazily (on <see cref="SelectPawn"/>). Instantiating all 18 race/class
    /// pawns in one frame native-crashed UnityPlayer on Windows D3D12 playtest builds.
    /// </summary>

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

    /// <summary>
    /// Drop create-gallery while still in lobby (back to select). World enter unloads Menu.
    /// </summary>

    void EnsurePawnContainer()
    {
        DestroyAllPawns();
        _pawnContainer = new GameObject("Pawns");
        LobbyPresentation.Host(_pawnContainer);
    }




    



    /// <summary>
    /// After activation: wait pose then bind skinned gear onto the anim skeleton.
    /// Equip may have run while inactive; wait pose moves bones, so sync once at the end.
    /// </summary>
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
