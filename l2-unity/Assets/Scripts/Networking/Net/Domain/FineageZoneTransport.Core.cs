using System;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class FineageZoneTransport : IZoneTransport
{
    public void BeginSession(string account, string password)
    {
        NetSession session = Session;
        if (session == null)
        {
            Debug.LogError(
                "[Net] BeginSession: NetSession not bound — AppLifetimeScope must CreateForCompositionRoot first");
            return;
        }

        _disconnectNotified = false;
        session.BeginLobby(account, password, registerIfMissing: true);
    }

    public void SendMove(Vector3 fromUnity, Vector3 toUnity)
    {
        NetSession session = Session;
        if (session == null)
            return;

        Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(toUnity);
        session.SendMove(
            Mathf.RoundToInt(l2.x),
            Mathf.RoundToInt(l2.y),
            Mathf.RoundToInt(l2.z));
    }
}
