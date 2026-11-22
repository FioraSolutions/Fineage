using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;
using VContainer;

public partial class CharSelectWindow : L2Window
{
    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _startButton = (Button) GetElementById("btnStart");
        _startButton.AddManipulator(new ButtonClickSoundManipulator(_startButton));
        _startButton.RegisterCallback<ClickEvent>(evt => StartGamePressed());

        _deleteButton = (Button)GetElementById("btnDelete");
        _deleteButton.AddManipulator(new ButtonClickSoundManipulator(_deleteButton));
        _deleteButton.RegisterCallback<ClickEvent>(evt => DeletePressed());
        _deleteButtonLabel = _deleteButton.Q<Label>("ButtonLabel");

        _createButton = (Button)GetElementById("btnCreate");
        _createButton.AddManipulator(new ButtonClickSoundManipulator(_createButton));
        _createButton.RegisterCallback<ClickEvent>(evt => CreatePressed());

        Button reloginButton = (Button)GetElementById("btnReLogin");
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

        _HPBar = GetElementById("LobbyHPBar");
        _HPBarBG = GetElementById("HPBarBG");
        _MPBarBG = GetElementById("MPBarBG");
        _MPBar = GetElementById("LobbyMPBar");
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

        ApplyLobbyScreenAnchors();

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

    void ApplyLobbyScreenAnchors()
    {
        ApplyPlacedWindow("CharSelectDetailWindow", "NCLobbyWnd_CharInfo", true);
        ApplyPlacedWindow("StartButtonWindow", "NCLobbyStartWnd", false);
        ApplyPlacedWindow("MenuWindow", "NCLobbyFunctionWnd", false);
    }

    void ApplyPlacedWindow(string elementName, string windowName, bool keepExistingChildren)
    {
        VisualElement host = GetElementById(elementName);
        if (host == null)
            return;

        List<string> present = new List<string>();
        host.Query<VisualElement>().ForEach(element =>
        {
            if (!string.IsNullOrEmpty(element.name))
                present.Add(element.name);
        });
        InterludeWindowLayout.Apply(host, windowName);
        if (!keepExistingChildren)
            return;

        List<VisualElement> created = new List<VisualElement>();
        host.Query<VisualElement>().ForEach(element =>
        {
            if (element.parent != host || string.IsNullOrEmpty(element.name))
                return;
            if (!present.Contains(element.name))
                created.Add(element);
        });
        for (int i = 0; i < created.Count; i++)
            created[i].RemoveFromHierarchy();
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
