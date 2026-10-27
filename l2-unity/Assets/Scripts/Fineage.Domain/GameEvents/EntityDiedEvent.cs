/// <summary>Raised when a die packet is applied to a world entity.</summary>
public readonly struct EntityDiedEvent
{
    public int ObjectId { get; }

    public EntityDiedEvent(int objectId)
    {
        ObjectId = objectId;
    }
}
