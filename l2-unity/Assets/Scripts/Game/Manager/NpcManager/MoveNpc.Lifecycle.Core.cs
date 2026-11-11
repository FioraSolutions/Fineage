using UnityEngine;

public partial class MoveNpc : MonoBehaviour
{
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _entity = GetComponent<NpcEntity>();
        //var _debugObject = GameObject.FindWithTag("DebugMove");
        //_debugLine = _debugObject.GetComponent<DrawLine>();
    }

    public bool IsSpecialDetectedIsMoveObject()
    {
        if (_isFollow == true | _isMove == true) return true;

        if (transform.position == _lastPosSpecial)
        {
            return false;
        }

        _lastPosSpecial = transform.position;

        return _detectedIsMove;
    }
}
