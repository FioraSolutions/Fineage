using UnityEngine;
using VContainer;

public partial class NpcCursorManager : MonoBehaviour
{
    private void Awake()
    {
        App.InjectGameObject(gameObject);
        WireManagerInteractionPacketServices();
    }

    void Start()
    {
        WireManagerInteractionPacketServices();
        // Icons may not be bound yet when the component is added at spawn (#1907); Update retries.
        TryReloadCursors();
        if (CombatTrace.Enabled)
            CombatTrace.Write("cursor.manager", "go=" + gameObject.name + " icons=" + (IconPacketServices.Icons != null) +
                " default=" + (_defaultCursor != null) + " attack=" + (_hoverCursorAtk != null));
    }

    // The Editor Game view (and the OS on alt-tab) drops a custom cursor when focus leaves; re-apply on return.
    void OnApplicationFocus(bool focused)
    {
        if (focused)
            _currentCursor = -1;
    }

    void Update()
    {
        if (_defaultCursor == null)
            TryReloadCursors();

        Camera cam = ResolvePickCamera();
        if (cam == null)
        {
            ApplyCursor(DefaultCursorId, _defaultCursor);
            return;
        }

        Ray ray = cam.ScreenPointToRay(PollInput.MousePosition);
        ItemEntity dropEntity = null;
        ItemDropPicker dropPicker = DropPicker;
        bool dropHit = dropPicker != null &&
            dropPicker.TryPick(ray, PickDistance, _stickyDropItem, out dropEntity, out _);
        _stickyDropItem = dropHit ? dropEntity : null;

        if (dropHit && dropEntity != null)
        {
            ApplyCursor(PickupCursorId, _hoverCursorPickup);
            return;
        }

        // MouseOverUI sticks after the target window opens. The sword cursor
        // then never comes back while the pointer is on the mob. Trust a fresh pick.
        L2UI ui = SystemUIPacketServices.Ui;
        if (ui != null && ui.IsPointerOverPanel())
        {
            ApplyCursor(DefaultCursorId, _defaultCursor);
            return;
        }

        // Hover cursor SoT (#930): Attackable / talkable on hover, not only when targeted.
        if (!EntityPick.TryPick(ray, PickDistance, out Entity entity, out _))
        {
            ApplyCursor(DefaultCursorId, _defaultCursor);
            return;
        }

        ApplyEntityCursor(entity);
    }

    void ApplyEntityCursor(Entity entity)
    {
        switch (EntityInteraction.ResolveCursor(entity))
        {
            case EntityInteraction.CursorKind.Attack:
                ApplyCursor(AtkCursorId, _hoverCursorAtk);
                break;
            case EntityInteraction.CursorKind.Talk:
                ApplyCursor(TalkCursorId, _hoverCursorTalk);
                break;
            default:
                ApplyCursor(DefaultCursorId, _defaultCursor);
                break;
        }
    }
}
