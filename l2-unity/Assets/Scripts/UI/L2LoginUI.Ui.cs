using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2LoginUI : L2UI
{
    System.Collections.IEnumerator ShowLoginWhenReady()
    {
        // AddWindow builds via coroutine — wait until LoginWindow visual exists.
        for (int i = 0; i < 120; i++)
        {
            var login = LoginWin;
            if (login != null && login.gameObject != null)
            {
                login.ShowWindow();
                yield break;
            }
            yield return null;
        }
    }

    public void ShowServerSelectWindow() {
        LoginWin.HideWindow();
        LicenseWin.HideWindow();
        ServerWin.ShowWindow();
    }

    public void ShowLicenseWindow() {
        LoginWin.HideWindow();
        LicenseWin.ShowWindow();
        ServerWin.HideWindow();
        LicenseWin.DevAutoAgreeIfEnabled();
    }

    public void ShowCharSelectWindow() {
        LoginWin.ShowErrorText("");
        LoginWin.HideWindow();
        CharCreateWin.HideWindow();
        CharCreateWin.ReleaseCreatePawns();
        CharSelectWin.ShowWindow();
        ServerWin.HideWindow();
    }

    public void ShowLoginWindow() {
        CharSelectWin.HideWindow();
        LoginWin.ShowWindow();
        CharCreateWin.HideWindow();
        CharSelectWin.HideWindow();
        LicenseWin.HideWindow();
        ServerWin.HideWindow();
    }

    public void ShowCharCreationWindow() {
        CharSelectWin.HideWindow();
        CharCreateWin.Init();
        CharCreateWin.ShowWindow();

    }

    public void SetCharTemplations(List<PlayerTemplates> playerTemplates)
    {
        CharCreateWin.Clear();
        CharCreateWin.SetPlayerTemplates(playerTemplates);
    }

    public void ShowCharCreationError(string text)
    {
        CharCreateWin.SetlabelError(text);
    }
}
