using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using static UnityEngine.Rendering.DebugUI;

public partial class BufferPanel : L2Window
{

    private static BufferPanel _instance;
    private VisualTreeAsset _barSlotTemplate;
    private float _defaultLeftOffset = 200;
    private Dictionary<int, DataCell> _dictElement;

    private int[] _deathPenalty;
    private int[] _weightPenalty;
    private int[] _weaponPenalty;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static BufferPanel Instance { get { return _instance; } }
    public BLink _bLink;
    public VisualElement _content;
    private FilterData _filterData;

    [Inject] SkillgrpTable _skillGrps;

    private SkillgrpTable SkillGrps
    {
        get { return _skillGrps != null ? _skillGrps : TablePacketServices.SkillGrps; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _dictElement = new Dictionary<int, DataCell>();
            _filterData = new FilterData(_dictElement);
            _bLink = new BLink(this);
            WireAuxiliaryGameUIPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireAuxiliaryGameUIPacketServices();
    }

    private void WireAuxiliaryGameUIPacketServices()
    {
        AuxiliaryGameUIPacketServices.Bind(buffer: this);
        // EnterWorld may have stashed AbnormalStatusUpdate before this tray existed.
        ApplyServices.Status?.FlushPendingSelfAbnormals();
    }

    protected override void LoadAssets()
    {
        _barSlotTemplate = LoadAsset("Data/UI/_Elements/Template/SlotBuffer");
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Buffer/SkillTray");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        _content = GetElementById("content");
        
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        CreateItemsPanel1();

        yield return new WaitForEndOfFrame();

        _content.style.opacity = 0;
        MovePanelToChatPosition(_defaultLeftOffset);
    }




  



   








  


    // Add these events after the class declaration
    public event Action<int, float> OnTimeLeftUpdated
    {
        add { _bLink.OnTimeLeftUpdated += value; }
        remove { _bLink.OnTimeLeftUpdated -= value; }
    }


    private List<Action<int, float>> _allCurrentHandler = new List<Action<int, float>>();


    public event Action<int, float> OnSpecificTimeLeftUpdated
    {
        add { _bLink.OnSpecificTimeLeftUpdated += value; }
        remove { _bLink.OnSpecificTimeLeftUpdated -= value; }
    }






    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
        _dictElement?.Clear();
        _dictElement = null;
        _bLink?.OnDisable();
    }

}
