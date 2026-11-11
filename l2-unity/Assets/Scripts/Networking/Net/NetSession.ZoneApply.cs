using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{






    /// <summary>
    /// Zone <see cref="EntitySpawnMsg.CurHp"/> is authoritative at enter-view (select StatusUpdate must not look like damage).
    /// Decode already fills MaxHp when the optional CurHp tail is absent on old peers.
    /// </summary>

    /// <summary>
    /// Single EntitySpawn → NpcInfoDto mapper (#930): position, heading, Attackable→EntityType, speeds, HP.
    /// </summary>




}
