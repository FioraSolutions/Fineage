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

    private LoginWindow LoginWin
    {
        get { return _login != null ? _login.LoginWindow : IncomingPacketActions.LoginWindow; }
    }

    private LicenseWindow LicenseWin
    {
        get { return _login != null ? _login.License : IncomingPacketActions.LicenseWindow; }
    }

    private ServerSelectWindow ServerWin
    {
        get { return _login != null ? _login.Servers : IncomingPacketActions.ServerSelect; }
    }

    private CharSelectWindow CharSelectWin
    {
        get { return _login != null ? _login.CharSelect : IncomingPacketActions.CharSelect; }
    }

    private CharCreationWindow CharCreateWin
    {
        get { return _login != null ? _login.CharCreate : IncomingPacketActions.CharCreate; }
    }

    public void SetLoading(bool loading)
    {
        this.loading = loading;
    }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DiBootstrap.EnsureLoginScope();
        } else {
            Destroy(this);
        }
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
        IncomingPacketActions.BindLogin(null);
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
