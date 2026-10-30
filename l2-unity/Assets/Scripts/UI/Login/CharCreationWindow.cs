using System.Collections;
using System.Collections.Generic;


using UnityEngine;

using UnityEngine.UIElements;
using VContainer;

public class CharCreationWindow : L2Window {
    [Inject] LoginRuntime _login;

    private CharacterCreator Creator => LoginPacketServices.Creator;

    private LoginCameraManager LoginCamera => LoginPacketServices.LoginCamera;

    private GameClient Game => LoginPacketServices.Game;

    private GameManager Manager => LoginPacketServices.Manager;

    private VisualTreeAsset _arrowInputTemplate;
    private ArrowInputManipulator hairstyleManipulator;
    private ArrowInputManipulator hairColorManipulator;
    private ArrowInputManipulator faceManipulator;
    private ArrowInputManipulator genderManipulator;
    private ArrowInputManipulator classManipulator;
    private ArrowInputManipulator raceManipulator;
    private TextField userInputField;
    private VisualElement pawnRotateWindow;
    private Label labelError;
    private Button pawnZoominButton;
    private List<PlayerTemplates> _playerTemplates;
    private bool isInit = false;
    private static CharCreationWindow _instance;
    public static CharCreationWindow Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            WireLoginPacketServices();
        } else {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireLoginPacketServices();
    }

    private void WireLoginPacketServices()
    {
        LoginPacketServices.Bind(login: _login);
    }

    private void OnDestroy() {
        _instance = null;
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/CharCreationWindow");
        _arrowInputTemplate = LoadAsset("Data/UI/_Elements/Template/ArrowInput");
    }

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

            if (FineageWireConfig.UseLoginHandoff)
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

    private void RefreshFace(CharacterRaceAnimation raceId , string face)
    {
        byte iFace = FineageWireConfig.UseLoginHandoff
            ? (byte)LobbyCharacterCreate.ClampAppearance(faceManipulator != null ? faceManipulator.Index : 0, 2)
            : ConvertType.ConvertTypeToByte(face);
        Creator.ReBuildFace(raceId, iFace);
    }

    private void RefreshHair(CharacterRaceAnimation raceId,  string hairColor, string hairStyle)
    {
        byte typeHairColor;
        byte typeHairStyle;
        if (FineageWireConfig.UseLoginHandoff)
        {
            int sex = genderManipulator != null && genderManipulator.Index == LobbyCharacterCreate.SexFemale
                ? LobbyCharacterCreate.SexFemale
                : LobbyCharacterCreate.SexMale;
            typeHairColor = (byte)LobbyCharacterCreate.ClampAppearance(hairColorManipulator != null ? hairColorManipulator.Index : 0, 3);
            typeHairStyle = (byte)LobbyCharacterCreate.ClampAppearance(
                hairstyleManipulator != null ? hairstyleManipulator.Index : 0,
                LobbyCharacterCreate.MaxHairStyle(sex));
        }
        else
        {
            typeHairColor = ConvertType.ConvertTypeToByte(hairColor);
            typeHairStyle = ConvertType.ConvertTypeToByte(hairStyle);
        }

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
        if (FineageWireConfig.UseLoginHandoff)
            ApplyLobbyCreationDefaults();
    }

    void ApplyLobbyCreationDefaults()
    {
        if (raceManipulator == null)
            return;
        raceManipulator.UpdateValues(LobbyRaceCatalog.RaceNames);
        raceManipulator.ResetInput();
        if (classManipulator != null)
            classManipulator.UpdateValues(LobbyRaceCatalog.ClassNames("Human"));
        if (LoginCamera != null)
            LoginCamera.SwitchCamera("Human");
        if (hairstyleManipulator != null)
        {
            int sex = genderManipulator != null && genderManipulator.Value == "Female"
                ? LobbyCharacterCreate.SexFemale
                : LobbyCharacterCreate.SexMale;
            hairstyleManipulator.UpdateValues(LobbyCharacterCreate.HairLabels(sex));
            hairstyleManipulator.ResetInput();
        }
    }

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

        if (FineageWireConfig.UseLoginHandoff)
        {
            SubmitLobbyCreate(class1, sex);
            return;
        }

        var sendPaket = new CharacterCreateCommand(_playerTemplates , class1, sex , hairColor , hairStyle , face , race , name);
        Game.Send(sendPaket);
        //Debug.Log("Button click ButtonPressed");
        //Debug.Log("");
    }

    void SubmitLobbyCreate(string className, string sexName)
    {
        if (classManipulator == null || classManipulator.Index < 0 || genderManipulator == null || genderManipulator.Index < 0)
        {
            SetlabelError("Choose a class and gender.");
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

        ZoneV2Client zone = ZoneV2Client.Instance;
        if (zone == null || !zone.CreateLobbyCharacter(name, classId, raceId, sex, hairStyle, hairColor, face))
        {
            SetlabelError(LobbyCharacterText.ErrorMessage("auth_required", null));
            return;
        }

        SetlabelError("");
    }

    public void Clear()
    {
        classManipulator.ClearInput();
        genderManipulator.ClearInput();
        hairstyleManipulator.ClearInput();
        hairColorManipulator.ClearInput();
        faceManipulator.ClearInput();
        raceManipulator.ClearInput();
        userInputField.value = "";
        SetlabelError("");
    }

    private void PreviousButtonPressed() {
        if (FineageWireConfig.UseLoginHandoff)
        {
            ZoneV2Client zone = ZoneV2Client.Instance;
            if (zone != null)
                zone.CancelLobbyCreate();
        }

        classManipulator.ClearInput();
        genderManipulator.ClearInput();
        hairstyleManipulator.ClearInput();
        hairColorManipulator.ClearInput();
        faceManipulator.ClearInput();
        raceManipulator.ClearInput();
        userInputField.value = "";
        SetlabelError("");
        ReleaseCreatePawns();
        Manager.OnAuthAllowed();
    }

    void ReleaseCreatePawns()
    {
        if (Creator != null)
            Creator.DestroyAllPawns();
        isInit = false;
    }

    private void ShowRotatePawnWindow() {
        pawnRotateWindow.style.display = DisplayStyle.Flex;

        if (pawnZoominButton.ClassListContains("toggle")) {
            pawnZoominButton.RemoveFromClassList("toggle");
        }
    }

    private void HideRotatePawnWindow() {
        pawnRotateWindow.style.display = DisplayStyle.None;
    }

    private void ToggleZoomin(bool removeOnly) {
        if(pawnZoominButton.ClassListContains("toggle")) {
            LoginCamera.ZoomOut();

            pawnZoominButton.RemoveFromClassList("toggle");
        } else {

            LoginCamera.ZoomIn();

            pawnZoominButton.AddToClassList("toggle");
        }
    }
}