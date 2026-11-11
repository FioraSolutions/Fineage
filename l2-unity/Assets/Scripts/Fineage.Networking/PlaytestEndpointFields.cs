using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


public struct PlaytestEndpointFields
{
    public string Host;
    public string LoginHost;
    public string GameHost;
    public int LoginPort;
    public int GamePort;
    public bool HostSet;
    public bool LoginHostSet;
    public bool GameHostSet;
    public bool LoginPortSet;
    public bool GamePortSet;
}
