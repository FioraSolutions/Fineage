using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ChatWindow : L2Window
{

    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _messageLabelTemplate;
    private TextField _chatInput;
    private VisualElement _chatInputContainer;
    private ChatTab _activeTab;

    [SerializeField] public List<ChatTab> _tabs;
    [SerializeField] private bool _chatOpened = false;

    public bool ChatOpened { get { return _chatOpened; } }

    /// <summary>True after the tab labels exist. Zone chat lines wait for this.</summary>
    public bool Ready { get; private set; }

    private static ChatWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static ChatWindow Instance { get { return _instance; } }






}
