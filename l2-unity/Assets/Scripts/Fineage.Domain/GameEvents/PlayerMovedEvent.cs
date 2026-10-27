using UnityEngine;

/// <summary>Raised when a domain move is applied to the live world.</summary>
public readonly struct PlayerMovedEvent
{
    public int ObjectId { get; }
    public Vector3 Destination { get; }
    public Vector3 Origin { get; }

    public PlayerMovedEvent(int objectId, Vector3 destination, Vector3 origin)
    {
        ObjectId = objectId;
        Destination = destination;
        Origin = origin;
    }
}
