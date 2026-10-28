using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Accept or decline <c>ClanAskJoin</c> (1050). The server drops the invite after 15 seconds.
/// </summary>
public class ClanAskJoinWindow : L2PopupWindow
{
    const float TimeoutSeconds = ZoneV2ClanBook.InviteTimeoutSeconds;

    static ClanAskJoinWindow _instance;
    VisualElement _progressBar;
    Label _invitationText;
    Button _yesButton;
    Button _noButton;
    int _requestorObjectId;
    bool _pendingShow;
    string _pendingText = "";
    Coroutine _countdown;

    public static ClanAskJoinWindow Instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            ClanPacketServices.Bind(ask: this);
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        ClanPacketServices.Bind(ask: this);
    }

    public void Present(ZoneV2ClanAskJoin ask)
    {
        _requestorObjectId = ask.RequestorObjectId;
        string who = string.IsNullOrEmpty(ask.RequestorName) ? "A player" : ask.RequestorName;
        string clan = string.IsNullOrEmpty(ask.ClanName) ? "a clan" : ask.ClanName;
        if (_invitationText == null)
        {
            _pendingShow = true;
            _pendingText = who + " invites you to join the clan " + clan + ".";
            return;
        }

        _invitationText.text = who + " invites you to join the clan " + clan + ".";
        ShowWindow();
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ClanAskJoinWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();
        OnCenterScreen(root);
        if (_pendingShow)
        {
            _pendingShow = false;
            if (_invitationText != null && !string.IsNullOrEmpty(_pendingText))
                _invitationText.text = _pendingText;
            ShowWindow();
        }
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);
        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        _progressBar = GetElementById("ProgressGauge");
        _invitationText = (Label)GetElementById("invitationText");
        _yesButton = (Button)GetElementById("YesButton");
        _noButton = (Button)GetElementById("NoButton");
        RegisterClickWindowEvent(_windowEle, dragArea);
        _yesButton.RegisterCallback<ClickEvent>(OnYesClicked);
        _noButton.RegisterCallback<ClickEvent>(OnNoClicked);
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        if (_progressBar != null)
            _progressBar.style.width = Length.Percent(100);
        if (_countdown != null)
            StopCoroutine(_countdown);
        _countdown = StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        float left = TimeoutSeconds;
        while (left > 0f)
        {
            left -= Time.deltaTime;
            if (_progressBar != null)
                _progressBar.style.width = Length.Percent(Mathf.Clamp01(left / TimeoutSeconds) * 100f);
            yield return null;
        }

        Send(0);
        HideWindow();
    }

    void OnYesClicked(ClickEvent evt)
    {
        Send(1);
        HideWindow();
    }

    void OnNoClicked(ClickEvent evt)
    {
        Send(0);
        HideWindow();
    }

    void Send(int response)
    {
        if (_requestorObjectId <= 0)
            return;
        GameClient game = ClanPacketServices.Game;
        if (game != null)
            game.Send(new ClanAnswerJoinRequestCommand(_requestorObjectId, response));
        _requestorObjectId = 0;
    }

    public override void HideWindow()
    {
        if (_countdown != null)
        {
            StopCoroutine(_countdown);
            _countdown = null;
        }

        base.HideWindow();
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
        if (_yesButton != null)
            _yesButton.UnregisterCallback<ClickEvent>(OnYesClicked);
        if (_noButton != null)
            _noButton.UnregisterCallback<ClickEvent>(OnNoClicked);
    }
}
