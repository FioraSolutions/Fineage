#nullable disable
using System.Collections.Generic;

/// <summary>
/// One row in the server select window. Fineage-Backend login has no server-list message yet,
/// so the list is built from the configured endpoints: one world per login server today.
/// When the backend grows a real list, decode it into these rows and nothing else changes.
/// </summary>
public sealed class LobbyServer
{
    public int Id { get; }
    public string Name { get; }
    public HostPort Zone { get; }
    public bool Online { get; }
    public int CharacterCount { get; }

    public LobbyServer(int id, string name, HostPort zone, bool online, int characterCount)
    {
        Id = id;
        Name = name;
        Zone = zone;
        Online = online;
        CharacterCount = characterCount;
    }

    /// <summary>
    /// The single world behind <paramref name="endpoints"/>. It is online by construction:
    /// this is only called after the login server accepted our credentials and sent a roster.
    /// </summary>
    public static IReadOnlyList<LobbyServer> FromEndpoints(FineageEndpoints endpoints, int characterCount)
    {
        string name = endpoints.IsPublic ? "Fineage" : "Fineage (local)";
        return new[] { new LobbyServer(1, name, endpoints.Zone, true, characterCount) };
    }
}
