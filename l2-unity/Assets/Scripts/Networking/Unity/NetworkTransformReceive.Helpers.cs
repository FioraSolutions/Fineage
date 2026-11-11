using System;
using System.Collections;
using UnityEngine;
using VContainer;

public partial class NetworkTransformReceive : MonoBehaviour
{
    public void SetFinalRotation(float finalRotation) {
        _newRotation = finalRotation;
    }

    public void LookAt(Transform target) {
        if (target != null) {
            _newRotation = VectorUtils.CalculateMoveDirectionAngle(transform.position, target.position);
        }
    }

    public void LookAt(Vector3 position) {
        _newRotation = VectorUtils.CalculateMoveDirectionAngle(transform.position, position);
    }

    public bool IsPositionSynced() {
        return _positionSynced;
    }

    public void PausePositionSync() {
        _positionSyncPaused = true;
        _positionSynced = true;
    }

    public void ResumePositionSync() {
        _positionSyncPaused = false;
    }
}
