using System;
using UnityEngine;

/// <summary>Apply the party member list for minimal roster feedback (#142 locator queue slice 258).</summary>
public static class PartyMemberListApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyPartyMemberList(ZoneV2PartyMemberList list) =>
        Queue(() =>
        {
            PartyPacketServices.ApplyMemberList(list);
            string summary = ZoneV2DomainMap.FormatPartyMemberList(list);
            Debug.Log("[Party] " + summary);
            ChatTypeData data = ChatTypes.GetById(0);
            if (data != null)
            {
                CreatureMessage message = new CreatureMessage("Party", summary, data);
                CreatureSayApplyIncomingPacketServices.ApplyCreatureSay(message);
            }
        });
}
