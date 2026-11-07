/// <summary>
/// System menu / exit UI access with [Inject] wiring and singleton fallback (#142 sub-slice 14).
/// </summary>
public static class SystemUIPacketServices
{
    private static L2GameUI _ui;
    private static AudioManager _audio;
    private static SystemMenuWindow _menu;
    private static ExitWindow _exit;

    public static void Bind(
        L2GameUI ui = null,
        AudioManager audio = null,
        SystemMenuWindow menu = null,
        ExitWindow exit = null)
    {
        if (ui != null)
            _ui = ui;
        if (audio != null)
            _audio = audio;
        if (menu != null)
            _menu = menu;
        if (exit != null)
            _exit = exit;
    }

    public static L2GameUI Ui =>
        _ui != null ? _ui : L2GameUI.Instance;

    public static AudioManager Audio =>
        _audio != null ? _audio : AudioManager.Instance;

    public static SystemMenuWindow Menu =>
        _menu != null ? _menu : SystemMenuWindow.Instance;

    public static ExitWindow Exit =>
        _exit != null ? _exit : ExitWindow.Instance;
}
