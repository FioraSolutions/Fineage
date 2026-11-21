using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>
/// Binds UI Toolkit labels/buttons/toggles whose text holds an l10n key (class <c>l10n</c>).
/// </summary>
public static class L10nUiBinder
{
    static readonly List<VisualElement> BoundRoots = new List<VisualElement>();

    static L10nUiBinder()
    {
        L10n.LocaleChanged += RebindAll;
    }

    public static void Bind(VisualElement root)
    {
        if (root == null)
            return;
        if (!BoundRoots.Contains(root))
            BoundRoots.Add(root);
        Apply(root);
    }

    public static void Unbind(VisualElement root)
    {
        if (root == null)
            return;
        BoundRoots.Remove(root);
    }

    public static void SetText(Label label, string key)
    {
        if (label == null)
            return;
        label.text = L10n.Get(key);
        label.AddToClassList("l10n");
        label.userData = key;
    }

    public static void SetText(Button button, string key)
    {
        if (button == null)
            return;
        button.text = L10n.Get(key);
        button.AddToClassList("l10n");
        button.userData = key;
        Label inner = button.Q<Label>(className: "button-label");
        if (inner != null)
            inner.text = L10n.Get(key);
    }

    public static void SetText(Toggle toggle, string key)
    {
        if (toggle == null)
            return;
        string text = L10n.Get(key);
        toggle.label = text;
        toggle.AddToClassList("l10n");
        toggle.userData = key;
    }

    static void RebindAll()
    {
        for (int i = BoundRoots.Count - 1; i >= 0; i--)
        {
            VisualElement root = BoundRoots[i];
            if (root == null || root.panel == null)
            {
                BoundRoots.RemoveAt(i);
                continue;
            }

            Apply(root);
        }
    }

    static void Apply(VisualElement root)
    {
        root.Query<Label>(className: "l10n").ForEach(BindLabel);
        root.Query<Button>(className: "l10n").ForEach(BindButton);
        root.Query<Toggle>(className: "l10n").ForEach(BindToggle);
        Label title = root.Q<Label>("windows-name-label");
        if (title != null && (title.ClassListContains("l10n") || LooksLikeKey(title.text)))
            BindLabel(title);
    }

    static bool LooksLikeKey(string text) =>
        !string.IsNullOrEmpty(text) && text.IndexOf('.') > 0 && !text.Contains(" ");

    static void BindLabel(Label label)
    {
        string key = KeyFrom(label);
        if (string.IsNullOrEmpty(key))
            return;
        label.userData = key;
        label.text = L10n.Get(key);
    }

    static void BindButton(Button button)
    {
        string key = KeyFrom(button);
        if (string.IsNullOrEmpty(key))
            return;
        button.userData = key;
        string text = L10n.Get(key);
        button.text = text;
        Label inner = button.Q<Label>(className: "button-label");
        if (inner != null)
            inner.text = text;
    }

    static void BindToggle(Toggle toggle)
    {
        string key = KeyFrom(toggle);
        if (string.IsNullOrEmpty(key))
            return;
        toggle.userData = key;
        toggle.label = L10n.Get(key);
    }

    static string KeyFrom(VisualElement element)
    {
        if (element?.userData is string saved && !string.IsNullOrEmpty(saved))
            return saved;
        string text = element switch
        {
            TextElement te => te.text,
            Toggle t => t.label,
            _ => null,
        };
        return text?.Trim() ?? "";
    }
}