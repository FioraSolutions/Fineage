using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


/// <summary>
/// Literal IPs pass through. Hostnames are resolved, IPv4 first, so a tunnel DNS name can be dialed.
/// </summary>
public static class EndpointAddress
{
    public static IPAddress Resolve(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentException("Server host is empty.");

        host = host.Trim();
        IPAddress parsed;
        if (IPAddress.TryParse(host, out parsed))
            return parsed;

        IPAddress[] addresses;
        try
        {
            addresses = Dns.GetHostAddresses(host);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Could not resolve server host '" + host + "'.", ex);
        }

        IPAddress any = null;
        for (int i = 0; i < addresses.Length; i++)
        {
            if (any == null)
                any = addresses[i];
            if (addresses[i].AddressFamily == AddressFamily.InterNetwork)
                return addresses[i];
        }
        if (any != null)
            return any;
        throw new InvalidOperationException("Could not resolve server host '" + host + "'.");
    }
}
