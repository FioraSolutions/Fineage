using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Login vs account-lobby camera presentation (forest vista vs character select/create).
/// </summary>
public static class LoginLobbyPresentation
{
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
        if (loginCamera != null)
        {
            loginCamera.depthTextureMode |= DepthTextureMode.Depth;
            UniversalAdditionalCameraData extra = loginCamera.GetUniversalAdditionalCameraData();
            extra.requiresDepthOption = CameraOverrideOption.On;
        }
        // ZoneInfo rows on Lobby disagree. The BSP camera-zone table names the
        // one that contains the vista cameras; that DistanceFog is the haze.
        // The same zone's Terrains[] decides whether the heightmap is drawn.
        string zone = L2ZoneRender.AgreedCameraZone(LobbyMapInterludeContract.ZoneCatalogMap);
        L2ZoneRender.SetActiveMap(LobbyMapInterludeContract.ZoneCatalogMap, zone);
        L2ZoneTerrainDraw.Apply(LobbyMapInterludeContract.ZoneCatalogMap);
        L2EnvWorldLut.PushDistanceFog(12f);
        if (loginCamera != null && L2ZoneRender.TryFogClearColor(out Color fog))
        {
            loginCamera.clearFlags = CameraClearFlags.SolidColor;
            loginCamera.backgroundColor = fog;
        }
        L2Ue2MaterialRuntime.Ensure();
        LoginForestMusic.Play();
    }

    public static void EnterAccountLobby(Camera _)
    {
        LobbyAccountRenderSettings.Apply();
        LoginForestMusic.Play();
    }

    public static void LeaveLoginFlow()
    {
        LoginForestMusic.Stop();
        LobbyAccountRenderSettings.Release();
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
