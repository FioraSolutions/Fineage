using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClanAskJoinWindow : L2PopupWindow
{
    const float TimeoutSeconds = NetClanBook.InviteTimeoutSeconds;

    static ClanAskJoinWindow _instance;
    VisualElement _progressBar;
    Label _invitationText;
    Button _yesButton;
    Button _noButton;
    int _requestorObjectId;
    bool _pendingShow;
    string _pendingText = "";
    Coroutine _countdown;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static ClanAskJoinWindow Instance => _instance;













}
