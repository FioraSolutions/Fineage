using System;

public partial class CharCreationWindow : L2Window
{
    public void SetPlayerTemplates(List<PlayerTemplates> playerTemplates)
    {
        if(_playerTemplates != null)
        {
            if (_playerTemplates.Count > 0) _playerTemplates.Clear();
        }
        
        _playerTemplates = playerTemplates;
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        userInputField = (TextField) GetElementById("UserInputField");
        userInputField.AddManipulator(new BlinkingCursorManipulator(userInputField));

        Button createButton = (Button) GetElementById("CreateButton");
        createButton.AddManipulator(new ButtonClickSoundManipulator(createButton));
        createButton.RegisterCallback<ClickEvent>(evt => CreateButtonPressed());

        Button previousButton = (Button)GetElementById("PreviousButton");
        previousButton.AddManipulator(new ButtonClickSoundManipulator(previousButton));
        previousButton.RegisterCallback<ClickEvent>(evt => PreviousButtonPressed());

        VisualElement charDetailWindow = GetElementById("CharCreationDetailWindow");

        pawnRotateWindow = GetElementById("CharacterRotateWindow");
        HideRotatePawnWindow();
        labelError = (Label)GetElementById("LabelError");

        Button pawnRotateLeftButton = pawnRotateWindow.Q<Button>("TurnLeftButton");
        Button pawnRotateRightButton = pawnRotateWindow.Q<Button>("TurnRightButton");
        pawnZoominButton = pawnRotateWindow.Q<Button>("ZoomButton");

        pawnRotateLeftButton.AddManipulator(new ButtonClickSoundManipulator(pawnRotateLeftButton));
        pawnRotateRightButton.AddManipulator(new ButtonClickSoundManipulator(pawnRotateRightButton));
        pawnZoominButton.AddManipulator(new ButtonClickSoundManipulator(pawnZoominButton));

        pawnRotateLeftButton.RegisterCallback<PointerDownEvent>((evt) => {
            Creator.RotatePawn(true);
        }, TrickleDown.TrickleDown);

        pawnRotateRightButton.RegisterCallback<PointerDownEvent>((evt) => {
            Creator.RotatePawn(false);
        }, TrickleDown.TrickleDown);

        pawnRotateLeftButton.RegisterCallback<PointerUpEvent>((evt) => {
            Creator.StopRotatingPawn();
        });

        pawnRotateRightButton.RegisterCallback<PointerUpEvent>((evt) => {
            Creator.StopRotatingPawn();
        });

        pawnZoominButton.RegisterCallback<ClickEvent>((evt) => {
            ToggleZoomin(false);
        });

        labelError.text = string.Empty;
        VisualElement raceInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement classInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement genderInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement hairstyleInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement hairColorInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement faceInput = _arrowInputTemplate.Instantiate()[0];


        hairstyleManipulator = new ArrowInputManipulator(hairstyleInput, "Hairstyle", new string[] { "Type A", "Type B", "Type C", "Type D", "Type E" }, -1, (index, value) => {
            if (Creator.PawnIndex == -1) {
                hairstyleManipulator.ClearInput();
                return;
            }
            CharacterRaceAnimation race = Creator.GetRaceAnimator(Creator.PawnIndex);
            RefreshHair(race, hairColorManipulator.Value, hairstyleManipulator.Value);
            //RefreshFace(CharacterRaceAnimation.FFighter, faceManipulator.Value, hairColorManipulator.Value, hairstyleManipulator.Value);
        });
        hairstyleInput.AddManipulator(hairstyleManipulator);

        hairColorManipulator = new ArrowInputManipulator(hairColorInput, "Hair Color", new string[] { "Type A", "Type B", "Type C", "Type D" }, -1, (index, value) => {
            if (Creator.PawnIndex == -1) {
                hairColorManipulator.ClearInput();
                return;
            }
            CharacterRaceAnimation race = Creator.GetRaceAnimator(Creator.PawnIndex);
            RefreshHair(race, hairColorManipulator.Value, hairstyleManipulator.Value);
        });
        hairColorInput.AddManipulator(hairColorManipulator);

        faceManipulator = new ArrowInputManipulator(faceInput, "Face", new string[] { "Type A", "Type B", "Type C" }, -1, (index, value) => {
            if(Creator.PawnIndex == -1) {
                faceManipulator.ClearInput();
                return;
            }
            CharacterRaceAnimation race = Creator.GetRaceAnimator(Creator.PawnIndex);
            RefreshFace(race, faceManipulator.Value);
            //Debug.Log("Face Change");
        });
        faceInput.AddManipulator(faceManipulator);

        genderManipulator = new ArrowInputManipulator(genderInput, "Gender", new string[] { "Male", "Female" }, -1, (index, value) => {
            if (classManipulator.Value == "") {
                genderManipulator.ClearInput();
                return;
            }

            Camera cam = LoginCamera.SelectGenderCamera(raceManipulator.Value, classManipulator.Value, value);
            if (cam != null) {
                LoginCamera.SwitchCamera(cam);
            }

            hairstyleManipulator.UpdateValues(LobbyCharacterCreate.HairLabels(value == "Female" ? LobbyCharacterCreate.SexFemale : LobbyCharacterCreate.SexMale));
            hairstyleManipulator.ResetInput();
            hairColorManipulator.ResetInput();
            faceManipulator.ResetInput();

            ShowRotatePawnWindow();
            Creator.ResetPawnSelection();
            Creator.SelectPawn(raceManipulator.Value, classManipulator.Value, value);
            
        });
        genderInput.AddManipulator(genderManipulator);

        classManipulator = new ArrowInputManipulator(classInput, "Class", new string[] { "Fighter", "Mystic" }, -1, (index, value) => {
            if(raceManipulator.Value == "Dwarf" && value == "Mystic") {
                classManipulator.ResetInput();
                return;
            }

            if (raceManipulator.Value == "") {
                classManipulator.ClearInput();
                return;
            }

            Camera cam = LoginCamera.SelectClassCamera(raceManipulator.Value, value);
            if (cam != null) {
                LoginCamera.SwitchCamera(cam);
            }

            genderManipulator.ClearInput();
            hairstyleManipulator.ClearInput();
            hairColorManipulator.ClearInput();
            faceManipulator.ClearInput();

            HideRotatePawnWindow();
            Creator.ResetPawnSelection();
            //Creator.SpawnPawnWithId(currentPawnIndex);
        });
        classInput.AddManipulator(classManipulator);

        raceManipulator = new ArrowInputManipulator(raceInput, "Race", LobbyRaceCatalog.RaceNames, -1, (index, value) => {
            LoginCamera.SwitchCamera(value);
            classManipulator.UpdateValues(LobbyRaceCatalog.ClassNames(value));
            genderManipulator.ClearInput();
            hairstyleManipulator.ClearInput();
            hairColorManipulator.ClearInput();
            faceManipulator.ClearInput();

            HideRotatePawnWindow();
            Creator.ResetPawnSelection();
            SetlabelError("");
        });
        raceInput.AddManipulator(raceManipulator);

        raceManipulator.ClearInput();

        charDetailWindow.Add(raceInput);
        charDetailWindow.Add(classInput);
        charDetailWindow.Add(genderInput);
        charDetailWindow.Add(hairstyleInput);
        charDetailWindow.Add(hairColorInput);
        charDetailWindow.Add(faceInput);
    }
}
