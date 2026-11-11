using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


public sealed class PlaytestEndpointInput
{
    public string[] CommandLine;
    public IDictionary<string, string> Environment;
    public string PersistentJson;
    public string SidecarJson;
    public string StreamingJson;
    public string BakedJson;
}
