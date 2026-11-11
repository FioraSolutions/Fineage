using System;

public partial class CharCreationWindow : L2Window
{
    [Inject] LoginRuntime _login;

    private CharacterCreator Creator => LoginPacketServices.Creator;

    private LoginCameraManager LoginCamera => LoginPacketServices.LoginCamera;

    private GameManager Manager => LoginPacketServices.Manager;

    private VisualTreeAsset _arrowInputTemplate;
    private ArrowInputManipulator hairstyleManipulator;
    private ArrowInputManipulator hairColorManipulator;
    private ArrowInputManipulator faceManipulator;
    private ArrowInputManipulator genderManipulator;
    private ArrowInputManipulator classManipulator;
    private ArrowInputManipulator raceManipulator;
    private TextField userInputField;
    private VisualElement pawnRotateWindow;
    private Label labelError;
    private Button pawnZoominButton;
    private List<PlayerTemplates> _playerTemplates;
    private bool isInit = false;
    private static CharCreationWindow _instance;
    [System.Obsolete("Use [Inject] or LoginPacketServices.CharCreate instead")]
    public static CharCreationWindow Instance { get { return _instance; } }

}
