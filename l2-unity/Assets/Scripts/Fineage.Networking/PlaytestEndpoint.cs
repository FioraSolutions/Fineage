using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


/// <summary>
/// Resolved shared-playtest login and game endpoint.
/// Defaults are the Cloudflare tunnel hostnames. An empty override does not clear them.
/// <see cref="PublicHost"/> is the landing name (HTTP). Login and game are separate TCP names.
/// </summary>
public sealed partial class PlaytestEndpoint
{
    public const string DefaultPublicHost = "fineage.evpoint.by";
    public const string DefaultLoginHost = "login.fineage.evpoint.by";
    public const string DefaultGameHost = "game.fineage.evpoint.by";
    public const int DefaultLoginPort = 2106;
    public const int DefaultGamePort = 7777;

    public string PublicHost { get; private set; }
    public string LoginHost { get; private set; }
    public string GameHost { get; private set; }
    public int LoginPort { get; private set; }
    public int GamePort { get; private set; }
    public bool OverrideLogin { get; private set; }
    public bool OverrideGameHost { get; private set; }
    public bool OverrideGamePort { get; private set; }

    /// <summary>
    /// Layers are applied lowest priority first, on top of the Cloudflare defaults.
    /// An empty field does not clear a lower layer. <c>host</c> is the landing name and
    /// does not replace login or game hosts.
    /// </summary>
}
