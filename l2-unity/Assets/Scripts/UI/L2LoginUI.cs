using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class L2LoginUI : L2UI
{
    [SerializeField] private VisualElement _loadingElement;

    private bool loading = false;

    private static L2LoginUI _instance;
    public static L2LoginUI Instance { get { return _instance; } }

    public bool IsLoading { get { return loading; } }

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

        IncomingPacketActions.LoginWindow.AddWindow(_rootVisualContainer);
        IncomingPacketActions.CharSelect.AddWindow(_rootVisualContainer);
        IncomingPacketActions.CharSelect.HideWindow();
        IncomingPacketActions.CharCreate.AddWindow(_rootVisualContainer);
        IncomingPacketActions.CharCreate.HideWindow();
        IncomingPacketActions.LicenseWindow.AddWindow(_rootVisualContainer);
        IncomingPacketActions.LicenseWindow.HideWindow();
        IncomingPacketActions.ServerSelect.AddWindow(_rootVisualContainer);
        IncomingPacketActions.ServerSelect.HideWindow();
        HideMissingMaterialRenderers();
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

    static void HideMissingMaterialRenderers()
    {
        // Lobby ships/FX often ship with null material slots → magenta error mats on Metal.
        var rends = Object.FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < rends.Length; i++)
        {
            var mats = rends[i].sharedMaterials;
            bool bad = mats == null || mats.Length == 0;
            if (!bad)
            {
                for (int m = 0; m < mats.Length; m++)
                {
                    if (mats[m] == null)
                    {
                        bad = true;
                        break;
                    }
                }
            }
            if (bad)
                rends[i].enabled = false;
        }
    }

    System.Collections.IEnumerator ShowLoginWhenReady()
    {
        // AddWindow builds via coroutine — wait until LoginWindow visual exists.
        for (int i = 0; i < 120; i++)
        {
            var login = IncomingPacketActions.LoginWindow;
            if (login != null && login.gameObject != null)
            {
                login.ShowWindow();
                yield break;
            }
            yield return null;
        }
    }

    public void ShowServerSelectWindow() {
        IncomingPacketActions.LoginWindow.HideWindow();
        IncomingPacketActions.LicenseWindow.HideWindow();
        IncomingPacketActions.ServerSelect.ShowWindow();
    }

    public void ShowLicenseWindow() {
        IncomingPacketActions.LoginWindow.HideWindow();
        IncomingPacketActions.LicenseWindow.ShowWindow();
        IncomingPacketActions.ServerSelect.HideWindow();
        IncomingPacketActions.LicenseWindow.DevAutoAgreeIfEnabled();
    }

    public void ShowCharSelectWindow() {
        IncomingPacketActions.LoginWindow.HideWindow();
        IncomingPacketActions.CharCreate.HideWindow();
        IncomingPacketActions.CharSelect.ShowWindow();
        IncomingPacketActions.ServerSelect.HideWindow();
    }

    public void ShowLoginWindow() {
        IncomingPacketActions.CharSelect.HideWindow();
        IncomingPacketActions.LoginWindow.ShowWindow();
        IncomingPacketActions.CharCreate.HideWindow();
        IncomingPacketActions.CharSelect.HideWindow();
        IncomingPacketActions.LicenseWindow.HideWindow();
        IncomingPacketActions.ServerSelect.HideWindow();
    }

    public void ShowCharCreationWindow() {
        IncomingPacketActions.CharSelect.HideWindow();
        IncomingPacketActions.CharCreate.Init();
        IncomingPacketActions.CharCreate.ShowWindow();

    }

    public void SetCharTemplations(List<PlayerTemplates> playerTemplates)
    {
        IncomingPacketActions.CharCreate.Clear();
        IncomingPacketActions.CharCreate.SetPlayerTemplates(playerTemplates);
    }

    public void ShowCharCreationError(string text)
    {
        IncomingPacketActions.CharCreate.SetlabelError(text);
    }
}
