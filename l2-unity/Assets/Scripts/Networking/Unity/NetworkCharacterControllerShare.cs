using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class NetworkCharacterControllerShare : MonoBehaviour
{
    [Inject] World _world;

    private World GameWorld => NetworkingPacketServices.GameWorld;

    private PlayerController LocalPlayer => NetworkingPacketServices.Player;

    private CharacterController _characterController;
    private NetworkTransformShare _networkTransformShare;
    [SerializeField] private int _sharingLoopDelayMs = 100;
    [SerializeField] private Vector3 _lastDirection;
    [SerializeField] private long _lastSharingTimestamp = 0;

    private static NetworkCharacterControllerShare _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static NetworkCharacterControllerShare Instance { get { return _instance; } }







}
