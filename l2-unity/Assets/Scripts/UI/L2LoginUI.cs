using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class L2LoginUI : L2UI
{
    [Inject] LoginRuntime _login;

    [SerializeField] private VisualElement _loadingElement;

    private bool loading = false;

    private static L2LoginUI _instance;
    public static L2LoginUI Instance { get { return _instance; } }

    public bool IsLoading { get { return loading; } }

    private LoginWindow LoginWin => LoginPacketServices.LoginWindow;

    private LicenseWindow LicenseWin => LoginPacketServices.LicenseWindow;

    private ServerSelectWindow ServerWin => LoginPacketServices.ServerSelect;

    private CharSelectWindow CharSelectWin => LoginPacketServices.CharSelect;

    private CharCreationWindow CharCreateWin => LoginPacketServices.CharCreate;

    public void SetLoading(bool loading)
    {
        this.loading = loading;
    }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DiBootstrap.EnsureLoginScope();
            WireLoginPacketServices();
        } else {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireLoginPacketServices();
    }

    private void WireLoginPacketServices()
    {
        LoginPacketServices.Bind(login: _login);
    }

    protected override void Update() {
        base.Update();
    }

    public void OnManualDestroy() {
        
        if (loading)
        {
           //Debug.Log("L2LoginUI Manual : DESTROYYYY");
            _instance = null;
        }
    }

    private void OnDestroy()
    { 
        LoginPacketServices.ClearLoginRuntime();
        if (loading)
        {
            //Debug.Log("L2LoginUI Auto : DESTROYYYY");
            _instance = null;
        }
    }

    protected override void LoadUI() {
        base.LoadUI();

        LoginWin.AddWindow(_rootVisualContainer);
        CharSelectWin.AddWindow(_rootVisualContainer);
        CharSelectWin.HideWindow();
        CharCreateWin.AddWindow(_rootVisualContainer);
        CharCreateWin.HideWindow();
        LicenseWin.AddWindow(_rootVisualContainer);
        LicenseWin.HideWindow();
        ServerWin.AddWindow(_rootVisualContainer);
        ServerWin.HideWindow();
        // Early pass (menu-only). SceneLoader runs a second pass after additive l2_lobby.
        LobbyBrokenMaterialHider.HideBrokenRenderers("login_ui_load");
        InterludeSplashPlayer.Present(RootElement);
        InterludeLoginProof.Arm(RootElement);
        StartCoroutine(ShowLoginWhenReady());
        try
        {
            SkillAnimationDatabase.Initialize();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[L2LoginUI] SkillAnimationDatabase.Initialize failed: " + ex.Message);
        }
    }

    System.Collections.IEnumerator ShowLoginWhenReady()
    {
        // AddWindow builds via coroutine — wait until LoginWindow visual exists.
        for (int i = 0; i < 120; i++)
        {
            var login = LoginWin;
            if (login != null && login.gameObject != null)
            {
                login.ShowWindow();
                yield break;
            }
            yield return null;
        }
    }

    public void ShowServerSelectWindow() {
        LoginWin.HideWindow();
        LicenseWin.HideWindow();
        ServerWin.ShowWindow();
    }

    public void ShowLicenseWindow() {
        LoginWin.HideWindow();
        LicenseWin.ShowWindow();
        ServerWin.HideWindow();
        LicenseWin.DevAutoAgreeIfEnabled();
    }

    public void ShowCharSelectWindow() {
        LoginWin.ShowErrorText("");
        LoginWin.HideWindow();
        CharCreateWin.HideWindow();
        CharSelectWin.ShowWindow();
        ServerWin.HideWindow();
    }

    public void ShowLoginWindow() {
        CharSelectWin.HideWindow();
        LoginWin.ShowWindow();
        CharCreateWin.HideWindow();
        CharSelectWin.HideWindow();
        LicenseWin.HideWindow();
        ServerWin.HideWindow();
    }

    public void ShowCharCreationWindow() {
        CharSelectWin.HideWindow();
        CharCreateWin.Init();
        CharCreateWin.ShowWindow();

    }

    public void SetCharTemplations(List<PlayerTemplates> playerTemplates)
    {
        CharCreateWin.Clear();
        CharCreateWin.SetPlayerTemplates(playerTemplates);
    }

    public void ShowCharCreationError(string text)
    {
        CharCreateWin.SetlabelError(text);
    }
}
