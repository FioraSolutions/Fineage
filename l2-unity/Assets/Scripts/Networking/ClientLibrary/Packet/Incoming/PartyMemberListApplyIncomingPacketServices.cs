using System;
using UnityEngine;

/// <summary>Apply Zone V2 <see cref="ZoneV2MsgType.PartyMemberList"/> (1029) for minimal roster feedback.</summary>
public static class PartyMemberListApplyIncomingPacketServices
{
    public static void Queue(Action action) => UiApplyIncomingPacketServices.Queue(action);

    public static void ApplyPartyMemberList(ZoneV2PartyMemberList list) =>
        Queue(() =>
        {
            PartyPacketServices.ApplyMemberList(list);
            string summary = ZoneV2Messages.FormatPartyMemberList(list);
            Debug.Log("[Party] " + summary);
            ChatTypeData data = ChatTypes.GetById(0);
            if (data != null)
            {
                CreatureMessage message = new CreatureMessage("Party", summary, data);
                CreatureSayApplyIncomingPacketServices.ApplyCreatureSay(message);
            }
        });
}
