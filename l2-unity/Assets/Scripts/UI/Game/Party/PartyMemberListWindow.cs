using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class PartyMemberListWindow : L2Window
{
    private static PartyMemberListWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static PartyMemberListWindow Instance => _instance;

    private VisualElement _memberRows;
    private Label _titleLabel;








}
