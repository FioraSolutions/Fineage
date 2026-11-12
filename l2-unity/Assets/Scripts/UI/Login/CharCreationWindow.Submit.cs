using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class CharCreationWindow
{
    public void SetlabelError(string error)
    {
        if (labelError != null)
            labelError.text = error ?? "";
    }

    private void CreateButtonPressed()
    {

        // Debug.Log("event create button");

        string class1 = classManipulator.Value;
        string sex = genderManipulator.Value;
        string hairStyle = hairstyleManipulator.Value;
        string hairColor = hairColorManipulator.Value;
        string face = faceManipulator.Value;
        string race = raceManipulator.Value;
        string name = userInputField.value;

        SubmitLobbyCreate(class1, sex);
        //Debug.Log("");
    }

    void SubmitLobbyCreate(string className, string sexName)
    {
        if (raceManipulator == null || raceManipulator.Index < 0
            || classManipulator == null || classManipulator.Index < 0
            || genderManipulator == null || genderManipulator.Index < 0)
        {
            SetlabelError("Choose a race, class, and gender.");
            return;
        }
        if (hairstyleManipulator == null || hairstyleManipulator.Index < 0
            || hairColorManipulator == null || hairColorManipulator.Index < 0
            || faceManipulator == null || faceManipulator.Index < 0)
        {
            SetlabelError("Choose hair and face.");
            return;
        }

        string raceName = raceManipulator != null ? raceManipulator.Value : "";
        int classId = LobbyRaceCatalog.ClassIdFor(raceName, className);
        int raceId = LobbyRaceCatalog.RaceId(raceName);
        int sex = sexName == "Female" ? LobbyCharacterCreate.SexFemale : LobbyCharacterCreate.SexMale;
        int hairStyle = LobbyCharacterCreate.ClampAppearance(
            hairstyleManipulator != null ? hairstyleManipulator.Index : 0,
            LobbyCharacterCreate.MaxHairStyle(sex));
        int hairColor = LobbyCharacterCreate.ClampAppearance(hairColorManipulator != null ? hairColorManipulator.Index : 0, 3);
        int face = LobbyCharacterCreate.ClampAppearance(faceManipulator != null ? faceManipulator.Index : 0, 2);
        string name = userInputField != null && userInputField.value != null ? userInputField.value.Trim() : "";
        string code = LobbyCharacterCreate.Validate(name, classId, raceId, sex, hairStyle, hairColor, face);
        if (code != null)
        {
            SetlabelError(LobbyCharacterText.ErrorMessage(code, null));
            return;
        }

        NetSession session = NetSession.TryGetBound();
        if (session == null)
            return;
        _ = session.CreateCharacter(name, classId, raceId, sex, hairStyle, hairColor, face);
        SetlabelError("");
    }

    public void Clear()
    {
        // Same empty step 0 as window open (#933).
        ApplyLobbyCreationDefaults();
        if (userInputField != null)
            userInputField.value = "";
        SetlabelError("");
        HideRotatePawnWindow();
    }

}
