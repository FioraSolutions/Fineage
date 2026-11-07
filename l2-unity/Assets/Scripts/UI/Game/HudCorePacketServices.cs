/// <summary>
/// Core HUD window access with [Inject] wiring and live fallback (#142).
/// Covers Interlude character/action/target/menu/map chrome plus slot/tooltip hosts.
/// </summary>
public static class HudCorePacketServices
{
    private static CharacterInfoWindow _characterInfo;
    private static ActionWindow _action;
    private static TargetWindow _target;
    private static MenuWindow _menu;
    private static MapWindow _map;
    private static L2SlotManager _slots;
    private static L2ToolTip _toolTip;

    public static void Bind(
        CharacterInfoWindow characterInfo = null,
        ActionWindow action = null,
        TargetWindow target = null,
        MenuWindow menu = null,
        MapWindow map = null,
        L2SlotManager slots = null,
        L2ToolTip toolTip = null)
    {
        if (characterInfo != null)
            _characterInfo = characterInfo;
        if (action != null)
            _action = action;
        if (target != null)
            _target = target;
        if (menu != null)
            _menu = menu;
        if (map != null)
            _map = map;
        if (slots != null)
            _slots = slots;
        if (toolTip != null)
            _toolTip = toolTip;
    }

    public static CharacterInfoWindow CharacterInfo =>
        _characterInfo != null ? _characterInfo : CharacterInfoWindow.Instance;

    public static ActionWindow Action =>
        _action != null ? _action : ActionWindow.Instance;

    public static TargetWindow Target =>
        _target != null ? _target : TargetWindow.Instance;

    public static MenuWindow Menu =>
        _menu != null ? _menu : MenuWindow.Instance;

    public static MapWindow Map =>
        _map != null ? _map : MapWindow.Instance;

    public static L2SlotManager Slots =>
        _slots != null ? _slots : L2SlotManager.Instance;

    public static L2ToolTip ToolTip =>
        _toolTip != null ? _toolTip : L2ToolTip.Instance;
}
