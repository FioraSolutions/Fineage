using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class DeadManager : MonoBehaviour, IDead
{
    public event Action<int> OnReadyToRemove;

    private static IDead _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static IDead Instance { get { return (_instance as UnityEngine.Object) != null ? _instance : null; } }

    [Inject] private L2ActorFade _actorFade;

    private L2ActorFade ActorFade
    {
        get { return _actorFade != null ? _actorFade : ManagerInteractionPacketServices.ActorFade; }
    }

    private Dictionary<int, DeadData> _dict;
    private List<int> _remove = new List<int>();








}
