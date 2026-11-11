using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class PartyInvitationWindow : L2PopupWindow
{

    private static PartyInvitationWindow _instance;
    private VisualElement _progressBar;
    private VisualElement _progressBarBg;
    private Label _invitationText;
    private Button _yesButton;
    private Button _noButton;

    private string _inviterName;
    private string _lootDistribution;
    private int _requestorObjectId;
    private float _timeoutSeconds = 20f;
    private Coroutine _countdownCoroutine;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static PartyInvitationWindow Instance
    {
        get { return _instance; }
    }

    public event Action<bool> OnInvitationResponse;



















}
