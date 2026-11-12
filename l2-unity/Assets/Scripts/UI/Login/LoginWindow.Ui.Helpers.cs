using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class LoginWindow
{
    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/LoginWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _labelError = (Label)GetElementById("LabelError");
        _loginButton = (Button) GetElementById("LoginButton");
        _loginButton.AddManipulator(new ButtonClickSoundManipulator(_loginButton));
        _loginButton.RegisterCallback<ClickEvent>(evt => LoginButtonPressed());
        SetLoginBusy(false);

        Button exitButton = (Button)GetElementById("ExitButton");
        exitButton.AddManipulator(new ButtonClickSoundManipulator(exitButton));
        exitButton.RegisterCallback<ClickEvent>(evt => ExitButtonPressed());

        _userInput = (TextField) GetElementById("UserInputField");
        _userInput.RegisterCallback<FocusEvent>((evt) => OnInputFocus(evt, _userInput));
        _userInput.RegisterCallback<BlurEvent>((evt) => OnInputBlur(evt, _userInput));
        _userInput.maxLength = 16;
        _userInput.RegisterValueChangedCallback(OnLoginInputChanged);

        _userInput.AddManipulator(new BlinkingCursorManipulator(_userInput));
        _userInput.RegisterCallback<KeyDownEvent>((evt) => OnKeyPressed(evt, _userInput));

        _passwordInput = (TextField)GetElementById("PasswordInputField");
        _passwordInput.RegisterCallback<FocusEvent>((evt) => OnInputFocus(evt, _passwordInput));
        _passwordInput.RegisterCallback<BlurEvent>((evt) => OnInputBlur(evt, _passwordInput));
        _passwordInput.maxLength = 16;

        _passwordInput.AddManipulator(new BlinkingCursorManipulator(_passwordInput));
        _passwordInput.RegisterCallback<KeyDownEvent>((evt) => OnKeyPressed(evt, _passwordInput));


        _logo = root.Q<VisualElement>("L2Logo");

        _rememberToggle = root.Q<Toggle>("RememberToggle");
        bool remember = PlayerPrefs.GetInt(RememberKey, 0) == 1;
        if (_rememberToggle != null)
        {
            _rememberToggle.SetValueWithoutNotify(remember);
            _rememberToggle.RegisterValueChangedCallback(evt =>
            {
                if (!evt.newValue)
                    StoreRemembered(false, null, null);
            });
        }
        if (remember)
        {
            _userInput.SetValueWithoutNotify(PlayerPrefs.GetString(RememberAccountKey, ""));
            _passwordInput.SetValueWithoutNotify(PlayerPrefs.GetString(RememberPasswordKey, ""));
        }

        WireLoginMenu(root);

        _userInput.Focus();
    }

    private void OnKeyPressed(KeyDownEvent evt, TextField input) {
        KeyCode keyCode = evt.keyCode;
        if (keyCode == KeyCode.Tab) {
            if (input == _passwordInput) {
                _userInput.Focus();
            } else {
                _passwordInput.Focus();
            }

            evt.PreventDefault();

        } else if (keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter) {
            AudioPacketServices.Audio.PlayUISound("click_01");
            LoginButtonPressed();
            evt.PreventDefault();
        }
    }

    private void OnLoginInputChanged(ChangeEvent<string> evt) {
        string username = evt.newValue;

        // Regex pattern: ^[a-zA-Z0-9]{1,16}$
        // This allows only alphanumeric characters, 1-16 characters long
        Regex regex = new Regex(@"^[a-zA-Z0-9]{0,16}$");

        if (!regex.IsMatch(username)) {
            // If the input doesn't match the regex, revert to the previous value
            _userInput.SetValueWithoutNotify(evt.previousValue);
        }
    }

    private void OnInputFocus(FocusEvent evt, VisualElement input) {
        if (!input.ClassListContains("highlighted")) {
            input.AddToClassList("highlighted");
        }
    }
}
