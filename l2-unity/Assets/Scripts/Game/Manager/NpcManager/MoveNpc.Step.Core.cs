using UnityEngine;

public partial class MoveNpc
{
    private float GetMonsterSpeed()
    {
        return (_entity.Running) ? _entity.Stats.UnitySpeedRun : _entity.Stats.UnitySpeedWalking;
    }

    private bool IsRotating()
    {
        // пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ
        float rotationDifference = Quaternion.Angle(_lastRotation, transform.rotation);

        // пїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ, пїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ
        return rotationDifference > rotationThreshold;
    }

    public void RotateInTargetObject(Transform targetObj)
    {
        _targetObj = targetObj;
        _isRotate = true;
        _lastRotation = transform.rotation;
    }

    public void StopRotateObject()
    {
        _isRotate = false;
    }
}
