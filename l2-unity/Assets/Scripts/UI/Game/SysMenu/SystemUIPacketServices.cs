/// <summary>
/// System menu UI access with [Inject] wiring and singleton fallback (#142 sub-slice 14).
/// </summary>
public static class SystemUIPacketServices
{
    private static L2GameUI _ui;
    private static AudioManager _audio;

    public static void Bind(L2GameUI ui = null, AudioManager audio = null)
    {
        if (ui != null)
            _ui = ui;
        if (audio != null)
            _audio = audio;
    }

    public static L2GameUI Ui =>
        _ui != null ? _ui : L2GameUI.Instance;

    public static AudioManager Audio =>
        _audio != null ? _audio : AudioManager.Instance;
}
