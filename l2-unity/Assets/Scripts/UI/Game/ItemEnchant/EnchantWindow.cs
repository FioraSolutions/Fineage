using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public partial class EnchantWindow : L2PopupWindow
{

    private static EnchantWindow _instance;
    private VisualTreeAsset _inventorySlotTemplate;
    private VisualTreeAsset _inventorySlotChoiceTemplate;
    private VisualElement _slot1;
    private VisualElement _slot2;
    private VisualElement _progressBar;
    private Label _textLabel;
    private const string _defaultTextLabel = "Please place the item to be enchanted in the empty slot below.";
    private VisualElement _progressBarBg;
    private VisualElement _animElement;
    private List<Texture2D> _loadingAnimWaitPng;
    private List<Texture2D> _loadingAnimSuccessPng;
    private List<Texture2D> _loadingAnimFailPng;
    private bool _isStartAnim = false;
    private bool _isRun = false;
    private float _moveDuration = 1.5f;
    private float offsetLeft2 = 76;//default offset element2
    private float offsetLeft1 = 0;//default offset element2
   // private EtcItemgrp _useScroll;
    private EnchantSlot _enchant1;
    private EnchantSlot _enchant2;
    private ItemInstance _selectItem;
    private bool _zoneWire;
    private bool _closingFromServer;
    private bool _awaitingRoll;
    private int _scrollObjectId;
    private bool _frameCloseWired;

    public bool AcceptsTarget
    {
        get { return _zoneWire && !_isWindowHidden; }
    }


    [Obsolete("Use [Inject] / PacketServices instead")]
    public static EnchantWindow Instance
    {
        get { return _instance; }
    }

}
