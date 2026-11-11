using System.Collections.Generic;
using UnityEngine;

public sealed class RequestShortCutRegCommand : INetworkCommand
{
    public int TypeId;
    public int WorldSlot;
    public int Id;
    public int Level;

    public RequestShortCutRegCommand(int typeId, int worldSlot, int id, int level)
    {
        TypeId = typeId;
        WorldSlot = worldSlot;
        Id = id;
        Level = level;
    }
}

public sealed class RequestShortCutDelCommand : INetworkCommand
{
    public int WorldSlot;

    public RequestShortCutDelCommand(int worldSlot)
    {
        WorldSlot = worldSlot;
    }
}

