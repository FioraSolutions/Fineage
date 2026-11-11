using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using VContainer;

public partial class L2LoginUI : L2UI
{
    protected override void Update() {
        base.Update();
    }

    public void OnManualDestroy() {
        
        if (loading)
        {
           //Debug.Log("L2LoginUI Manual : DESTROYYYY");
            _instance = null;
        }
    }

    private void OnDestroy()
    { 
        LoginPacketServices.ClearLoginRuntime();
        if (loading)
        {
            //Debug.Log("L2LoginUI Auto : DESTROYYYY");
            _instance = null;
        }
    }

    protected override void LoadUI() {
        base.LoadUI();

        LoginWin.AddWindow(_rootVisualContainer);
        CharSelectWin.AddWindow(_rootVisualContainer);
        CharSelectWin.HideWindow();
        CharCreateWin.AddWindow(_rootVisualContainer);
        CharCreateWin.HideWindow();
        LicenseWin.AddWindow(_rootVisualContainer);
        LicenseWin.HideWindow();
        ServerWin.AddWindow(_rootVisualContainer);
        ServerWin.HideWindow();
        // Early pass (menu-only). SceneLoader runs a second pass after additive l2_lobby.
        LobbyBrokenMaterialHider.HideBrokenRenderers("login_ui_load");
        InterludeSplashPlayer.Present(RootElement);
        InterludeLoginProof.Arm(RootElement);
        StartCoroutine(ShowLoginWhenReady());
        try
        {
            SkillAnimationDatabase.Initialize();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[L2LoginUI] SkillAnimationDatabase.Initialize failed: " + ex.Message);
        }
    }
}
