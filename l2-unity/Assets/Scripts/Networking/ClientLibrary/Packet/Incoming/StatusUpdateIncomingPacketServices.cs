/// <summary>
/// Status update wire packet handler entry (#142 batch slice 143).
/// </summary>
public static class StatusUpdateIncomingPacketServices
{
    public static World GameWorld => WorldIncomingPacketServices.GameWorld;

    public static void ApplyStatusUpdate(int objectId, System.Collections.Generic.List<StatusUpdate.Attribute> attributes)
    {
        if (GameWorld != null)
            GameWorld.StatusUpdate(objectId, attributes);
    }
}
