using System;

/// <summary>
/// Pet delete wire packet handler entry (#142 batch slice 138).
/// </summary>
public static class PetDeleteIncomingPacketServices
{
    public static World GameWorld => WorldIncomingPacketServices.GameWorld;

    public static void ApplyPetDelete(DeleteObjectDto dto)
    {
        SpawnFrameBudget.Cancel(dto.ObjectId);
        if (GameWorld != null)
            GameWorld.DeleteObject(dto.ObjectId);
    }
}
