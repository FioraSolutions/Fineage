using System;

public partial class DescriptionSkillWindow : L2PopupWindow
{

    private static DescriptionSkillWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static DescriptionSkillWindow Instance { get { return _instance; } }
    private DataProviderWindows _dataProvider;
    private VisualElement _content;
    private Label _spLabel;
    private Button _backButton;
    private Button _okButton;
    private AcquireSkillInfoDto _packet;









}
