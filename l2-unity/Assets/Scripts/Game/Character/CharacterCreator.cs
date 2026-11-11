using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;
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






    /// <summary>
    /// Char create: spawn pawns lazily (on <see cref="SelectPawn"/>). Instantiating all 18 race/class
    /// pawns in one frame native-crashed UnityPlayer on Windows D3D12 playtest builds.
    /// </summary>


    /// <summary>
    /// Drop create-gallery while still in lobby (back to select). World enter unloads Menu.
    /// </summary>





    



    /// <summary>
    /// After activation: wait pose then bind skinned gear onto the anim skeleton.
    /// Equip may have run while inactive; wait pose moves bones, so sync once at the end.
    /// </summary>








}
