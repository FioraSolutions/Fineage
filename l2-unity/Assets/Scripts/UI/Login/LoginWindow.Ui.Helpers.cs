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
        if (_loginButton != null)
        {
            _loginButton.AddManipulator(new ButtonClickSoundManipulator(_loginButton));
            _loginButton.RegisterCallback<ClickEvent>(evt => LoginButtonPressed());
        }
        SetLoginBusy(false);

        Button exitButton = (Button)GetElementById("ExitButton");
        if (exitButton != null)
        {
            exitButton.AddManipulator(new ButtonClickSoundManipulator(exitButton));
            exitButton.RegisterCallback<ClickEvent>(evt => ExitButtonPressed());
        }

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

        // One Fineage mark — extra L2Logo nodes (re-entrant AddWindow / old Lineage2_Login) ghost the II.
        var logos = root.Query<VisualElement>("L2Logo").ToList();
        _logo = null;
        for (int i = 0; i < logos.Count; i++)
        {
            if (logos[i] == null)
                continue;
            if (_logo == null)
            {
                _logo = logos[i];
                _logo.style.display = DisplayStyle.Flex;
                continue;
            }

            logos[i].RemoveFromHierarchy();
        }

        _rememberToggle = root.Q<Toggle>("RememberToggle");
        bool remember = PlayerPrefs.GetInt(RememberKey, 0) == 1;
        if (_rememberToggle != null)
        {
            _rememberToggle.label = "Remember";
            _rememberToggle.text = "Remember";
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

        // Inline styles beat Unity's default TextField chrome and any password-only padding.
        ApplyLoginFieldMetrics(_userInput);
        ApplyLoginFieldMetrics(_passwordInput);
        _userInput.schedule.Execute(() => {
            ApplyLoginFieldMetrics(_userInput);
            ApplyLoginFieldMetrics(_passwordInput);
        });

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

    // ID and PWD must share one box: Unity's password TextField keeps extra inline padding USS cannot clear.
    private static void ApplyLoginFieldMetrics(TextField field) {
        if (field == null)
            return;

        field.Query(className: BaseField<string>.labelUssClassName).ForEach(label => {
            label.style.display = DisplayStyle.None;
        });

        field.style.width = 176;
        field.style.minWidth = 176;
        field.style.maxWidth = 176;
        field.style.height = 17;
        field.style.minHeight = 0;
        field.style.maxHeight = 17;
        field.style.marginTop = 0;
        field.style.marginRight = 0;
        field.style.marginBottom = 0;
        field.style.marginLeft = 0;
        field.style.paddingTop = 0;
        field.style.paddingRight = 0;
        field.style.paddingBottom = 0;
        field.style.paddingLeft = 0;
        field.style.borderTopWidth = 0;
        field.style.borderRightWidth = 0;
        field.style.borderBottomWidth = 0;
        field.style.borderLeftWidth = 0;

        ZeroLoginFieldChrome(field);

        bool foundText = false;
        field.Query(className: TextElement.ussClassName).ForEach(text => {
            if (text.ClassListContains(BaseField<string>.labelUssClassName))
                return;
            foundText = true;
            text.style.paddingLeft = 6;
            text.style.paddingRight = 4;
        });
        if (foundText)
            return;

        VisualElement textInput = field.Q("unity-text-input");
        if (textInput == null)
            textInput = field.Q(className: TextField.inputUssClassName);
        if (textInput == null)
            return;

        textInput.style.paddingLeft = 6;
        textInput.style.paddingRight = 4;
    }

    private static void ZeroLoginFieldChrome(VisualElement element) {
        int count = element.childCount;
        for (int i = 0; i < count; i++) {
            VisualElement child = element[i];
            child.style.marginTop = 0;
            child.style.marginRight = 0;
            child.style.marginBottom = 0;
            child.style.marginLeft = 0;
            child.style.paddingTop = 0;
            child.style.paddingRight = 0;
            child.style.paddingBottom = 0;
            child.style.paddingLeft = 0;
            child.style.borderTopWidth = 0;
            child.style.borderRightWidth = 0;
            child.style.borderBottomWidth = 0;
            child.style.borderLeftWidth = 0;
            if (child.ClassListContains(BaseField<string>.labelUssClassName))
                child.style.display = DisplayStyle.None;
            ZeroLoginFieldChrome(child);
        }
    }
}
