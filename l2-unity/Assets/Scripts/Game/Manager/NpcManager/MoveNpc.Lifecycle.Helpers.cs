using UnityEngine;

public partial class MoveNpc : MonoBehaviour
{
    private void FixedUpdate()
    {
    
            if (_isRotate)
            {
                // пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ, пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ
                if (IsRotating())
                {
                    // Debug.Log("пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ. " + transform.position);
                }
                else
                {
                    if (_targetObj != null)
                    {
                        RotateObject();
                    }

                }
                _lastRotation = transform.rotation;
            }

            if (_isFollow)
            {

                _flatTransformPos = new Vector3(transform.position.x, 0, transform.position.z);

                if (!_entity.IsDead())
                {

                    var cVector = new Vector3(_targetModelObj.TarEntity().transform.position.x, 0, _targetModelObj.TarEntity().transform.position.z);

                    if (Vector2.Distance(cVector, _flatTransformPos) <= _targetModelObj.Distance())
                    {
                        OnFinish(_targetModelObj.TarEntity().transform.position);
                        _isFollow = false;
                    }

                    if (Vector2.Distance(cVector, _flatTransformPos) >= _targetModelObj.Distance())
                    {
                        MoveToPoint(_targetModelObj.TarEntity().transform.position, true);
                    }
                }
            }
            else if (_isMove == true)
            {
                if (_targetMove == null)
                    return;
                //Debug.Log("NPC MOVEEEEEEEEEEEE POSIIITIIIION");
                MoveToPoint(_targetMove, false);
            }

        
    }

    public void CancelMove()
    {
        if (_isMove)
        {
            _isMove = false;
        }
    }
}
