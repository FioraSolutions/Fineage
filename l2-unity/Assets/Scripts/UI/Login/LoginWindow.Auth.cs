using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class LoginWindow
{
    private void OnInputBlur(BlurEvent evt, VisualElement input) {
        if (input.ClassListContains("highlighted")) {
            input.RemoveFromClassList("highlighted");
        }
    }

    private void LoginButtonPressed() {
        if (_loginBusy)
            return;

        LoginClient login = Login;
        string account = _userInput.value.ToLower().Trim();
        string password = _passwordInput.value.Trim();

        if (string.IsNullOrEmpty(account) && login != null)
            account = (login.Account ?? string.Empty).ToLower().Trim();
        if (string.IsNullOrEmpty(password) && login != null)
            password = (login.Password ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            return;

        StoreRemembered(_rememberToggle != null && _rememberToggle.value, account, password);

        SetLoginBusy(true);

        IZoneTransport transport = _zoneTransport ?? (App.HasContainer ? App.Resolve<IZoneTransport>() : null);

        ShowStatusText("Connecting to " + FineageWireConfig.LoginHost + ":" + FineageWireConfig.LoginPort);

        try
        {
            if (transport != null)
                transport.BeginSession(account, password);
            else
                _ = NetSession.Ensure().BeginLobby(account, password, registerIfMissing: true);
        }
        catch (Exception ex)
        {
            Debug.LogError("[Login] BeginSession failed: " + ex.Message);
            ShowErrorText(ex.Message);
        }
    }

    void SetLoginBusy(bool busy)
    {
        _loginBusy = busy;
        if (_loginButton != null)
            _loginButton.SetEnabled(!busy);
    }

    private void ExitButtonPressed()
    {
        // Same path as System Menu → Exit (Editor stops Play; player builds quit).
        GameManager manager = LoginPacketServices.Manager ?? GamePacketServices.Manager;
        if (manager != null)
        {
            manager.QuitGame();
            return;
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowErrorText(string text)
    {
        ShowStatusText(text);
        // Failures / clear — allow another attempt. In-flight status uses ShowStatusText only.
        SetLoginBusy(false);
    }

    public void ShowStatusText(string text)
    {
        if (_labelError != null)
            _labelError.text = text ?? string.Empty;
    }

    public override void HideWindow() {
        base.HideWindow();
        if (_logo != null)
            _logo.style.display = DisplayStyle.None;
    }

    public override void ShowWindow() {
        base.ShowWindow();
        if (_logo != null)
            _logo.style.display = DisplayStyle.Flex;
        SetLoginBusy(false);
        _userInput?.Focus();
    }

    static void StoreRemembered(bool remember, string account, string password)
    {
        if (remember)
        {
            PlayerPrefs.SetInt(RememberKey, 1);
            PlayerPrefs.SetString(RememberAccountKey, account ?? "");
            PlayerPrefs.SetString(RememberPasswordKey, password ?? "");
        }
        else
        {
            PlayerPrefs.DeleteKey(RememberKey);
            PlayerPrefs.DeleteKey(RememberAccountKey);
            PlayerPrefs.DeleteKey(RememberPasswordKey);
        }
        PlayerPrefs.Save();
    }
}
