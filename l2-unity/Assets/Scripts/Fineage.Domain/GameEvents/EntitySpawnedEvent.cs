/// <summary>Raised after the live world finishes spawning a player entity.</summary>
public readonly struct EntitySpawnedEvent
{
    public int ObjectId { get; }

    public EntitySpawnedEvent(int objectId)
    {
        ObjectId = objectId;
    }
}
