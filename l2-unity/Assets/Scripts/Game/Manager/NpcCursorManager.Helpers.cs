using UnityEngine;
using VContainer;

public partial class NpcCursorManager : MonoBehaviour
{
    void ApplyCursor(int id, Texture2D texture)
    {
        if (texture == null)
            return;

        // Same cursor: re-assert once a second anyway. Something else (CreatorTableWindows' CursorMode.Auto
        // reset, the Editor Game view losing hover) can replace it without NpcCursorManager noticing.
        if (_currentCursor == id && Time.unscaledTime < _reassertAt)
            return;

        bool changed = _currentCursor != id;
        _currentCursor = id;
        _reassertAt = Time.unscaledTime + 1f;
        Cursor.SetCursor(texture, Vector2.zero, CursorMode.ForceSoftware);
        if (changed && CombatTrace.Enabled)
            CombatTrace.Write("cursor.apply", "id=" + id + " tex=" + texture.name + " " + texture.width + "x" + texture.height);
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
        if (_defaultCursor == null && !_loggedMissing)
        {
            _loggedMissing = true;
            Debug.LogError("[Cursor] Default cursor texture did not load (IconManager.LoadCursorByName(\"Default\") returned null); the OS cursor stays.");
        }
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
