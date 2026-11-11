using System;
using System.Collections;
using UnityEngine;
using VContainer;

public partial class NetworkTransformReceive : MonoBehaviour
{
    [Inject] World _world;

    private World GameWorld => NetworkingPacketServices.GameWorld;
    [SerializeField] private Vector3 _serverPosition;
    private Vector3 _lastPos;
    private float _newRotation;
    private float _posLerpValue;
    [SerializeField] private bool _positionSyncProtection = true;
    private float _positionSyncNodesThreshold = 3f;
    [SerializeField] private bool _positionSynced = false;
    [SerializeField] private bool _positionSyncPaused = false;
    private float _lerpDuration = 0.1f;
    [SerializeField] private float _positionDelta;
    [SerializeField] private long _lastDesyncTime = 0;
    [SerializeField] private long _lastDesyncDuration = 0;
    private long _maximumAllowedDesyncTimeMs = 0;



    /* Set new theorical position */

    /* Safety measure to keep the transform position synced */


    /* Ajust rotation */






}
