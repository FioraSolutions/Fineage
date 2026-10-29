#nullable disable
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// The one place the Fineage client opens a TCP connection (login lobby and zone).
/// Resolves DNS with IPv4 preferred, applies a connect timeout, honours cancellation,
/// and turns every failure into a single readable <see cref="FineageConnectException"/>.
/// </summary>
public static class FineageTcp
{
    public static readonly TimeSpan DefaultConnectTimeout = TimeSpan.FromSeconds(10);

    public static async Task<TcpClient> ConnectAsync(string role, HostPort endpoint, CancellationToken ct)
    {
        if (!endpoint.IsValid)
            throw new FineageConnectException(role, endpoint, "endpoint is not configured");

        IPAddress address = await ResolveAsync(role, endpoint).ConfigureAwait(false);
        var tcp = new TcpClient(address.AddressFamily) { NoDelay = true };
        try
        {
            Task connect = tcp.ConnectAsync(address, endpoint.Port);
            Task timeout = Task.Delay(DefaultConnectTimeout, ct);
            if (await Task.WhenAny(connect, timeout).ConfigureAwait(false) != connect)
            {
                ct.ThrowIfCancellationRequested();
                throw new FineageConnectException(role, endpoint,
                    "timed out after " + (int)DefaultConnectTimeout.TotalSeconds + "s");
            }

            await connect.ConfigureAwait(false);
            return tcp;
        }
        catch (OperationCanceledException)
        {
            tcp.Dispose();
            throw;
        }
        catch (FineageConnectException)
        {
            tcp.Dispose();
            throw;
        }
        catch (Exception ex)
        {
            tcp.Dispose();
            throw new FineageConnectException(role, endpoint, ex.Message, ex);
        }
    }

    static async Task<IPAddress> ResolveAsync(string role, HostPort endpoint)
    {
        IPAddress literal;
        if (IPAddress.TryParse(endpoint.Host.Trim(), out literal))
            return literal;

        IPAddress[] addresses;
        try
        {
            addresses = await Dns.GetHostAddressesAsync(endpoint.Host.Trim()).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new FineageConnectException(role, endpoint, "DNS lookup failed: " + ex.Message, ex);
        }

        IPAddress fallback = null;
        foreach (IPAddress a in addresses)
        {
            if (a.AddressFamily == AddressFamily.InterNetwork)
                return a;
            if (fallback == null)
                fallback = a;
        }

        if (fallback == null)
            throw new FineageConnectException(role, endpoint, "DNS returned no addresses");
        return fallback;
    }
}

/// <summary>A connect failure with the role and endpoint in the message, ready to show to the player.</summary>
public sealed class FineageConnectException : Exception
{
    public HostPort Endpoint { get; }

    public FineageConnectException(string role, HostPort endpoint, string reason, Exception inner = null)
        : base("Cannot reach " + role + " server " + endpoint + ": " + reason, inner)
    {
        Endpoint = endpoint;
    }
}
