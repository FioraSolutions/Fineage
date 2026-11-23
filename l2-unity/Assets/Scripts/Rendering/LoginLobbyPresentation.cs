using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Login vs account-lobby camera presentation (forest vista vs character select/create).
/// </summary>
public static class LoginLobbyPresentation
{
    static readonly List<Renderer> HiddenSheets = new List<Renderer>();

    public static bool IsForestLoginCamera(Camera cam) => cam != null && cam.name == "Login";

    public static bool IsAccountLobbyCamera(Camera cam)
    {
        if (cam == null)
            return false;
        if (cam.name == "CharSelect")
            return true;

        Transform t = cam.transform;
        while (t != null)
        {
            switch (t.name)
            {
                case "CharSelect":
                case "Human":
                case "Elf":
                case "DarkElf":
                case "Dark Elf":
                case "Orc":
                case "Dwarf":
                    return true;
            }
            t = t.parent;
        }

        return false;
    }

    public static void EnterForestLogin(Camera loginCamera)
    {
        LobbyAccountRenderSettings.Release();
        // Logon_Warp's ActionWarp lands on InterpolationPoint5, not the UnrealEd
        // viewport (Camera25, Tag U2Viewport2). UE2 FovAngle=50 is horizontal
        // (PlayerController.uc default); Unity vertical derives from the live aspect.
        L2Ue2Fov.Apply(loginCamera, L2Ue2Fov.OfficialHfovDeg);
        // The heightmap sheet is above this eye (about 135 UU) and one-sided, so the
        // lower frame is the clear color. The grove in front of the point sits below
        // the eye. Hide the sheet's renderer; the collider stays.
        SetHeightmapSheetDrawn(false);
        // ZoneInfo rows on Lobby disagree. The BSP camera-zone table names the
        // one that contains the vista cameras; that DistanceFog is the haze.
        string zone = L2ZoneRender.AgreedCameraZone(LobbyMapInterludeContract.ZoneCatalogMap);
        L2ZoneRender.SetActiveMap(LobbyMapInterludeContract.ZoneCatalogMap, zone);
        L2EnvWorldLut.PushDistanceFog(12f);
        LoginForestMusic.Play();
    }

    public static void EnterAccountLobby(Camera _)
    {
        SetHeightmapSheetDrawn(true);
        LobbyAccountRenderSettings.Apply();
        LoginForestMusic.Play();
    }

    public static void LeaveLoginFlow()
    {
        SetHeightmapSheetDrawn(true);
        LoginForestMusic.Stop();
        LobbyAccountRenderSettings.Release();
    }

    /// <summary>
    /// The converted heightmap is the object named <c>l2_lobby</c>. World tiles use
    /// the same mesh name as their tile id, so this only matches the lobby sheet.
    /// </summary>
    static void SetHeightmapSheetDrawn(bool drawn)
    {
        if (drawn)
        {
            for (int i = 0; i < HiddenSheets.Count; i++)
            {
                if (HiddenSheets[i] != null)
                    HiddenSheets[i].enabled = true;
            }
            HiddenSheets.Clear();
            return;
        }

        HiddenSheets.Clear();
        Renderer[] renderers = UnityEngine.Object.FindObjectsByType<Renderer>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            if (r == null || !string.Equals(r.gameObject.name, "l2_lobby", StringComparison.Ordinal))
                continue;
            if (!r.enabled)
                continue;
            r.enabled = false;
            HiddenSheets.Add(r);
        }
    }

    public static void OnSwitchCamera(Camera cam)
    {
        if (IsForestLoginCamera(cam))
            EnterForestLogin(cam);
        else if (IsAccountLobbyCamera(cam))
            EnterAccountLobby(cam);
        else
            LeaveLoginFlow();
    }
}