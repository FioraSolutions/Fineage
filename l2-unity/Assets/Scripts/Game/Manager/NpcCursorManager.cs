using UnityEngine;
using VContainer;

public partial class NpcCursorManager : MonoBehaviour
{
    [Inject] ItemDropPicker _dropPicker;

    private ItemDropPicker DropPicker
    {
        get { return _dropPicker != null ? _dropPicker : ManagerInteractionPacketServices.DropPicker; }
    }

    private Texture2D _defaultCursor;
    private Texture2D _hoverCursorTalk;
    private Texture2D _hoverCursorAtk;
    private Texture2D _hoverCursorPickup;
    private int _currentCursor;
    private ItemEntity _stickyDropItem;
    const int DefaultCursorId = 0;
    const int AtkCursorId = 1;
    const int TalkCursorId = 2;
    const int PickupCursorId = 3;
    const float PickDistance = 1000f;








}
