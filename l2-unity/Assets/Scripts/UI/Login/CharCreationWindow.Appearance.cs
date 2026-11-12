using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class CharCreationWindow
{
    private void RefreshFace(CharacterRaceAnimation raceId , string face)
    {
        byte iFace = (byte)LobbyCharacterCreate.ClampAppearance(faceManipulator != null ? faceManipulator.Index : 0, 2);
        Creator.ReBuildFace(raceId, iFace);
    }

    private void RefreshHair(CharacterRaceAnimation raceId,  string hairColor, string hairStyle)
    {
        int sex = genderManipulator != null && genderManipulator.Index == LobbyCharacterCreate.SexFemale
            ? LobbyCharacterCreate.SexFemale
            : LobbyCharacterCreate.SexMale;
        byte typeHairColor = (byte)LobbyCharacterCreate.ClampAppearance(hairColorManipulator != null ? hairColorManipulator.Index : 0, 3);
        byte typeHairStyle = (byte)LobbyCharacterCreate.ClampAppearance(
            hairstyleManipulator != null ? hairstyleManipulator.Index : 0,
            LobbyCharacterCreate.MaxHairStyle(sex));

        Creator.ReBuildHair(raceId, typeHairColor, typeHairStyle);
    }

    public void Init()
    {
        if (!isInit)
        {
            isInit = true;
            Creator.SpawnAllCharCreatePawns();
        }
    }

    public bool IsOpen
    {
        get { return !_isWindowHidden; }
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        ApplyLobbyCreationDefaults();
    }

    void ApplyLobbyCreationDefaults()
    {
        if (raceManipulator == null)
            return;
        // Interlude step 0: empty stage until race chosen (#933).
        if (Creator != null)
            Creator.ResetPawnSelection();
        raceManipulator.UpdateValues(LobbyRaceCatalog.RaceNames);
        raceManipulator.ResetInput();
        if (classManipulator != null)
            classManipulator.UpdateValues(LobbyRaceCatalog.ClassNames("Human"));
        if (LoginCamera != null)
            LoginCamera.SwitchCamera("Human");
        // Appearance empty until sex chosen (#933).
        if (genderManipulator != null)
            genderManipulator.ClearInput();
        if (hairstyleManipulator != null)
        {
            hairstyleManipulator.UpdateValues(System.Array.Empty<string>());
            hairstyleManipulator.ClearInput();
        }
        if (hairColorManipulator != null)
        {
            hairColorManipulator.UpdateValues(System.Array.Empty<string>());
            hairColorManipulator.ClearInput();
        }
        if (faceManipulator != null)
        {
            faceManipulator.UpdateValues(System.Array.Empty<string>());
            faceManipulator.ClearInput();
        }
    }

}
