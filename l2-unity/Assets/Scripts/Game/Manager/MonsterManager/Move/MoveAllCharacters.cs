using System;

﻿using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.SocialPlatforms;

public partial class MoveAllCharacters : MonoBehaviour, IMoveAllCharacters
{
    private static IMoveAllCharacters _instance;

    [System.Obsolete("Use MoveAllCharactersPacketServices.Moves or [Inject] IMoveAllCharacters instead")]
    public static IMoveAllCharacters Instance { get { return (_instance as UnityEngine.Object) != null ? _instance : null; } }

    private Dictionary<int, MovementData> _moveDict;
    private Dictionary<int, RotateData> _rotateDict;
    private List<int> _removeListMove;
    private List<int> _removeListRotate;
    private Dictionary<int, MovementData> _stoppedMoveData;
    private List<int> _reconciledStoppedMoves;

    private void Awake()
    {
        if ((_instance as UnityEngine.Object) == null)
        {
            _instance = this;
            _moveDict = new Dictionary<int, MovementData>();
            _rotateDict = new Dictionary<int, RotateData>();
            _removeListMove = new List<int>();
            _removeListRotate = new List<int>();
            _stoppedMoveData = new Dictionary<int, MovementData>();
            _reconciledStoppedMoves = new List<int>();
            CrowdAnimTest.EnsureTimeCap();
            MoveAllCharactersPacketServices.Bind(moves: this);
        }
        else
        {
            Destroy(this);
        }
    }

    void Update()
    {
        CrowdAnimTest.EnsureTimeCap();
        if (_moveDict == null)
            return;

        StepMoves();

        if (_rotateDict == null || _rotateDict.Count == 0) return;

        foreach (KeyValuePair<int, RotateData> kvp in _rotateDict)
        {
            RotateData data = kvp.Value;
            bool isRotate = data.IsRotate();

            if (isRotate)
            {
                if (data.IsMonsterDead())
                {
                    _removeListRotate.Add(kvp.Key);
                }
                else if (data.IsEntityTarget() & data.IsEntityMonster())
                {
                    Transform _targetRotation = data.GetTargetTransform();
                    Transform transform = data.GetMonsterTransform();
                    float angleThreshold = data.GetAngleThreshold();

                    if (!VectorUtils.IsFacingAttacker(transform, _targetRotation.position, angleThreshold))
                    {

                        TurnTowardsAttacker(_targetRotation.position, transform);
                    }
                    else
                    {
                        data.SetRotate(false);
                        _removeListRotate.Add(kvp.Key);
                    }
                }
                else
                {
                    _removeListRotate.Add(kvp.Key);
                }

            }
        }

        RemoveFinishKeyRotate();
    }









    void StepMoves()
    {
        ReconcileStoppedMoves();
        if (_moveDict.Count == 0)
        {
            MoveCostLog.NoteStep(0, 0, 0);
            MoveCostLog.EndFrame();
            return;
        }

        long start = System.Diagnostics.Stopwatch.GetTimestamp();
        int moving = 0;
        foreach (KeyValuePair<int, MovementData> kvp in _moveDict)
        {
            MovementData data = kvp.Value;
            bool isMove = data.IsMove();
            MovementTarget movementTarget = data.GetMovementTarget();

            if (isMove == true | data.IsEntity())
            {
                if (movementTarget == null || !movementTarget.TryGetTarget(out Vector3 target))
                {
                    _removeListMove.Add(kvp.Key);
                    continue;
                }

                if (isMove)
                    moving++;
                MoveToPoint(kvp.Key, target, data);
            }
            else
            {

                _removeListMove.Add(kvp.Key);
            }

        }

        RemoveFinishKeyMove();
        double ms = (System.Diagnostics.Stopwatch.GetTimestamp() - start) * 1000.0 / System.Diagnostics.Stopwatch.Frequency;
        MoveCostLog.NoteStep(moving, moving, ms);
        MoveCostLog.EndFrame();
    }






    public void OnDestroy()
    {
        if (ReferenceEquals(_instance, this))
            _instance = null;
        if (_moveDict == null)
            return; // duplicate destroyed in Awake: never initialized
        _moveDict.Clear();
        _rotateDict.Clear();
        _removeListMove.Clear(); 
        _removeListRotate.Clear(); 
        _stoppedMoveData.Clear();
        _reconciledStoppedMoves.Clear();
        _rotateDict.Clear(); 
        _removeListMove.Clear();
        _removeListRotate.Clear();

        _moveDict = null;
        _rotateDict = null;
        _removeListMove = null;
        _removeListRotate = null;
        _stoppedMoveData = null;
        _reconciledStoppedMoves = null;
        _rotateDict = null;
        _removeListMove = null;
        _removeListRotate = null;


        Destroy(gameObject);
    }

}
