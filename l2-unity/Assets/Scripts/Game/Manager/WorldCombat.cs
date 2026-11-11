using System;

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static StatusUpdate;
using static UnityEngine.EventSystems.EventTrigger;
using VContainer;

public partial class WorldCombat : MonoBehaviour
{
    [SerializeField] private GameObject _impactParticle;

    private static WorldCombat _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static WorldCombat Instance { get { return _instance; } }











}
