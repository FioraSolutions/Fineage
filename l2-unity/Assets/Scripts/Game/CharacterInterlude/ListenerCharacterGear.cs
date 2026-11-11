using System;
using UnityEngine;
using VContainer;

public partial class ListenerCharacterGear : MonoBehaviour
{

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static ListenerCharacterGear Instance { get; private set; }



    private EventBus Bus
    {
        get { return _bus != null ? _bus : GamePacketServices.Bus; }
    }

    private World GameWorld
    {
        get { return _world != null ? _world : GamePacketServices.GameWorld; }
    }






}
