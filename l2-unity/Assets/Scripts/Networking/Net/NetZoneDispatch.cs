using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetZoneDispatch
{
    readonly NetSession _session;
    readonly Func<Task> _enterWorldAsync;

}
