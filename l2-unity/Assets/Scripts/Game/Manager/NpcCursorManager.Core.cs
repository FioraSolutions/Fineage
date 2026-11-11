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
        _defaultCursor = IconPacketServices.Icons.LoadCursorByName("Default");
        _hoverCursorAtk = IconPacketServices.Icons.LoadCursorByName("Attack");
        _hoverCursorTalk = IconPacketServices.Icons.LoadCursorByName("Talk");
        _hoverCursorPickup = IconPacketServices.Icons.LoadCursorByName("Pickup");
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

        if (SystemUIPacketServices.Ui != null && SystemUIPacketServices.Ui.MouseOverUI)
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
