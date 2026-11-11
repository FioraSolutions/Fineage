using UnityEngine;

public partial class FreeFlyCamera : MonoBehaviour
{
    private void SetCursorState() {
        if(PollInput.EscapeDown) {
            Cursor.lockState = _wantedMode = CursorLockMode.None;
        }

        if(PollInput.LeftMouseDown) {
            _wantedMode = CursorLockMode.Locked;
        }

        // Apply cursor state
        Cursor.lockState = _wantedMode;
        // Hide cursor when locking
        Cursor.visible = (CursorLockMode.Locked != _wantedMode);
    }
}
