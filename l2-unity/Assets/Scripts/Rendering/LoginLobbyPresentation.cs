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
        LoginForestPresenter.Present(loginCamera);
    }

    public static void EnterAccountLobby(Camera _)
    {
        LoginForestPresenter.DismissVistaOnly();
        LobbyAccountRenderSettings.Apply();
        LoginForestMusic.Play();
    }

    public static void LeaveLoginFlow()
    {
        LoginForestPresenter.Dismiss();
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