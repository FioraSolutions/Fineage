using System;

public partial class CharSelectWindow : L2Window
{
    [Inject] LoginRuntime _login;

    private CharacterSelector Characters => LoginPacketServices.Characters;

    private GameManager Manager => LoginPacketServices.Manager;

    private VisualTreeAsset _arrowInputTemplate;
    private ArrowInputManipulator _charNameManipulator;
    private List<CharSelectionInfoPackage> _characters;
    private List<CharSelectInfoPackage> _charactersInterlude;
    private List<LobbyCharacter> _lobbyCharacters;
    private int _lobbySelectIndex = -1;
    private bool _deleteArmed;
    private int _deleteArmedSlot = -1;
    private Button _startButton;
    private Button _deleteButton;
    private Button _createButton;
    private Label _deleteButtonLabel;
    private Label _lobbyStatus;

    private Label _levelLabel;
    private Label _classLabel;
    private Label _hpLabel;
    private Label _mpLabel;
    private Label _expLabel;
    private Label _spLabel;
    private Label _karmaLabel;

    private VisualElement _HPBar;
    private VisualElement _HPBarBG;
    private VisualElement _MPBar;
    private VisualElement _MPBarBG;
    private VisualElement _ExpBarBG;
    private VisualElement _ExpBar;
    private CharSelectInfoPackage _selectChar;

    private static CharSelectWindow _instance;
    [System.Obsolete("Use [Inject] or LoginPacketServices.CharSelect instead")]
    public static CharSelectWindow Instance { get { return _instance; } }








}
