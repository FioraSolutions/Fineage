using UnityEngine;

public partial class MoveNpc
{
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
