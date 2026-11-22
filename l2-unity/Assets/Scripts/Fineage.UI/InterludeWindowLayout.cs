using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Builds a window from the layout asset generated from docs/ui-original.
/// Missing window, texture, or resolved size is logged with the window and control name.
/// </summary>
public static class InterludeWindowLayout
{
    public const string ResourcesKey = "Data/UI/Interlude/WindowLayouts";
    const string PlacedClass = "il-layout-placed";
    const string TextureBoundClass = "il-tex-bound";
    const string DefaultFontResource = "Data/UI/Font/tahoma";

    static bool _loadAttempted;
    static InterludeLayoutFile _file;
    static Dictionary<string, InterludeLayoutWindow> _byName;
    static bool _inLayout;
    static readonly HashSet<string> _logged = new HashSet<string>();

    public static bool Apply(VisualElement host, string windowName)
    {
        if (host == null)
        {
            Debug.LogError("[InterludeLayout] Apply was given a null host for window '" + windowName + "'.");
            return false;
        }

        InterludeLayoutWindow window = Find(windowName);
        if (window == null)
            return false;

        bool placed = host.ClassListContains(PlacedClass);
        float rootWidth;
        float rootHeight;
        bool haveSize = TryLiveRootSize(host, window, placed, out rootWidth, out rootHeight);
        if (!placed)
            PlaceHost(host, window, rootWidth, rootHeight);

        if (!haveSize)
            return true;

        LayoutChildren(host, window, rootWidth, rootHeight);
        return true;
    }

    public static bool TryWindowSize(string windowName, out float width, out float height)
    {
        width = 0f;
        height = 0f;
        InterludeLayoutWindow window = Find(windowName);
        if (window == null || window.controls == null || window.controls.Length == 0)
            return false;
        width = window.controls[0].width;
        height = window.controls[0].height;
        return width > 0f && height > 0f;
    }

    public static bool TryMeasure(string windowName, string controlName, out float width, out float height)
    {
        width = 0f;
        height = 0f;
        InterludeLayoutWindow window = Find(windowName);
        if (window == null || window.controls == null || window.controls.Length == 0)
            return false;

        float rootWidth = window.controls[0].width;
        float rootHeight = window.controls[0].height;
        if (rootWidth <= 0f || rootHeight <= 0f)
            return false;

        Dictionary<string, InterludeLayoutGeometry.Size> rects;
        string error;
        if (!InterludeLayoutGeometry.TryResolve(window, rootWidth, rootHeight, out rects, out error))
        {
            LogOnce(windowName + "/" + controlName, "[InterludeLayout] " + windowName + ": " + error);
            return false;
        }

        InterludeLayoutGeometry.Size size;
        if (!rects.TryGetValue(controlName, out size) || !size.HasWidth || !size.HasHeight)
            return false;
        width = size.Width;
        height = size.Height;
        return width > 0f && height > 0f;
    }

    public static string ControlText(string windowName, string controlName)
    {
        InterludeLayoutWindow window = Find(windowName);
        if (window == null || window.controls == null)
            return "";
        for (int i = 0; i < window.controls.Length; i++)
        {
            if (window.controls[i].name == controlName)
                return window.controls[i].text ?? "";
        }

        return "";
    }

    public static int ControlMaxLength(string windowName, string controlName)
    {
        InterludeLayoutControl control = FindControl(windowName, controlName);
        return control == null ? 0 : control.maxLength;
    }

    public static void SetPressed(VisualElement element, string windowName, string controlName, bool pressed)
    {
        if (element == null)
            return;
        InterludeLayoutControl control = FindControl(windowName, controlName);
        if (control == null)
            return;
        string path = pressed && !string.IsNullOrEmpty(control.texturePushed)
            ? control.texturePushed
            : control.texture;
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError(
                "[InterludeLayout] window '" + windowName + "' control '" + controlName +
                "' has no " + (pressed ? "pushed" : "normal") + " texture.");
            return;
        }

        Texture2D texture = LoadTexture(path, windowName, controlName, pressed ? "pushedTex" : "texture");
        if (texture != null)
            element.style.backgroundImage = new StyleBackground(texture);
    }

#if UNITY_EDITOR
    public static void ResetForEditModeTests()
    {
        _loadAttempted = false;
        _file = null;
        _byName = null;
        _logged.Clear();
    }
#endif

    static void PlaceHost(VisualElement host, InterludeLayoutWindow window, float rootWidth, float rootHeight)
    {
        host.AddToClassList(PlacedClass);
        host.style.position = Position.Absolute;
        host.style.marginLeft = 0f;
        host.style.marginRight = 0f;
        host.style.marginTop = 0f;
        host.style.marginBottom = 0f;
        host.style.paddingLeft = 0f;
        host.style.paddingRight = 0f;
        host.style.paddingTop = 0f;
        host.style.paddingBottom = 0f;
        host.style.overflow = Overflow.Visible;
        if (window.stretchToParent != 0)
        {
            host.style.left = 0f;
            host.style.top = 0f;
            host.style.right = 0f;
            host.style.bottom = 0f;
            host.style.width = Length.Percent(100f);
            host.style.height = Length.Percent(100f);
        }
        else
        {
            host.style.width = rootWidth;
            host.style.height = rootHeight;
            if (window.placementKnown == 0)
            {
                LogOnce(
                    "place:" + window.name,
                    "[InterludeLayout] window '" + window.name +
                    "' has no screen placement in the extracted layout. " +
                    "Internal controls were placed; the window was not moved.");
            }
            else
            {
                PlaceOnParent(host, window);
            }
        }

        string captured = window.name;
        host.RegisterCallback<GeometryChangedEvent>(_ => OnHostGeometry(host, captured));
    }

    static void OnHostGeometry(VisualElement host, string windowName)
    {
        if (_inLayout || host == null || !host.ClassListContains(PlacedClass))
            return;
        InterludeLayoutWindow window = Find(windowName);
        if (window == null)
            return;
        float rootWidth;
        float rootHeight;
        if (!TryLiveRootSize(host, window, true, out rootWidth, out rootHeight))
            return;
        LayoutChildren(host, window, rootWidth, rootHeight);
    }

    static bool TryLiveRootSize(
        VisualElement host,
        InterludeLayoutWindow window,
        bool placed,
        out float rootWidth,
        out float rootHeight)
    {
        rootWidth = 0f;
        rootHeight = 0f;
        if (window.controls == null || window.controls.Length == 0)
        {
            Debug.LogError("[InterludeLayout] window '" + window.name + "' has no controls.");
            return false;
        }

        if (window.stretchToParent != 0)
        {
            float w = host.resolvedStyle.width;
            float h = host.resolvedStyle.height;
            if (float.IsNaN(w) || float.IsNaN(h) || w <= 1f || h <= 1f)
                return false;
            rootWidth = w;
            rootHeight = h;
            return true;
        }

        rootWidth = window.controls[0].width;
        rootHeight = window.controls[0].height;
        if (rootWidth <= 0f || rootHeight <= 0f)
        {
            Debug.LogError(
                "[InterludeLayout] window '" + window.name +
                "' resolved no positive size (" + rootWidth + "x" + rootHeight + ").");
            return false;
        }

        if (placed)
        {
            float w = host.resolvedStyle.width;
            float h = host.resolvedStyle.height;
            if (!float.IsNaN(w) && w > 1f)
                rootWidth = w;
            if (!float.IsNaN(h) && h > 1f)
                rootHeight = h;
        }

        return true;
    }

    static void PlaceOnParent(VisualElement host, InterludeLayoutWindow window)
    {
        float width = window.controls[0].width;
        float height = window.controls[0].height;
        host.style.left = StyleKeyword.Auto;
        host.style.right = StyleKeyword.Auto;
        host.style.top = StyleKeyword.Auto;
        host.style.bottom = StyleKeyword.Auto;
        host.style.translate = new Translate(0f, 0f);
        string alignment = window.alignment ?? "";
        float x = window.placeX;
        float y = window.placeY;
        if (alignment == "TOP_LEFT")
        {
            host.style.left = x;
            host.style.top = y;
            return;
        }

        if (alignment == "TOP_CENTER")
        {
            host.style.left = Length.Percent(50f);
            host.style.top = y;
            host.style.translate = new Translate(Pixel(-width * 0.5f + x), Pixel(0f));
            return;
        }

        if (alignment == "TOP_RIGHT")
        {
            host.style.right = -x;
            host.style.top = y;
            return;
        }

        if (alignment == "BOTTOM_LEFT")
        {
            host.style.left = x;
            host.style.bottom = -y;
            return;
        }

        if (alignment == "BOTTOM_CENTER")
        {
            host.style.left = Length.Percent(50f);
            host.style.bottom = -y;
            host.style.translate = new Translate(Pixel(-width * 0.5f + x), Pixel(0f));
            return;
        }

        if (alignment == "BOTTOM_RIGHT")
        {
            host.style.right = -x;
            host.style.bottom = -y;
            return;
        }

        if (alignment == "CENTER_LEFT")
        {
            host.style.left = x;
            host.style.top = Length.Percent(50f);
            host.style.translate = new Translate(Pixel(0f), Pixel(-height * 0.5f + y));
            return;
        }

        if (alignment == "CENTER_RIGHT")
        {
            host.style.right = -x;
            host.style.top = Length.Percent(50f);
            host.style.translate = new Translate(Pixel(0f), Pixel(-height * 0.5f + y));
            return;
        }

        if (alignment == "CENTER")
        {
            host.style.left = Length.Percent(50f);
            host.style.top = Length.Percent(50f);
            host.style.translate = new Translate(Pixel(-width * 0.5f + x), Pixel(-height * 0.5f + y));
            return;
        }

        Debug.LogError(
            "[InterludeLayout] window '" + window.name +
            "' alignment '" + alignment + "' is not an extracted alignment.");
    }

    static Length Pixel(float value)
    {
        return new Length(value, LengthUnit.Pixel);
    }

    static void LayoutChildren(VisualElement host, InterludeLayoutWindow window, float rootWidth, float rootHeight)
    {
        Dictionary<string, InterludeLayoutGeometry.Size> rects;
        string error;
        _inLayout = true;
        try
        {
            if (!InterludeLayoutGeometry.TryResolve(window, rootWidth, rootHeight, out rects, out error))
            {
                Debug.LogError("[InterludeLayout] window '" + window.name + "': " + error);
                return;
            }

            for (int i = 1; i < window.controls.Length; i++)
            {
                InterludeLayoutControl control = window.controls[i];
                InterludeLayoutGeometry.Size size;
                if (!rects.TryGetValue(control.name, out size))
                    continue;
                VisualElement element = EnsureElement(host, window, control);
                if (element == null)
                    continue;
                ApplyRect(window.name, control, element, size);
                BindTexturesOnce(window.name, control, element);
            }
        }
        finally
        {
            _inLayout = false;
        }
    }

    static VisualElement EnsureElement(VisualElement host, InterludeLayoutWindow window, InterludeLayoutControl control)
    {
        VisualElement parent = host;
        if (!string.IsNullOrEmpty(control.parent) && control.parent != window.name)
        {
            parent = host.Q<VisualElement>(control.parent);
            if (parent == null)
            {
                Debug.LogError(
                    "[InterludeLayout] window '" + window.name + "' control '" + control.name +
                    "' parent '" + control.parent + "' was not created.");
                parent = host;
            }
        }

        VisualElement element = host.Q<VisualElement>(control.name);
        if (element == null)
        {
            element = CreateElement(control);
            element.name = control.name;
            element.AddToClassList("il-xdat");
            parent.Add(element);
            return element;
        }

        if (element.parent != parent)
            parent.Add(element);
        return element;
    }

    static VisualElement CreateElement(InterludeLayoutControl control)
    {
        string type = control.type ?? "";
        if (type == "Button" || type == "TabButton")
        {
            Button button = new Button();
            ClearChrome(button);
            if (type == "TabButton")
                button.AddToClassList("tab-header");
            if (!string.IsNullOrEmpty(control.text))
                button.text = control.text;
            return button;
        }

        if (type == "TextBox" || type == "Text" || type == "NameCtrl" || type == "TextOrEdit")
        {
            Label label = new Label();
            if (!string.IsNullOrEmpty(control.text))
                label.text = control.text;
            label.pickingMode = PickingMode.Ignore;
            return label;
        }

        VisualElement element = new VisualElement();
        element.pickingMode = type == "EditBox" || type == "ComboBox" || type == "TreeCtrl" || type == "MinimapCtrl"
            ? PickingMode.Position
            : PickingMode.Ignore;
        return element;
    }

    static void ClearChrome(Button button)
    {
        button.style.marginLeft = 0f;
        button.style.marginRight = 0f;
        button.style.marginTop = 0f;
        button.style.marginBottom = 0f;
        button.style.paddingLeft = 0f;
        button.style.paddingRight = 0f;
        button.style.paddingTop = 0f;
        button.style.paddingBottom = 0f;
        button.style.borderTopWidth = 0f;
        button.style.borderBottomWidth = 0f;
        button.style.borderLeftWidth = 0f;
        button.style.borderRightWidth = 0f;
        button.style.backgroundColor = Color.clear;
    }

    static void ApplyRect(
        string windowName,
        InterludeLayoutControl control,
        VisualElement element,
        InterludeLayoutGeometry.Size size)
    {
        element.style.position = Position.Absolute;
        element.style.left = size.Left;
        element.style.top = size.Top;
        element.style.marginLeft = 0f;
        element.style.marginRight = 0f;
        element.style.marginTop = 0f;
        element.style.marginBottom = 0f;
        // StatusIconCtrl is the anchor the client fills with icons at runtime.
        // Keep the extracted size as the minimum and do not clamp icons added later.
        if ((control.type ?? "") == "StatusIconCtrl" && size.HasWidth && size.HasHeight && size.Width > 0f && size.Height > 0f)
        {
            element.style.minWidth = size.Width;
            element.style.minHeight = size.Height;
            element.style.overflow = Overflow.Visible;
            if (element.childCount == 0)
            {
                element.style.width = size.Width;
                element.style.height = size.Height;
            }

            return;
        }

        if (size.HasWidth && size.Width > 0f)
            element.style.width = size.Width;
        else if (control.width > -9000f && (!size.HasWidth || size.Width <= 0f))
        {
            LogOnce(
                "w:" + windowName + "/" + control.name,
                "[InterludeLayout] window '" + windowName + "' control '" + control.name +
                "' width " + control.width + " resolved to " + size.Width + ".");
        }

        if (size.HasHeight && size.Height > 0f)
            element.style.height = size.Height;
        else if (control.height > -9000f && (!size.HasHeight || size.Height <= 0f))
        {
            LogOnce(
                "h:" + windowName + "/" + control.name,
                "[InterludeLayout] window '" + windowName + "' control '" + control.name +
                "' height " + control.height + " resolved to " + size.Height + ".");
        }
    }

    static void BindTexturesOnce(string windowName, InterludeLayoutControl control, VisualElement element)
    {
        if (element.ClassListContains(TextureBoundClass))
            return;
        element.AddToClassList(TextureBoundClass);

        string type = control.type ?? "";
        bool bar = type == "StatusBar" || type == "BarCtrl";
        if (bar)
        {
            Stamp(element, control.textureBack, windowName, control.name, "backTexture");
            VisualElement fill = new VisualElement();
            fill.name = control.name + "__fill";
            fill.AddToClassList("l2-bar-percent-fill");
            fill.pickingMode = PickingMode.Ignore;
            fill.style.position = Position.Absolute;
            fill.style.left = 0f;
            fill.style.top = 0f;
            fill.style.height = Length.Percent(100f);
            fill.style.width = Length.Percent(100f);
            Stamp(fill, control.texture, windowName, control.name, "foreTexture");
            element.Add(fill);

            Label label = new Label(control.text ?? "");
            label.name = control.name + "__text";
            label.AddToClassList("bar-label");
            label.AddToClassList("bar-label-numbers");
            label.pickingMode = PickingMode.Ignore;
            label.style.position = Position.Absolute;
            label.style.left = 0f;
            label.style.top = 0f;
            label.style.right = 0f;
            label.style.bottom = 0f;
            ApplyTextStyle(windowName, control, label);
            element.Add(label);

            return;
        }

        Stamp(element, control.texture, windowName, control.name, "texture");
        if (!string.IsNullOrEmpty(control.texturePushed) && type != "TabButton")
        {
            Texture2D normal = LoadTexture(control.texture, windowName, control.name, "texture");
            Texture2D pushed = LoadTexture(control.texturePushed, windowName, control.name, "pushedTex");
            if (normal != null && pushed != null)
            {
                element.RegisterCallback<PointerDownEvent>(_ =>
                {
                    element.style.backgroundImage = new StyleBackground(pushed);
                });
                element.RegisterCallback<PointerUpEvent>(_ =>
                {
                    element.style.backgroundImage = new StyleBackground(normal);
                });
            }
        }

        if (element is Label || element is Button)
            ApplyTextStyle(windowName, control, element);
    }

    static void ApplyTextStyle(string windowName, InterludeLayoutControl control, VisualElement element)
    {
        if (control.fontType != -9999)
        {
            LogOnce(
                "font:" + windowName + "/" + control.name,
                "[InterludeLayout] window '" + windowName + "' control '" + control.name +
                "' fontType " + control.fontType + " has no face in the extracted layout.");
        }
        else
        {
            Font font = Resources.Load<Font>(DefaultFontResource);
            if (font == null)
            {
                LogOnce(
                    "font-missing",
                    "[InterludeLayout] default font resource '" + DefaultFontResource + "' did not load.");
            }
            else
            {
                element.style.unityFont = font;
            }
        }

        if (!string.IsNullOrEmpty(control.fontColor))
        {
            Color color;
            if (!TryParseColor(control.fontColor, out color))
            {
                Debug.LogError(
                    "[InterludeLayout] window '" + windowName + "' control '" + control.name +
                    "' fontColor '" + control.fontColor + "' is not a hex color.");
            }
            else
            {
                element.style.color = color;
            }
        }

        if (string.IsNullOrEmpty(control.textAlign))
            return;
        if (control.textAlign == "Center")
            element.style.unityTextAlign = TextAnchor.MiddleCenter;
        else if (control.textAlign == "Left")
            element.style.unityTextAlign = TextAnchor.MiddleLeft;
        else if (control.textAlign == "Right")
            element.style.unityTextAlign = TextAnchor.MiddleRight;
        else
        {
            Debug.LogError(
                "[InterludeLayout] window '" + windowName + "' control '" + control.name +
                "' textAlign '" + control.textAlign + "' is not an extracted alignment.");
        }
    }

    static void Stamp(VisualElement element, string path, string windowName, string controlName, string slot)
    {
        Texture2D texture = LoadTexture(path, windowName, controlName, slot);
        if (texture == null)
            return;
        element.style.backgroundImage = new StyleBackground(texture);
        element.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        element.style.backgroundColor = Color.clear;
    }

    static Texture2D LoadTexture(string path, string windowName, string controlName, string slot)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        Texture2D texture = Resources.Load<Texture2D>(path);
        if (texture == null)
        {
            Debug.LogError(
                "[InterludeLayout] window '" + windowName + "' control '" + controlName +
                "' " + slot + " '" + path + "' did not load.");
        }

        return texture;
    }

    static bool TryParseColor(string hex, out Color color)
    {
        color = default(Color);
        if (string.IsNullOrEmpty(hex) || hex[0] != '#')
            return false;
        string body = hex.Substring(1);
        if (body.Length != 6 && body.Length != 8)
            return false;
        int offset = body.Length == 8 ? 2 : 0;
        byte alpha = 255;
        if (body.Length == 8 && !byte.TryParse(body.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out alpha))
            return false;
        byte red;
        byte green;
        byte blue;
        if (!byte.TryParse(body.Substring(offset, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out red))
            return false;
        if (!byte.TryParse(body.Substring(offset + 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out green))
            return false;
        if (!byte.TryParse(body.Substring(offset + 4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out blue))
            return false;
        color = new Color32(red, green, blue, alpha);
        return true;
    }

    static InterludeLayoutControl FindControl(string windowName, string controlName)
    {
        InterludeLayoutWindow window = Find(windowName);
        if (window == null || window.controls == null)
            return null;
        for (int i = 0; i < window.controls.Length; i++)
        {
            if (window.controls[i].name == controlName)
                return window.controls[i];
        }

        Debug.LogError(
            "[InterludeLayout] window '" + windowName + "' control '" + controlName +
            "' is not in '" + ResourcesKey + "'.");
        return null;
    }

    static InterludeLayoutWindow Find(string windowName)
    {
        EnsureLoaded();
        if (_byName == null)
            return null;
        InterludeLayoutWindow window;
        if (_byName.TryGetValue(windowName, out window))
            return window;
        Debug.LogError(
            "[InterludeLayout] window '" + windowName +
            "' is not in '" + ResourcesKey + "'.");
        return null;
    }

    static void EnsureLoaded()
    {
        if (_loadAttempted)
            return;
        _loadAttempted = true;
        TextAsset asset = Resources.Load<TextAsset>(ResourcesKey);
        if (asset == null)
        {
            Debug.LogError(
                "[InterludeLayout] missing layout asset Resources key '" + ResourcesKey +
                "' (generated from l2-unity/docs/ui-original).");
            return;
        }

        _file = JsonUtility.FromJson<InterludeLayoutFile>(asset.text);
        if (_file == null || _file.windows == null || _file.windows.Length == 0)
        {
            Debug.LogError("[InterludeLayout] layout asset '" + ResourcesKey + "' has no windows.");
            _file = null;
            return;
        }

        _byName = new Dictionary<string, InterludeLayoutWindow>();
        for (int i = 0; i < _file.windows.Length; i++)
        {
            InterludeLayoutWindow window = _file.windows[i];
            if (window == null || string.IsNullOrEmpty(window.name))
            {
                Debug.LogError("[InterludeLayout] layout asset '" + ResourcesKey + "' has a window with no name.");
                continue;
            }

            _byName[window.name] = window;
        }
    }

    static void LogOnce(string key, string message)
    {
        if (_logged.Contains(key))
            return;
        _logged.Add(key);
        Debug.LogError(message);
    }
}
