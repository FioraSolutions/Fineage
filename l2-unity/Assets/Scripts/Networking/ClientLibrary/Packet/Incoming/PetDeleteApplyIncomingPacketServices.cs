using System;

/// <summary>
/// Pet delete wire packet handler entry (#142 batch slice 138).
/// Apply facade (#142 batch slice 155). Re-homes from <see cref="PetDeleteIncomingPacketServices"/>.
/// </summary>
public static class PetDeleteApplyIncomingPacketServices
{
    public static World GameWorld => WorldIncomingPacketServices.GameWorld;

    public static void ApplyPetDelete(DeleteObjectDto dto)
    {
        SpawnFrameBudget.Cancel(dto.ObjectId);
        if (GameWorld != null)
            GameWorld.DeleteObject(dto.ObjectId);
    }
}
