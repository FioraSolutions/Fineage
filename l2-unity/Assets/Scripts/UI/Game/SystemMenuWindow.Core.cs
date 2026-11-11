using System;

public partial class SystemMenuWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireSystemUIPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireSystemUIPacketServices();
    }

    private void WireSystemUIPacketServices()
    {
        SystemUIPacketServices.Bind(ui: _gameUi, audio: _audio, menu: this);
    }

    private void Update()
    {
        if (_isWindowHidden)
        {
            return;
        }

        if (PollInput.LeftMouseDown)
        {
            if (!MouseOverThisWindow() && !HudCorePacketServices.Menu.MouseOverThisWindow())
            {
                HideWindow();
            }
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SystemMenuWindow");
    }
}
