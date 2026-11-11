using System;

public partial class SystemMenuWindow : L2PopupWindow
{
    [Inject] L2GameUI _gameUi;
    [Inject] AudioManager _audio;

    private float _windowHeight = 0f;
    private static SystemMenuWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SystemMenuWindow Instance
    {
        get { return _instance; }
    }








    private OptionsPanel _options;






}
