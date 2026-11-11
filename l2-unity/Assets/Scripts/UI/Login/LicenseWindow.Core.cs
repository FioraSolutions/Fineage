using System;

public partial class LicenseWindow : L2Window
{
    private void Awake() {
        if (_instance == null) {
            _instance = this;
            WireLoginPacketServices();
        } else {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireLoginPacketServices();
    }

    private void WireLoginPacketServices()
    {
        LoginPacketServices.Bind(login: _login);
    }

    private void OnDestroy() {
        _instance = null;
    }

    public void DevAutoAgreeIfEnabled() {
        if (FineageDev.AutoAdvanceLobby)
            StartCoroutine(AutoAgreeWhenShown());
    }
}
