using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ServerSelectWindow : L2Window
{
    [Inject] LoginRuntime _login;
    [Inject] IZoneTransport _zoneTransport;

    private IZoneTransport Transport =>
        _zoneTransport ?? (App.HasContainer ? App.Resolve<IZoneTransport>() : null);

    private LoginWindow LoginWin => LoginPacketServices.LoginWindow;

    private VisualTreeAsset _serverElementTemplate;
    private VisualElement _listContentContainer;
    private List<VisualElement> _serverElements;
    private List<ServerData> _serverData;
    private int _selectedServerId = -1;
    private ServerData _selectServerData;

    private static ServerSelectWindow _instance;
    [System.Obsolete("Use [Inject] or LoginPacketServices.ServerSelect instead")]
    public static ServerSelectWindow Instance { get { return _instance; } }











}
