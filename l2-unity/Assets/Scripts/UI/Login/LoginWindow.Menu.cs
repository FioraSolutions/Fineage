using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class LoginWindow
{
    const string NewAccountUrlKey = "fineage.login.newAccountUrl";
    const string LostAccountUrlKey = "fineage.login.lostAccountUrl";
    const string NewAccountUrlEnv = "FINEAGE_NEW_ACCOUNT_URL";
    const string LostAccountUrlEnv = "FINEAGE_LOST_ACCOUNT_URL";

    OptionsPanel _options;

    void WireLoginMenu(VisualElement root)
    {
        WireMenuButton(root, "NewAccountButton", OnNewAccountPressed);
        WireMenuButton(root, "LostAccountButton", OnLostAccountPressed);
        WireMenuButton(root, "OptionsButton", OnOptionsPressed);
        WireMenuButton(root, "ProductionTeamButton", OnProductionTeamPressed);
        WireMenuButton(root, "ReplayButton", OnReplayPressed);
    }

    static void WireMenuButton(VisualElement root, string name, Action handler)
    {
        Button button = root.Q<Button>(name);
        if (button == null)
            return;

        button.AddManipulator(new ButtonClickSoundManipulator(button));
        button.RegisterCallback<ClickEvent>(_ => handler());
    }

    void OnNewAccountPressed()
    {
        if (!TryOpenConfiguredUrl(NewAccountUrlKey, NewAccountUrlEnv))
            ShowErrorText("New Account URL is not configured.");
    }

    void OnLostAccountPressed()
    {
        if (!TryOpenConfiguredUrl(LostAccountUrlKey, LostAccountUrlEnv))
            ShowErrorText("Lost Account URL is not configured.");
    }

    void OnOptionsPressed()
    {
        VisualElement host = _root != null && _root.panel != null
            ? _root.panel.visualTree
            : _root;
        if (host == null)
            return;

        if (_options != null && _options.parent != null)
        {
            _options.RemoveFromHierarchy();
            _options = null;
            return;
        }

        _options = new OptionsPanel();
        host.Add(_options);
    }

    void OnProductionTeamPressed()
    {
        ShowErrorText("Production Team credits are not ported yet.");
    }

    void OnReplayPressed()
    {
        ShowErrorText("Replay is not ported yet.");
    }

    static bool TryOpenConfiguredUrl(string prefsKey, string envKey)
    {
        string url = PlayerPrefs.GetString(prefsKey, "");
        if (string.IsNullOrWhiteSpace(url))
            url = Environment.GetEnvironmentVariable(envKey) ?? "";
        if (string.IsNullOrWhiteSpace(url))
            return false;

        Application.OpenURL(url.Trim());
        return true;
    }
}
