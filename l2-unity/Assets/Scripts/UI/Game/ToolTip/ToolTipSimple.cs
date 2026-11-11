public partial class ToolTipSimple : L2PopupWindow, IToolTips
{
    private ShowToolTip _showToolTip;

    private static ToolTipSimple _instance;
    private VisualElement _content;
    private VisualTreeAsset _windowTemplateWeapon;
    private VisualTreeAsset _windowTemplateSkill;
    private VisualTreeAsset _windowTemplateSimple;
    private VisualTreeAsset _windowTemplateString;
    private VisualTreeAsset _windowTemplateAcccesories;
    private VisualTreeAsset _windowTemplateRequiredAcccesories;
    private VisualTreeAsset _windowTemplateArmor;
    private VisualTreeAsset _setsElements;
    private VisualTreeAsset _setsEffects;
    private VisualElement _contentInside;
    private float _lastHeightContent = 0;
    private float _heightContent = 0;
    private float _widtchContent = 0;
    private VisualElement _selectShow;
    private TooltipDataProvider _dataProvider;
    private CalcNewPosition _calcPosition;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static ToolTipSimple Instance { get { return _instance; } }



}
