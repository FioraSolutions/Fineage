using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class LoginWindow
{
    public void ShowErrorText(string text)
    {
        ShowStatusText(text);
        // Failures / clear — allow another attempt. In-flight status uses ShowStatusText only.
        SetLoginBusy(false);
    }

    public void ShowStatusText(string text)
    {
        if (_labelError == null)
            return;
        _labelError.text = text ?? string.Empty;
        UpdateStatusLayout();
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

    void StoreRemembered(bool remember, string account, string password)
    {
        ILoginSecretStore store = LoginSecretStore.Create();
        if (!remember)
        {
            store.Clear();
            DeleteLegacyRememberPrefs();
            return;
        }

        if (!store.Save(account ?? "", password ?? ""))
        {
            Debug.LogError(
                "[Login] Remember me did not save the account and password. " +
                "They were not written to PlayerPrefs or a file.");
            if (_rememberToggle != null)
                _rememberToggle.SetValueWithoutNotify(false);
            return;
        }

        DeleteLegacyRememberPrefs();
    }

    void LoadRemembered()
    {
        ILoginSecretStore store = LoginSecretStore.Create();
        string account;
        string password;
        bool haveVault = store.TryLoad(out account, out password);
        if (!haveVault && HasLegacyRememberPrefs())
        {
            string legacyAccount = PlayerPrefs.GetString(RememberAccountKey, "");
            string legacyPassword = PlayerPrefs.GetString(RememberPasswordKey, "");
            if ((!string.IsNullOrEmpty(legacyAccount) || !string.IsNullOrEmpty(legacyPassword))
                && store.Save(legacyAccount, legacyPassword)
                && store.TryLoad(out account, out password))
            {
                haveVault = true;
            }
            else if (!string.IsNullOrEmpty(legacyAccount) || !string.IsNullOrEmpty(legacyPassword))
            {
                Debug.LogError(
                    "[Login] A remembered password was stored in PlayerPrefs. " +
                    "It was removed and could not be moved into OS secure storage.");
            }
        }

        DeleteLegacyRememberPrefs();
        if (_rememberToggle != null)
            _rememberToggle.SetValueWithoutNotify(haveVault);
        if (!haveVault || _userInput == null || _passwordInput == null)
            return;
        _userInput.SetValueWithoutNotify(account ?? "");
        _passwordInput.SetValueWithoutNotify(password ?? "");
    }

    bool HasLegacyRememberPrefs()
    {
        return PlayerPrefs.HasKey(RememberKey)
            || PlayerPrefs.HasKey(RememberAccountKey)
            || PlayerPrefs.HasKey(RememberPasswordKey);
    }

    static void DeleteLegacyRememberPrefs()
    {
        bool any = PlayerPrefs.HasKey(RememberKey)
            || PlayerPrefs.HasKey(RememberAccountKey)
            || PlayerPrefs.HasKey(RememberPasswordKey);
        if (!any)
            return;
        PlayerPrefs.DeleteKey(RememberKey);
        PlayerPrefs.DeleteKey(RememberAccountKey);
        PlayerPrefs.DeleteKey(RememberPasswordKey);
        PlayerPrefs.Save();
    }
}
