using System;

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using VContainer;

public partial class CharSelectWindow : L2Window {
    [Inject] LoginRuntime _login;

    private CharacterSelector Characters => LoginPacketServices.Characters;

    private GameManager Manager => LoginPacketServices.Manager;

    private VisualTreeAsset _arrowInputTemplate;
    private ArrowInputManipulator _charNameManipulator;
    private List<CharSelectionInfoPackage> _characters;
    private List<CharSelectInfoPackage> _charactersInterlude;
    private List<LobbyCharacter> _lobbyCharacters;
    private int _lobbySelectIndex = -1;
    private bool _deleteArmed;
    private int _deleteArmedSlot = -1;
    private Button _startButton;
    private Button _deleteButton;
    private Button _createButton;
    private Label _deleteButtonLabel;
    private Label _lobbyStatus;

    private Label _levelLabel;
    private Label _classLabel;
    private Label _hpLabel;
    private Label _mpLabel;
    private Label _expLabel;
    private Label _spLabel;
    private Label _karmaLabel;

    private VisualElement _HPBar;
    private VisualElement _HPBarBG;
    private VisualElement _MPBar;
    private VisualElement _MPBarBG;
    private VisualElement _ExpBarBG;
    private VisualElement _ExpBar;
    private CharSelectInfoPackage _selectChar;

    private static CharSelectWindow _instance;
    [System.Obsolete("Use [Inject] or LoginPacketServices.CharSelect instead")]
    public static CharSelectWindow Instance { get { return _instance; } }

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

    private void Update() {
        if (!_isWindowHidden) {
            if (PollInput.EscapeDown) {
                AudioPacketServices.Audio.PlayUISound("click_01");
                ReLoginPressed();
            } else if (PollInput.EnterDown) {
                AudioPacketServices.Audio.PlayUISound("click_01");
                StartGamePressed();
            }
        }
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/CharSelectWindow"); 
        _arrowInputTemplate = LoadAsset("Data/UI/_Elements/Template/ArrowInput");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _startButton = (Button) GetElementById("StartButton");
        _startButton.AddManipulator(new ButtonClickSoundManipulator(_startButton));
        _startButton.RegisterCallback<ClickEvent>(evt => StartGamePressed());

        _deleteButton = (Button)GetElementById("DeleteButton");
        _deleteButton.AddManipulator(new ButtonClickSoundManipulator(_deleteButton));
        _deleteButton.RegisterCallback<ClickEvent>(evt => DeletePressed());
        _deleteButtonLabel = _deleteButton.Q<Label>("ButtonLabel");

        _createButton = (Button)GetElementById("CreateButton");
        _createButton.AddManipulator(new ButtonClickSoundManipulator(_createButton));
        _createButton.RegisterCallback<ClickEvent>(evt => CreatePressed());

        Button reloginButton = (Button)GetElementById("ReloginButton");
        reloginButton.AddManipulator(new ButtonClickSoundManipulator(reloginButton));
        reloginButton.RegisterCallback<ClickEvent>(evt => ReLoginPressed());

        VisualElement userNameInputContainer = GetElementById("UserSelectContainer");
        VisualElement userNameInput = _arrowInputTemplate.Instantiate()[0];
        _charNameManipulator = new ArrowInputManipulator(userNameInput, "Name", new string[] { }, -1, (index, value) => {
            if (_lobbyCharacters != null)
                Characters.SelectLobbyCharacter(index);
            else
                Characters.SelectInterludeCharacter(index);
        });

        _levelLabel = (Label)GetElementById("LevelLabel");
        _classLabel = (Label)GetElementById("ClassLabel");
        _hpLabel = (Label)GetElementById("HPText");
        _mpLabel = (Label)GetElementById("MPText");
        _expLabel = (Label)GetElementById("ExpText");
        _spLabel = (Label)GetElementById("SPLabel");
        _karmaLabel = (Label)GetElementById("KarmaLabel");

        _HPBar = GetElementById("HPBar");
        _HPBarBG = GetElementById("HPBarBG");
        _MPBarBG = GetElementById("MPBarBG");
        _MPBar = GetElementById("MPBar");
        _ExpBar = GetElementById("ExpBar");
        _ExpBarBG = GetElementById("ExpBarBG");

        userNameInput.AddManipulator(_charNameManipulator);
        userNameInputContainer.Add(userNameInput);

        VisualElement detail = GetElementById("CharSelectDetailWindow");
        _lobbyStatus = new Label(string.Empty);
        _lobbyStatus.name = "LobbyStatusLabel";
        _lobbyStatus.style.whiteSpace = WhiteSpace.Normal;
        _lobbyStatus.style.marginLeft = 16;
        _lobbyStatus.style.marginRight = 12;
        _lobbyStatus.style.marginTop = 6;
        _lobbyStatus.style.fontSize = 12;
        _lobbyStatus.style.color = new Color(0.9f, 0.75f, 0.45f);
        if (detail != null)
            detail.Add(_lobbyStatus);

        // Three full-height flex columns; only named chrome must pick so world pawn clicks work.
        ConfigureWorldClickThrough();

        SetEmptyData();
        if (_lobbyCharacters != null)
        {
            int selected = _lobbySelectIndex;
            ApplyLobbyNames();
            if (selected >= 0)
                SelectLobbySlot(selected);
        }
    }

    void ConfigureWorldClickThrough()
    {
        if (_windowEle == null)
            return;

        _windowEle.pickingMode = PickingMode.Ignore;
        foreach (VisualElement column in _windowEle.Children())
            column.pickingMode = PickingMode.Ignore;

        string[] chromeIds =
        {
            "CharSelectDetailWindow",
            "StartButtonWindow",
            "MenuWindow",
            "UserSelectContainer",
        };
        for (int i = 0; i < chromeIds.Length; i++)
        {
            VisualElement chrome = GetElementById(chromeIds[i]);
            if (chrome != null)
                chrome.pickingMode = PickingMode.Position;
        }
    }
}
