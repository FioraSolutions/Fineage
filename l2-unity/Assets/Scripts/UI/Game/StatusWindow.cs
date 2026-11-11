using System;

﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class StatusWindow : L2Window
{

    private Label _nameLabel;
    private Label _levelLabel;
    private Label _HPTextLabel;
    private Label _MPTextLabel;
    private Label _CPTextLabel;
    private VisualElement _CPBar;
    private VisualElement _CPBarBG;
    private VisualElement _HPBar;
    private VisualElement _HPBarBG;
    private VisualElement _MPBar;
    private VisualElement _MPBarBG;
    private VisualElement _EXPBar;
    private VisualElement _EXPBarBG;
    private Label _EXPTextLabel;

    [SerializeField] private float _statusWindowMinWidth = 175.0f;
    [SerializeField] private float _statusWindowMaxWidth = 400.0f;

    private static StatusWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static StatusWindow Instance { get { return _instance; } }

}
