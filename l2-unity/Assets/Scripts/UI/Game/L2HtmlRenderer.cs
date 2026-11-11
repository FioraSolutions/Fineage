using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

/**
 * @author AbsolutePower
 */
public sealed partial class L2HtmlRenderer
{
    private static readonly Color LinkColor = new Color32(70, 120, 230, 255);
    private static readonly Color LinkHoverColor = new Color32(110, 160, 255, 255);

    private static readonly Regex MultiSpaceRegex = new Regex(@"[ ]{3,}", RegexOptions.Compiled);
    private static readonly Regex VariableRegex = new Regex(@"\$(\w+)", RegexOptions.Compiled);
    private static readonly Regex PercentVariableRegex = new Regex(@"%(\w+)%", RegexOptions.Compiled);
    private static readonly Regex HexColorRegex = new Regex(@"^[0-9a-fA-F]{6}$|^[0-9a-fA-F]{8}$", RegexOptions.Compiled);

    private static readonly Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();
    private static readonly HashSet<string> MissingTextureCache = new HashSet<string>();

    private readonly VisualElement _root;
    private readonly Action<string> _onAction;

    private readonly Stack<VisualElement> _parentStack = new Stack<VisualElement>();
    private readonly Stack<string> _colorStack = new Stack<string>();
    private readonly Stack<float?> _fontSizeStack = new Stack<float?>();
    private readonly Stack<string> _linkActionStack = new Stack<string>();
    private readonly Stack<bool> _centerStack = new Stack<bool>();
    private readonly Stack<TextAnchor> _textAlignStack = new Stack<TextAnchor>();
    private readonly Stack<float> _cellPaddingStack = new Stack<float>();
    private readonly Stack<float> _cellSpacingStack = new Stack<float>();
    private readonly Stack<bool> _noWrapStack = new Stack<bool>();
    private readonly Stack<bool> _preStack = new Stack<bool>();

    private readonly Stack<bool> _boldStack = new Stack<bool>();
    private readonly Stack<bool> _italicStack = new Stack<bool>();
    private readonly Stack<bool> _underlineStack = new Stack<bool>();

    private readonly Dictionary<string, Func<string>> _valueProviders = new Dictionary<string, Func<string>>();
    private readonly Dictionary<string, RadioButton> _radioGroups = new Dictionary<string, RadioButton>();

    private readonly StringBuilder _textBuffer = new StringBuilder(256);
    private readonly Dictionary<string, string> _attrs = new Dictionary<string, string>(16);

    private string _activeColor;
    private float? _activeFontSize;

    private bool _boldMode;
    private bool _italicMode;
    private bool _underlineMode;
    private bool _noWrapMode;
    private bool _preMode;

    private bool _centerMode;
    private bool _insideHead;
    private bool _insideTitle;

    private TextAnchor _activeTextAlign = TextAnchor.MiddleLeft;

    private float _activeCellPadding = 1f;
    private float _activeCellSpacing = 0f;

    private DropdownField _activeDropdown;
    private string _activeDropdownVar;
    private int _activeDropdownSelectedIndex = -1;
    private readonly List<string> _activeDropdownLabels = new List<string>();
    private readonly List<string> _activeDropdownValues = new List<string>();

    private VisualElement CurrentParent => _parentStack.Count > 0 ? _parentStack.Peek() : _root;
    private string CurrentLinkAction => _linkActionStack.Count > 0 ? _linkActionStack.Peek() : null;


    public L2HtmlRenderer(VisualElement root, Action<string> onAction)
    {
        _root = root;
        _onAction = onAction;
    }

    public void Render(string html)
    {
        ClearState();
        _root.Clear();

        _root.style.overflow = Overflow.Hidden;
        _root.style.flexShrink = 1;
        _root.style.flexGrow = 1;
        _root.style.minWidth = 0;
        _root.style.maxWidth = Length.Percent(100);
        _root.style.width = Length.Percent(100);
        _root.style.alignItems = Align.Stretch;

        if (string.IsNullOrWhiteSpace(html))
            return;

        html = Decode(html);

        int i = 0;

        while (i < html.Length)
        {
            char c = html[i];

            if (c == '<')
            {
                int end = FindTagEnd(html, i + 1);

                if (end < 0)
                    break;

                string rawTag = html.Substring(i + 1, end - i - 1).Trim();

                string tagName = rawTag.StartsWith("/")
                    ? ReadTagName(rawTag.Substring(1).Trim()).ToLowerInvariant()
                    : ReadTagName(rawTag).ToLowerInvariant();

                bool inlineFormat = IsInlineFormatTag(tagName);

                if (!_insideHead && !_insideTitle)
                {
                    if (!inlineFormat)
                        FlushText();
                }
                else
                {
                    _textBuffer.Clear();
                }

                HandleTag(rawTag);

                i = end + 1;
            }
            else
            {
                if (!_insideHead && !_insideTitle)
                    _textBuffer.Append(c);

                i++;
            }
        }

        FlushText();
    }

    private void ClearState()
    {
        _parentStack.Clear();
        _colorStack.Clear();
        _fontSizeStack.Clear();
        _linkActionStack.Clear();
        _centerStack.Clear();
        _textAlignStack.Clear();
        _cellPaddingStack.Clear();
        _cellSpacingStack.Clear();
        _noWrapStack.Clear();
        _preStack.Clear();

        _boldStack.Clear();
        _italicStack.Clear();
        _underlineStack.Clear();

        _valueProviders.Clear();
        _radioGroups.Clear();

        _textBuffer.Clear();

        _activeColor = null;
        _activeFontSize = null;

        _boldMode = false;
        _italicMode = false;
        _underlineMode = false;
        _noWrapMode = false;
        _preMode = false;

        _centerMode = false;
        _insideHead = false;
        _insideTitle = false;

        _activeTextAlign = TextAnchor.MiddleLeft;

        _activeCellPadding = 1f;
        _activeCellSpacing = 0f;

        _activeDropdown = null;
        _activeDropdownVar = null;
        _activeDropdownSelectedIndex = -1;
        _activeDropdownLabels.Clear();
        _activeDropdownValues.Clear();
    }
}
