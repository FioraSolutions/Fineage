using System;

public class AutoAttackStartDto : IWireDto {
    public int EntityId { get; private set; }

    /// <summary>Net <c>AutoAttackStart</c>: the actor enters attack stance (Mobius ChangeWaitType-style stance, not a swing).</summary>
    public static AutoAttackStartDto FromNet(int entityId) =>
        new AutoAttackStartDto { EntityId = entityId };

    public void ReadFrom(PacketReader reader) {
        try {
            EntityId = reader.ReadI();
        } catch (Exception e) {
            // Debug.LogError(e);
        }
    }
}
