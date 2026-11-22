using UnityEngine;

/// <summary>
/// One Interlude char-create presentation pipeline: selection stage drives gallery visibility and cameras.
/// </summary>
public static class LobbyCharCreatePresentation
{
    public static void Apply(
        LobbyCharCreateStageCatalog.Stage stage,
        CharacterCreator creator,
        LoginCameraManager loginCamera,
        string race,
        string pawnClass,
        string gender)
    {
        if (creator == null)
            return;

        switch (stage)
        {
            case LobbyCharCreateStageCatalog.Stage.None:
                creator.ResetPawnSelection();
                if (loginCamera != null)
                    loginCamera.SwitchCamera("Login");
                break;

            case LobbyCharCreateStageCatalog.Stage.Race:
                if (string.IsNullOrEmpty(race))
                    goto case LobbyCharCreateStageCatalog.Stage.None;
                if (loginCamera != null)
                    loginCamera.SwitchCamera(race);
                creator.ResetPawnSelection();
                creator.ShowRaceGallery(race);
                break;

            case LobbyCharCreateStageCatalog.Stage.Class:
                if (string.IsNullOrEmpty(race) || string.IsNullOrEmpty(pawnClass))
                    goto case LobbyCharCreateStageCatalog.Stage.None;
                if (loginCamera != null)
                {
                    Camera cam = loginCamera.SelectClassCamera(race, pawnClass);
                    if (cam != null)
                        loginCamera.SwitchCamera(cam);
                }
                creator.ResetPawnSelection();
                creator.ShowClassGallery(race, pawnClass);
                break;

            case LobbyCharCreateStageCatalog.Stage.Sex:
                if (string.IsNullOrEmpty(race) || string.IsNullOrEmpty(pawnClass) || string.IsNullOrEmpty(gender))
                    goto case LobbyCharCreateStageCatalog.Stage.None;
                if (loginCamera != null)
                {
                    Camera cam = loginCamera.SelectGenderCamera(race, pawnClass, gender);
                    if (cam != null)
                        loginCamera.SwitchCamera(cam);
                }
                creator.SelectPawn(race, pawnClass, gender);
                break;
        }
    }
}