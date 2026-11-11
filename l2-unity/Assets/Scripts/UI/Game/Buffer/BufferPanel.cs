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

    [Inject] ISkillgrpLookup _skillGrps;

    private ISkillgrpLookup SkillGrps
    {
        get { return _skillGrps != null ? _skillGrps : TablePacketServices.SkillGrps; }
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







}
