using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetZoneDispatch
{
    public NetZoneDispatch(NetSession session, Func<Task> enterWorldAsync)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _enterWorldAsync = enterWorldAsync ?? throw new ArgumentNullException(nameof(enterWorldAsync));
    }
}
