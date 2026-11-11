using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class WorldPacketApply
{
    public void FlushKnownlist()
    {
        StorageNpc storage = StorageNpc.getInstance();
        NpcInfoDto[] npcs = storage.CopyNpcs();
        CharInfoDto[] chars = storage.CopyChars();
        // Debug.Log("[WorldSpawn] FlushKnownlist npcs=" + npcs.Length + " chars=" + chars.Length);

        // Spawning every known object in one go defeats SpawnFrameBudget: each NPC over the cap
        // re-queues itself as its own deferred action, so a stale knownlist turns one flush into
        // tens of thousands of queue entries that all replay on the following frame. Apply what the
        // budget allows and hand the remainder to the next frame as a single continuation.
        int npcIndex = 0;
        for (; npcIndex < npcs.Length; npcIndex++)
        {
            if (!SpawnFrameBudget.HasBudget())
                break;
            UpdateNpc(npcs[npcIndex]);
        }

        int charIndex = 0;
        for (; charIndex < chars.Length; charIndex++)
            UpdateUser(chars[charIndex]);

        if (npcIndex < npcs.Length)
        {
            NpcInfoDto[] remaining = new NpcInfoDto[npcs.Length - npcIndex];
            System.Array.Copy(npcs, npcIndex, remaining, 0, remaining.Length);
            QueueWorldNextFrame(apply => apply.UpdateNpcBatch(remaining));
        }
    }

    public void UpdateNpcBatch(NpcInfoDto[] batch)
    {
        if (batch == null || batch.Length == 0)
            return;

        int index = 0;
        for (; index < batch.Length; index++)
        {
            if (!SpawnFrameBudget.HasBudget())
                break;
            UpdateNpc(batch[index]);
        }

        if (index < batch.Length)
        {
            NpcInfoDto[] remaining = new NpcInfoDto[batch.Length - index];
            System.Array.Copy(batch, index, remaining, 0, remaining.Length);
            QueueWorldNextFrame(apply => apply.UpdateNpcBatch(remaining));
        }
    }
}
