using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

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




}
