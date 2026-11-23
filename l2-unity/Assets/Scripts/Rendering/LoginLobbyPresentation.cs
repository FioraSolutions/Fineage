using UnityEngine;

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
        // Official vista: the baked lobby scene through the Cameras.prefab "Login"
        // camera (transform from Lobby.unr Camera25). UE2 FovAngle=50 is horizontal
        // (PlayerController.uc default); Unity vertical derives from the live aspect.
        L2Ue2Fov.Apply(loginCamera, L2Ue2Fov.OfficialHfovDeg);
        // ZoneInfo rows on Lobby disagree. The BSP camera-zone table names the
        // one that contains the vista cameras; that DistanceFog is the haze.
        string zone = L2ZoneRender.AgreedCameraZone(LobbyMapInterludeContract.ZoneCatalogMap);
        L2ZoneRender.SetActiveMap(LobbyMapInterludeContract.ZoneCatalogMap, zone);
        L2EnvWorldLut.PushDistanceFog(12f);
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