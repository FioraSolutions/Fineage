using System;

public partial class L2LoginUI : L2UI
{
    [Inject] LoginRuntime _login;

    [SerializeField] private VisualElement _loadingElement;

    private bool loading = false;

    private static L2LoginUI _instance;
    [System.Obsolete("Use [Inject] or LoginPacketServices.Ui instead")]
    public static L2LoginUI Instance { get { return _instance; } }

    public bool IsLoading { get { return loading; } }

    private LoginWindow LoginWin => LoginPacketServices.LoginWindow;

    private LicenseWindow LicenseWin => LoginPacketServices.LicenseWindow;

    private ServerSelectWindow ServerWin => LoginPacketServices.ServerSelect;

    private CharSelectWindow CharSelectWin => LoginPacketServices.CharSelect;

    private CharCreationWindow CharCreateWin => LoginPacketServices.CharCreate;
















}
