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
    public bool IsRotate { get { return _isRotate; } }

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

    public void MoveToPawn(ModelMovePawn targetObj)
    {
        _isFollow = true;
        _targetModelObj = targetObj;
        //ShowDrawLineDebug(transform.position, targetObj.TarObj());
    }


    public void MoveToTargetPosition(Vector3 targetVector)
    {
        _isMove = true;
        _targetMove = targetVector;
        _lastPos = transform.position;
        
    }

    public void RotateObject()
    {
        // пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅ пїЅпїЅпїЅпїЅ
        Vector3 direction = _targetObj.position - transform.position;

        // пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅ Y, пїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅ (пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ, пїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ)
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


 


    /// <summary>
    /// Matches PlayerController.ApplyGravity: grounded bias -2f, airborne
    /// gravity capped at terminal velocity (2500 UU/s → 47.62 m/s).
    /// </summary>
    private float ApplyGravity(float deltaTime)
    {
        if (_controller.isGrounded)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity -= _gravity * deltaTime;
            if (_verticalVelocity < -TerminalVelocity)
                _verticalVelocity = -TerminalVelocity;
        }
        return _verticalVelocity;
    }








}
