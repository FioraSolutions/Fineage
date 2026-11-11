using System;
using System.Collections.Generic;

public static partial class NetAutoShot
{
    /// <summary>Classic <c>etcitem_type</c> for both soulshots and spiritshots.</summary>
    const string ShotEtcType = "soul_shot";

    static readonly object Gate = new object();
    static readonly HashSet<int> Enabled = new HashSet<int>();


    public static IReadOnlyCollection<int> EnabledItems
    {
        get
        {
            lock (Gate)
                return new List<int>(Enabled);
        }
    }


    /// <summary>Applies the zone's confirmation. Returns true when the state actually changed.</summary>

    /// <summary>
    /// A shot is an etc item whose <c>etcitem_type</c> is <c>soul_shot</c>. The client data file only
    /// carries that one token: spiritshot and blessed spiritshot rows share it, and the zone picks the
    /// charge kind from its own template (<c>isSoulshot</c> / <c>isSpiritshot</c>).
    /// </summary>

    /// <summary>Click / key entry point: flips the shot and sends the request. Returns the new intent.</summary>
}
