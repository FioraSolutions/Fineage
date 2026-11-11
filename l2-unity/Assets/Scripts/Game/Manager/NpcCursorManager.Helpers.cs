using UnityEngine;
using VContainer;

public partial class NpcCursorManager : MonoBehaviour
{
    void ApplyCursor(int id, Texture2D texture)
    {
        if (texture == null)
            return;

        // ForceSoftware: hardware cursors often fail to stick on macOS with Input System only.
        if (_currentCursor == id)
        {
            Cursor.SetCursor(texture, Vector2.zero, CursorMode.ForceSoftware);
            return;
        }

        _currentCursor = id;
        Cursor.SetCursor(texture, Vector2.zero, CursorMode.ForceSoftware);
    }

    void TryReloadCursors()
    {
        IconManager icons = IconPacketServices.Icons;
        if (icons == null)
            return;
        _defaultCursor = icons.LoadCursorByName("Default");
        _hoverCursorAtk = icons.LoadCursorByName("Attack");
        _hoverCursorTalk = icons.LoadCursorByName("Talk");
        _hoverCursorPickup = icons.LoadCursorByName("Pickup");
        _currentCursor = -1;
    }

    static Camera ResolvePickCamera()
    {
        CameraController controller = CameraPacketServices.Controller;
        if (controller != null)
        {
            Camera cam = controller.GetComponent<Camera>();
            if (cam != null && cam.enabled && cam.gameObject.activeInHierarchy)
                return cam;
        }

        return Camera.main;
    }

    private void WireManagerInteractionPacketServices()
    {
        ManagerInteractionPacketServices.Bind(dropPicker: _dropPicker);
    }
}
