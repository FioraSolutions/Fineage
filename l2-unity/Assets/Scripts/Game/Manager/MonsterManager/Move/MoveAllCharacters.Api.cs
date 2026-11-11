using System;
using System.Security.Cryptography;
using UnityEngine.InputSystem.XR;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class MoveAllCharacters
{
    public void AddRotate(int id, RotateData data)
    {
        if (!_rotateDict.ContainsKey(id))
        {

            _rotateDict.Add(id, data);
        }
        else
        {
            _rotateDict[id] = data;
        }
    }

    public void AddMoveData(int id, MovementData data)
    {
        MovementData prev;
        if (_moveDict.TryGetValue(id, out prev) && prev != null && prev.IsMove())
            data.InheritMoveTimer(prev);
        _moveDict[id] = data;
    }

    public void CancelMove(int objId)
    {
        if (_moveDict.ContainsKey(objId))
        {
            MovementData data = _moveDict[objId];
            data.SetIsMove(false);
        }
    }

    public void CancelRotate(int objId)
    {
        if (_rotateDict.ContainsKey(objId))
            _rotateDict.Remove(objId);
    }

    public void Forget(int objId)
    {
        _moveDict.Remove(objId);
        _rotateDict.Remove(objId);
        _stoppedMoveData.Remove(objId);
        _removeListMove.Remove(objId);
        _removeListRotate.Remove(objId);
        _reconciledStoppedMoves.Remove(objId);
    }

    public bool IsMoving(int objId)
    {
        if (_moveDict.ContainsKey(objId))
        {
            MovementData data = _moveDict[objId];
            return data.IsMove();
        }

        return false;
    }

    public MovementData GetMove(int objId)
    {
        MovementData data;
        return _moveDict.TryGetValue(objId, out data) ? data : null;
    }

}
