using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class NetworkCharacterControllerShare : MonoBehaviour
{
    private void FixedUpdate() {
        PlayerController found = LocalPlayer;
        if (found == null || _networkTransformShare == null)
            return;

        IPlayerPawn player = found;
        Vector3 newDirection = player.MoveDirection.normalized;
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (ShouldShareMoveDirection(newDirection, now)) {
            _lastSharingTimestamp = now;
            _lastDirection = newDirection;

            if (VectorUtils.IsVectorZero2D(newDirection)) {
                _networkTransformShare.SharePosition();
                _networkTransformShare.ShouldShareRotation = false;
            } else {
                _networkTransformShare.ShouldShareRotation = true;
            }

            ShareMoveDirection(newDirection);
        }
    }

    private bool ShouldShareMoveDirection(Vector3 newDirection, long timestamp) {
        if (VectorUtils.IsVectorZero2D(_lastDirection) && !VectorUtils.IsVectorZero2D(newDirection)) {
            // player just moved
            return true;
        }

        if (!VectorUtils.IsVectorZero2D(_lastDirection) && VectorUtils.IsVectorZero2D(newDirection)) {
            // player just stopped
            return true;
        }

        // Basic loop delay
        if (timestamp - _lastSharingTimestamp >= _sharingLoopDelayMs && newDirection != _lastDirection) {
            return true;
        }

        return false;
    }

    public void ShareMoveDirection(Vector3 moveDirection) {
        _lastDirection = moveDirection;
    }

    public void ForceShareMoveDirection() {
        PlayerController found = LocalPlayer;
        if (found == null)
            return;
        IPlayerPawn player = found;
        ShareMoveDirection(player.MoveDirection.normalized);
    }
}
