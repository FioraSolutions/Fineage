using Fineage.Net.Contract;
using UnityEngine;

/// <summary>Applies private-store broadcasts to world nameplates.</summary>
public static class NetPrivateStoreApply
{
    public static void ApplyPrivateStoreInfo(PrivateStoreInfoNetMsg msg)
    {
        if (msg == null)
            return;

        NetPlayerStoreRegistry.Note(msg);

        QueueApplyIncomingPacketServices.Queue(() =>
        {
            World world = WorldApplyIncomingPacketServices.GameWorld;
            if (world == null)
            {
                FineageLog.Error(FineageLogCategory.Net, "PrivateStoreInfo: GameWorld is null.");
                return;
            }

            Entity entity = world.GetEntityNoLockSync(msg.ObjectId);
            if (entity == null || entity.Identity == null)
            {
                FineageLog.Warn(
                    FineageLogCategory.Net,
                    $"PrivateStoreInfo: entity objectId={msg.ObjectId} not found.");
                return;
            }

            if (!PrivateStoreProtocolClient.IsOpen((byte)msg.StoreType))
            {
                entity.Identity.Title = "";
                return;
            }

            entity.Identity.Title = msg.Message ?? "";
            string color = PrivateStoreTitleStyles.ColorForStoreType(msg.StoreType);
            if (!string.IsNullOrEmpty(color))
                entity.Identity.TitleColor = color;
        });
    }
}