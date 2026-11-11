using System;

public partial class CharCreationWindow : L2Window
{
    private void OnDestroy() {
        _instance = null;
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/CharCreationWindow");
        _arrowInputTemplate = LoadAsset("Data/UI/_Elements/Template/ArrowInput");
    }
}
