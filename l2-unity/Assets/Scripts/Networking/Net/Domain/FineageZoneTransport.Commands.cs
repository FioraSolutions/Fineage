using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class FineageZoneTransport
{
    public bool TrySendGameCommand(INetworkCommand command)
    {
        if (TrySendWorldCommands(command))
            return true;
        if (TrySendInteractionCommands(command))
            return true;
        if (TrySendEconomySocialCommands(command))
            return true;
        return false;
    }
}
