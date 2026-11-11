using System;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class FineageZoneTransport : IZoneTransport
{
    public void BeginSession(string account, string password)
    {
        _disconnectNotified = false;
        Session.BeginLobby(account, password, registerIfMissing: true);
    }

    public void SendMove(Vector3 fromUnity, Vector3 toUnity)
    {
        Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(toUnity);
        Session.SendMove(
            Mathf.RoundToInt(l2.x),
            Mathf.RoundToInt(l2.y),
            Mathf.RoundToInt(l2.z));
    }
}
