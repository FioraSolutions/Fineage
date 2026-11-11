using System;

public partial class StatusWindow
{
    private void Awake() {
        if (_instance == null) {
            _instance = this;
            WireGameWindowPacketServices();
        } else {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireGameWindowPacketServices();
    }

    private void WireGameWindowPacketServices()
    {
        GameWindowPacketServices.Bind(status: this);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/StatusWindow");
    }
}
