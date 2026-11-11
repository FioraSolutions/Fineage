using System;

public static partial class WorldApplyPacketServices
{
    private static PlayerController _player;
    private static EventBus _bus;
    private static World _gameWorld;
    private static GameManager _manager;
    private static IWorldDomainSink _domain;
    private static WorldPacketApply _worldApply;


    public static PlayerController Player =>
        _player != null ? _player : NetworkingPacketServices.Player;

    public static EventBus Bus =>
        _bus != null ? _bus : GamePacketServices.Bus;

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : NetworkingPacketServices.GameWorld;

    public static GameManager Manager =>
        _manager != null ? _manager : NetworkingPacketServices.Manager;

    public static IWorldDomainSink Domain =>
        _domain != null ? _domain : NetworkingPacketServices.Domain;

    public static WorldPacketApply WorldApply =>
        _worldApply != null ? _worldApply : NetworkingPacketServices.WorldApply;




}
