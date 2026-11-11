using Fineage.Net.Contract;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;

public sealed partial class NetSession
{
    void HandleZone(PacketEnvelope envelope) => _zoneDispatch.Handle(envelope);
}
