using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using VContainer;

public sealed partial class EnterEventTable
{
    public const string EffectCommentPrefix = "EnterEventgrp ";
    public struct Row
    {
        public int IsRise;
        public int SpawnType;
        public string EffectClass;
        public string AnimName;
        public string SoundName;
        public float SoundVolume;
        public float SoundRadius;
    }

    static EnterEventTable _instance;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static EnterEventTable Instance
    {
        get
        {
            if (_instance == null)
                _instance = new EnterEventTable();
            return _instance;
        }
    }

    const string RelativePath = "Data/Meta/EnterEventgrp.txt";

    readonly Dictionary<int, Row> _rows = new Dictionary<int, Row>();

    public int Count => _rows.Count;




}
