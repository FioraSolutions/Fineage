using UnityEngine;

public partial class FreeFlyCamera : MonoBehaviour
{
    private void OnValidate() {
        if(_boostedSpeed < _movementSpeed)
            _boostedSpeed = _movementSpeed;
    }

    private void Start() {
        _initPosition = transform.position;
        _initRotation = transform.eulerAngles;



    }

    private void OnEnable() {
        if(_active)
            _wantedMode = CursorLockMode.Locked;

        Cursor.lockState = _wantedMode = CursorLockMode.None;
    }
}
