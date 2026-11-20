using System.Collections.Generic;
using Fineage.Net.Contract;

/// <summary>Shared merge helpers for coalesced zone packets (partial L2 attribute rows, etc.).</summary>
public static class NetPacketCoalesceMerge
{
    public static object Merge(ushort opcode, object accumulated, object incoming)
    {
        if (incoming == null)
            return accumulated;
        if (accumulated == null)
            return incoming;

        return opcode switch
        {
            NetOpcode.StatusUpdate => MergeStatusUpdate((StatusUpdateNetMsg)accumulated, (StatusUpdateNetMsg)incoming),
            NetOpcode.PartyMemberPosition => MergePartyMemberPosition(
                (PartyMemberPositionNetMsg)accumulated,
                (PartyMemberPositionNetMsg)incoming),
            _ => incoming,
        };
    }

    public static StatusUpdateNetMsg MergeStatusUpdate(StatusUpdateNetMsg prior, StatusUpdateNetMsg incoming)
    {
        var map = new Dictionary<int, int>(prior.AttributeIds.Length + incoming.AttributeIds.Length);
        MergeAttributes(map, prior.AttributeIds, prior.Values);
        MergeAttributes(map, incoming.AttributeIds, incoming.Values);

        var ids = new int[map.Count];
        var vals = new int[map.Count];
        int i = 0;
        foreach (KeyValuePair<int, int> row in map)
        {
            ids[i] = row.Key;
            vals[i] = row.Value;
            i++;
        }

        return new StatusUpdateNetMsg
        {
            EntityValue = incoming.EntityValue,
            EntityGeneration = incoming.EntityGeneration,
            AttributeIds = ids,
            Values = vals,
        };
    }

    public static PartyMemberPositionNetMsg MergePartyMemberPosition(
        PartyMemberPositionNetMsg prior,
        PartyMemberPositionNetMsg incoming)
    {
        var byId = new Dictionary<int, PartyMemberPositionEntryNet>();
        if (prior.Members != null)
        {
            foreach (PartyMemberPositionEntryNet row in prior.Members)
                byId[row.ObjectId] = row;
        }

        if (incoming.Members != null)
        {
            foreach (PartyMemberPositionEntryNet row in incoming.Members)
                byId[row.ObjectId] = row;
        }

        var merged = new PartyMemberPositionEntryNet[byId.Count];
        int i = 0;
        foreach (PartyMemberPositionEntryNet row in byId.Values)
            merged[i++] = row;

        return new PartyMemberPositionNetMsg { Members = merged };
    }

    static void MergeAttributes(Dictionary<int, int> map, int[] attributeIds, int[] values)
    {
        if (attributeIds == null || values == null)
            return;

        int count = System.Math.Min(attributeIds.Length, values.Length);
        for (int i = 0; i < count; i++)
            map[attributeIds[i]] = values[i];
    }
}