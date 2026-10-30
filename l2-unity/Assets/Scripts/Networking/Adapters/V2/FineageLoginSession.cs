using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Headless Fineage-Backend login: Hello, Auth, ListCharacters, SelectCharacter, Handoff.
/// Used when <c>-fineageChar</c> is set. The lobby UI uses <see cref="FineageLobbySession"/> instead.
/// The handoff token is the zone Auth password. The account password never goes to the zone.
/// </summary>
public static class FineageLoginSession
{
    public static async Task<ZoneV2Handoff> LoginAndSelectAsync(
        string host,
        int port,
        string account,
        string password,
        string characterName,
        Action<string> log,
        CancellationToken ct)
    {
        IPAddress ip = EndpointAddress.Resolve(host);
        TcpClient tcp = new TcpClient(ip.AddressFamily);
        try
        {
            await tcp.ConnectAsync(ip, port).ConfigureAwait(false);
            if (log != null)
                log("[FineageLocal] login tcp connected " + host + ":" + port);

            NetworkStream stream = tcp.GetStream();
            await ZoneV2FrameIO.SendAsync(
                stream,
                ZoneV2MsgType.Hello,
                ZoneV2Messages.EncodeHello(1, "unity"),
                ct).ConfigureAwait(false);

            await FineageLobbySession.ReadExpectedAsync(stream, ZoneV2MsgType.HelloAck, ct).ConfigureAwait(false);

            await ZoneV2FrameIO.SendAsync(
                stream,
                ZoneV2MsgType.Auth,
                ZoneV2Messages.EncodeAuth(account, password),
                ct).ConfigureAwait(false);

            ZoneV2Frame loginOk = await FineageLobbySession.ReadExpectedAsync(stream, ZoneV2MsgType.LoginOk, ct).ConfigureAwait(false);
            string canonical;
            if (!ZoneV2Messages.TryDecodeLoginOk(loginOk.Body, out canonical) || string.IsNullOrEmpty(canonical))
                canonical = account;
            if (log != null)
                log("[FineageLocal] login ok account=" + canonical);

            await ZoneV2FrameIO.SendAsync(
                stream,
                ZoneV2MsgType.ListCharacters,
                Array.Empty<byte>(),
                ct).ConfigureAwait(false);

            ZoneV2Frame listFrame = await FineageLobbySession.ReadExpectedAsync(stream, ZoneV2MsgType.CharacterList, ct).ConfigureAwait(false);
            ZoneV2CharacterSummary[] characters;
            if (!ZoneV2Messages.TryDecodeCharacterList(listFrame.Body, out characters))
                throw new InvalidDataException("Character list was not readable.");

            ZoneV2CharacterSummary selected;
            if (!TryPickCharacter(characters, characterName, out selected))
            {
                if (characters == null || characters.Length == 0)
                    throw new FineageEmptyRosterException();
                throw new InvalidOperationException(CharacterPickError(characters, characterName));
            }

            if (selected.DeleteSecondsLeft > 0)
                throw new InvalidOperationException("That character is pending deletion.");

            if (log != null)
                log("[FineageLocal] select character=" + selected.Name + " id=" + selected.CharacterId);

            await ZoneV2FrameIO.SendAsync(
                stream,
                ZoneV2MsgType.SelectCharacter,
                ZoneV2Messages.EncodeSelectCharacter(selected.CharacterId, selected.Name, -1),
                ct).ConfigureAwait(false);

            ZoneV2Frame handoffFrame = await FineageLobbySession.ReadExpectedAsync(stream, ZoneV2MsgType.Handoff, ct).ConfigureAwait(false);
            ZoneV2Handoff handoff;
            if (!ZoneV2Messages.TryDecodeHandoff(handoffFrame.Body, out handoff))
                throw new InvalidDataException("Handoff ticket was not readable.");

            if (string.IsNullOrWhiteSpace(handoff.ZoneHost))
                handoff.ZoneHost = FineageWireConfig.ZoneHost;
            if (handoff.ZonePort <= 0)
                handoff.ZonePort = FineageWireConfig.ZonePort;
            if (string.IsNullOrEmpty(handoff.Account))
                handoff.Account = canonical;

            string prefix = handoff.Token != null && handoff.Token.StartsWith("fa1.", StringComparison.Ordinal)
                ? "fa1."
                : "missing";
            int tokenLength = handoff.Token != null ? handoff.Token.Length : 0;
            if (log != null)
            {
                log("[FineageLocal] handoff character=" + handoff.CharacterName
                    + " zone=" + handoff.ZoneHost + ":" + handoff.ZonePort
                    + " tokenPrefix=" + prefix
                    + " tokenLength=" + tokenLength);
            }

            return handoff;
        }
        finally
        {
            tcp.Close();
        }
    }

    static bool TryPickCharacter(ZoneV2CharacterSummary[] characters, string characterName, out ZoneV2CharacterSummary selected)
    {
        selected = default;
        if (characters == null || characters.Length == 0)
            return false;

        if (!string.IsNullOrWhiteSpace(characterName))
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (string.Equals(characters[i].Name, characterName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    selected = characters[i];
                    return true;
                }
            }

            return false;
        }

        if (characters.Length == 1)
        {
            selected = characters[0];
            return true;
        }

        return false;
    }

    static string CharacterPickError(ZoneV2CharacterSummary[] characters, string characterName)
    {
        if (characters == null || characters.Length == 0)
            return "No characters on this account.";

        if (!string.IsNullOrWhiteSpace(characterName))
            return "Character '" + characterName.Trim() + "' was not found.";

        string names = "";
        for (int i = 0; i < characters.Length; i++)
        {
            if (i > 0)
                names += ", ";
            names += characters[i].Name;
        }

        return "Multiple characters (" + names + "). Pass -fineageChar <name>.";
    }
}

/// <summary>
/// Login succeeded and the account has no characters. The lobby UI must open CharSelect.
/// </summary>
public sealed class FineageEmptyRosterException : InvalidOperationException
{
    public const string Text = "No characters on this account.";

    public FineageEmptyRosterException() : base(Text)
    {
    }
}
