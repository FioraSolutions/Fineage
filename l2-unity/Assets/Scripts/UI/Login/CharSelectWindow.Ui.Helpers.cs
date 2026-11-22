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

        VisualElement detail = GetElementById("CharSelectDetailWindow");
        VisualElement start = GetElementById("StartButtonWindow");
        VisualElement menu = GetElementById("MenuWindow");
        if (detail == null || start == null || menu == null)
            Debug.LogError("[CharSelectWindow] character-select hosts are missing from CharSelectWindow.uxml.");
        else
        {
            InterludeWindowLayout.Apply(detail, "NCLobbyWnd_CharInfo");
            InterludeWindowLayout.Apply(start, "NCLobbyStartWnd");
            InterludeWindowLayout.Apply(menu, "NCLobbyFunctionWnd");
        }

        _startButton = RequireButton("btnStart");
        if (_startButton != null)
        {
            _startButton.AddManipulator(new ButtonClickSoundManipulator(_startButton));
            _startButton.RegisterCallback<ClickEvent>(evt => StartGamePressed());
        }

        _deleteButton = RequireButton("btnDelete");
        if (_deleteButton != null)
        {
            _deleteButton.AddManipulator(new ButtonClickSoundManipulator(_deleteButton));
            _deleteButton.RegisterCallback<ClickEvent>(evt => DeletePressed());
            _deleteButtonLabel = _deleteButton.Q<Label>();
        }

        _createButton = RequireButton("btnCreate");
        if (_createButton != null)
        {
            _createButton.AddManipulator(new ButtonClickSoundManipulator(_createButton));
            _createButton.RegisterCallback<ClickEvent>(evt => CreatePressed());
        }

        Button reloginButton = RequireButton("btnReLogin");
        if (reloginButton != null)
        {
            reloginButton.AddManipulator(new ButtonClickSoundManipulator(reloginButton));
            reloginButton.RegisterCallback<ClickEvent>(evt => ReLoginPressed());
        }

        VisualElement nameField = GetElementById("NameField");
        VisualElement userNameInput = _arrowInputTemplate.Instantiate()[0];
        _charNameManipulator = new ArrowInputManipulator(userNameInput, "Name", new string[] { }, -1, (index, value) => {
            if (_lobbyCharacters != null)
                Characters.SelectLobbyCharacter(index);
            else
                Characters.SelectInterludeCharacter(index);
        });
        userNameInput.AddManipulator(_charNameManipulator);
        if (nameField == null)
        {
            Debug.LogError("[CharSelectWindow] NameField is missing from NCLobbyWnd_CharInfo.");
        }
        else
        {
            if (nameField is Label nameLabel)
                nameLabel.text = "";
            userNameInput.style.position = Position.Absolute;
            userNameInput.style.left = 0f;
            userNameInput.style.top = 0f;
            userNameInput.style.right = 0f;
            userNameInput.style.bottom = 0f;
            nameField.Add(userNameInput);
        }

        _hpLabel = GetElementById("HPBar__text") as Label;
        _mpLabel = GetElementById("MPBar__text") as Label;
        _expLabel = GetElementById("EXPBar__text") as Label;
        _HPBar = GetElementById("HPBar__fill");
        _HPBarBG = GetElementById("HPBar");
        _MPBar = GetElementById("MPBar__fill");
        _MPBarBG = GetElementById("MPBar");
        _ExpBar = GetElementById("EXPBar__fill");
        _ExpBarBG = GetElementById("EXPBar");
        if (_HPBar == null || _MPBar == null || _ExpBar == null)
            Debug.LogError("[CharSelectWindow] NCLobbyWnd_CharInfo is missing HPBar, MPBar, or EXPBar.");

        // Level, class, SP, and karma are not controls on the extracted card.
        Debug.LogError(
            "[CharSelectWindow] NCLobbyWnd_CharInfo has no level, class, SP, or karma controls in the extracted layout.");

        if (detail != null)
        {
            _lobbyStatus = new Label(string.Empty);
            _lobbyStatus.name = "LobbyStatusLabel";
            _lobbyStatus.pickingMode = PickingMode.Ignore;
            _lobbyStatus.style.position = Position.Absolute;
            _lobbyStatus.style.left = 13f;
            _lobbyStatus.style.right = 13f;
            _lobbyStatus.style.top = 128f;
            _lobbyStatus.style.whiteSpace = WhiteSpace.Normal;
            _lobbyStatus.style.fontSize = 12;
            _lobbyStatus.style.color = new Color(0.9f, 0.75f, 0.45f);
            detail.Add(_lobbyStatus);
        }

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
    }

    Button RequireButton(string name)
    {
        Button button = GetElementById(name) as Button;
        if (button == null)
            Debug.LogError("[CharSelectWindow] extracted control '" + name + "' is missing.");
        return button;
    }
}
