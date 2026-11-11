using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class NpcDecoService
{
    readonly NpcDecoCatalog _catalog;
    readonly Dictionary<int, List<BaseEffect>> _byEntityId = new Dictionary<int, List<BaseEffect>>();
    readonly HashSet<string> _missingLogged = new HashSet<string>();
    readonly List<NpcDecoPiece> _loadBuffer = new List<NpcDecoPiece>(4);
    readonly EffectParticle _hostSettings;


    /// <summary>
    /// NPC deco is the known EnterWorld SIGSEGV source in Mac Standalone (Mono GC during spawn).
    /// Off outside the Editor until fixed; pass -npcdeco to force on.
    /// </summary>
    public static readonly bool Enabled =
#if UNITY_EDITOR
        true;
#else
        Array.IndexOf(Environment.GetCommandLineArgs(), "-npcdeco") >= 0;
#endif






}
