using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{














    /// <summary>
    /// <c>UserInfo</c> wire → <see cref="NetUserInfo"/> for <see cref="NetGearSession.NoteUserInfo"/>.
    ///
    /// <para>
    /// Every flag is set from the message rather than guessed: the zone server always
    /// populates vitals, stats and clan (0 meaning "none"), and an empty paperdoll is a
    /// legitimate answer for an unequipped character, not a missing tail.
    /// </para>
    /// </summary>



}
