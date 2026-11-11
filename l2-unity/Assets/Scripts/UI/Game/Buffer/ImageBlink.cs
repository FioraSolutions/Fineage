using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public partial class BLink
{
    public VisualElement _element;
    public float _blinkInterval = 0.1f;
    public float _intervalUse = 30f;
    private bool _isBlinking = true;
    private Dictionary<int , DataCell> _dict;
    private List<int> _filter = new List<int>();
    private float opacityHide = 0.4f;
    private float opacityShow = 1.0f;
    private float durationOpacity = 0.26f;
    public event Action<int, float> OnTimeLeftUpdated;
    public event Action<int, float> OnSpecificTimeLeftUpdated;
    private Dictionary<int, float> _lastUpdateTime = new Dictionary<int, float>();
    private float _updateInterval = 1.0f; // 1 second
    private HashSet<int> _trackedSkillIds = new HashSet<int>();
}
