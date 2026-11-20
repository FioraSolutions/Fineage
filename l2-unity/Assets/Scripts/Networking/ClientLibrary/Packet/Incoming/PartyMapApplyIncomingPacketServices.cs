using Fineage.Net.Contract;

public static class PartyMapApplyIncomingPacketServices
{
    public static void ApplyPartyMemberPositions(PartyMemberPositionNetMsg msg)
    {
        if (msg?.Members == null)
            return;

        foreach (PartyMemberPositionEntryNet row in msg.Members)
        {
            if (row == null)
                continue;
            MapPartyPositions.Apply(row.ObjectId, row.X, row.Y, row.Z);
        }
    }
}