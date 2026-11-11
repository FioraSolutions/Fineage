using System;

public partial class ExitWindow : L2PopupWindow
{
    [Inject] L2GameUI _gameUi;
    [Inject] AudioManager _audio;

    private Button _exitButton;
    private Button _restartButton;
    private Label _windowName;
    private Label _expAcquired;
    private Label _adenaAcquired;
    private Label _itemAcquired;

    private static ExitWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static ExitWindow Instance
    {
        get { return _instance; }
    }











}
