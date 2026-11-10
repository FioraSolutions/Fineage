using System;

public class AutoAttackStopDto : IWireDto {
    public int EntityId { get; private set; }

    /// <summary>Net <c>AutoAttackStop</c>: the actor leaves attack stance (no combat for 15 s, or the mob lost its target).</summary>
    public static AutoAttackStopDto FromNet(int entityId) =>
        new AutoAttackStopDto { EntityId = entityId };

    public void ReadFrom(PacketReader reader) {
        try {
            EntityId = reader.ReadI();
        } catch (Exception) {
            // Debug.LogError(e);
        }
    }
}
