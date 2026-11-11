public partial class FreeFlyCamera : MonoBehaviour
{
    private void CalculateCurrentIncrease(bool moving) {
        _currentIncrease = Time.deltaTime;

        if(!_enableSpeedAcceleration || _enableSpeedAcceleration && !moving) {
            _currentIncreaseMem = 0;
            return;
        }

        _currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1);
        _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
    }

    private void Update() {
        if(!_active)
            return;

        SetCursorState();

        if(Cursor.visible)
            return;

        // Translation
        if(_enableTranslation) {
            transform.Translate(Vector3.forward * PollInput.ScrollAxis * Time.deltaTime * _translationSpeed);
        }

        // Movement
        if(_enableMovement) {
            Vector3 deltaPosition = Vector3.zero;
            float currentSpeed = _movementSpeed;
            Vector2 move = PollInput.MoveAxes;

            if(PollInput.KeyHeld(_boostSpeed))
                currentSpeed = _boostedSpeed;

            if(move.y > 0)
                deltaPosition += transform.forward;

            if(move.y < 0)
                deltaPosition -= transform.forward;

            if(move.x < 0)
                deltaPosition -= transform.right;

            if(move.x > 0)
                deltaPosition += transform.right;

            if(PollInput.KeyHeld(_moveUp))
                deltaPosition += transform.up;

            if(PollInput.KeyHeld(_moveDown))
                deltaPosition -= transform.up;

            // Calc acceleration
            CalculateCurrentIncrease(deltaPosition != Vector3.zero);

            transform.position += deltaPosition * currentSpeed * _currentIncrease;
        }

        // Rotation
        if(_enableRotation) {
            Vector2 axis = PollInput.MouseAxis;
            // Pitch
            transform.rotation *= Quaternion.AngleAxis(
                -axis.y * _mouseSense,
                Vector3.right
            );

            // Paw
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y + axis.x * _mouseSense,
                transform.eulerAngles.z
            );
        }


        if(PollInput.KeyDown(Key.LeftAlt)) {
            _enableRotation = !_enableRotation;
        }

        // Return to init position
        if(PollInput.KeyDown(_initPositonButton)) {
            transform.position = _initPosition;
            transform.eulerAngles = _initRotation;
        }
    }
}
