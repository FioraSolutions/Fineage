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
