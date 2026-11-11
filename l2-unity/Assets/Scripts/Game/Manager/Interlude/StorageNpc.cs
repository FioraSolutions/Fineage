using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class StorageNpc
{
    private static StorageNpc instance;
    public static Dictionary<int , NpcInfoDto> npcs;
    public static Dictionary<int, UserInfoDto> users;
    public static Dictionary<int, CharInfoDto> chars;
    private object _sync = new object();




    /// <summary>
    /// Drop one object from the knownlist cache. Without this the dictionaries only ever grow:
    /// <see cref="AddNpcInfo"/> is called for every NpcInfo the zone sends, but nothing removed the
    /// entry when the object left the knownlist, so every later <c>FlushKnownlist</c> walked
    /// thousands of stale rows and re-queued spawns for objects that no longer exist.
    /// </summary>


    /// <summary>
    /// Wipe the whole knownlist cache. Used when a zone session ends, so a reconnect does not inherit
    /// the previous session's objects.
    /// </summary>




}
