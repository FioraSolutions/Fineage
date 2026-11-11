using System;
using System.Security.Cryptography;
using UnityEngine.InputSystem.XR;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class MoveAllCharacters
{
    private void RemoveFinishKeyMove()
    {
        foreach (int key in _removeListMove)
        {
            MovementData data;
            if (_moveDict.TryGetValue(key, out data))
            {
                _stoppedMoveData[key] = data;
                _moveDict.Remove(key);
            }
        }

        _removeListMove.Clear();
    }

    private void ReconcileStoppedMoves()
    {
        if (_stoppedMoveData == null || _stoppedMoveData.Count == 0)
            return;

        foreach (KeyValuePair<int, MovementData> pair in _stoppedMoveData)
        {
            MovementData current;
            if (_moveDict.TryGetValue(pair.Key, out current) &&
                current != null &&
                current.IsMove())
            {
                _reconciledStoppedMoves.Add(pair.Key);
                continue;
            }

            if (pair.Value == null || pair.Value.ReconcileStoppedAnimation())
                _reconciledStoppedMoves.Add(pair.Key);
        }

        for (int i = 0; i < _reconciledStoppedMoves.Count; i++)
            _stoppedMoveData.Remove(_reconciledStoppedMoves[i]);
        _reconciledStoppedMoves.Clear();
    }

    private void RemoveFinishKeyRotate()
    {
        foreach (int key in _removeListRotate)
        {
            if (_rotateDict.ContainsKey(key))
            {
                _rotateDict.Remove(key);
            }
        }

        _removeListRotate.Clear();
    }
}
