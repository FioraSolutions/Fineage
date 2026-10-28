using UnityEngine;

/// <summary>Apply Zone V2 <see cref="ZoneV2MsgType.PartyJoinResult"/> (1028) to inviter-side feedback.</summary>
public static class PartyJoinResultApplyIncomingPacketServices
{
    public static void ApplyPartyJoinResult(int response)
    {
        string text = response == 1
            ? "Your party invitation was accepted."
            : "Your party invitation was declined.";
        Debug.Log("[Party] " + text);
        ChatTypeData data = ChatTypes.GetById(0);
        if (data == null)
            return;
        CreatureMessage message = new CreatureMessage("Party", text, data);
        CreatureSayApplyIncomingPacketServices.ApplyCreatureSay(message);
    }
}
