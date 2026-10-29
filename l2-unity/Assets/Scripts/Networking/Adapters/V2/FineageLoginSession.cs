using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// What to do after LoginOk + CharacterList. An empty roster is CharSelect, never a login error.
/// </summary>
public enum FineageLoginPlan
{
    /// <summary>A character was chosen. SelectCharacter + Handoff may run.</summary>
    Handoff = 0,
    /// <summary>Show the church CharSelect. Empty rosters use Create. Do not hand off yet.</summary>
    CharacterSelect = 1,
    Failed = 2
}

/// <summary>
/// Result of <see cref="FineageLoginSession.LoginAndSelectAsync"/>.
/// <see cref="HasHandoff"/> is false when the player must use CharSelect (including zero characters).
/// </summary>
public readonly struct FineageLoginSelection
{
    public readonly bool HasHandoff;
    public readonly ZoneV2Handoff Handoff;
    public readonly string Account;
    public readonly ZoneV2CharacterSummary[] Characters;

    public FineageLoginSelection(bool hasHandoff, ZoneV2Handoff handoff, string account, ZoneV2CharacterSummary[] characters)
    {
        HasHandoff = hasHandoff;
        Handoff = handoff;
        Account = account ?? "";
        Characters = characters ?? Array.Empty<ZoneV2CharacterSummary>();
    }

    public static FineageLoginSelection CharacterSelect(string account, ZoneV2CharacterSummary[] characters)
    {
        return new FineageLoginSelection(false, default, account, characters);
    }
}

/// <summary>
/// Fineage-Backend login TCP: Hello, Auth, ListCharacters, then SelectCharacter + Handoff
/// only when a character is already chosen. An empty list returns to CharSelect / Create.
/// The handoff token is the zone Auth password. The account password never goes to the zone.
/// </summary>
public static class FineageLoginSession
{
    public static bool IsPublicLoginHost(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
            return false;
        string trimmed = host.Trim();
        if (trimmed == "127.0.0.1" || trimmed == "::1" || trimmed == "0.0.0.0")
            return false;
        if (trimmed.StartsWith("127.", StringComparison.Ordinal))
            return false;
        if (string.Equals(trimmed, "localhost", StringComparison.OrdinalIgnoreCase))
            return false;
        return true;
    }

    /// <summary>
    /// Empty roster always plans CharSelect (Create). It does not produce an error.
    /// <paramref name="playerPicksInUi"/> keeps a non-empty roster on CharSelect until the player picks.
    /// </summary>
    public static FineageLoginPlan Plan(
        ZoneV2CharacterSummary[] characters,
        string characterName,
        bool playerPicksInUi,
        out ZoneV2CharacterSummary selected,
        out string error)
    {
        selected = default;
        error = null;
        if (characters == null || characters.Length == 0)
            return FineageLoginPlan.CharacterSelect;

        if (playerPicksInUi)
            return FineageLoginPlan.CharacterSelect;

        if (!TryPickCharacter(characters, characterName, out selected))
        {
            error = CharacterPickError(characters, characterName);
            return FineageLoginPlan.Failed;
        }

        return FineageLoginPlan.Handoff;
    }

    public static async Task<FineageLoginSelection> LoginAndSelectAsync(
        string host,
        int port,
        string account,
        string password,
        string characterName,
        Action<string> log,
        CancellationToken ct)
    {
        TcpClient tcp = new TcpClient();
        try
        {
            await tcp.ConnectAsync(host, port).ConfigureAwait(false);
            if (log != null)
                log("[FineageLocal] login tcp connected " + host + ":" + port);

            NetworkStream stream = tcp.GetStream();
            await ZoneV2FrameIO.SendAsync(
                stream,
                ZoneV2MsgType.Hello,
                ZoneV2Messages.EncodeHello(1, "unity"),
                ct).ConfigureAwait(false);

            await ReadExpectedAsync(stream, ZoneV2MsgType.HelloAck, ct).ConfigureAwait(false);

            await ZoneV2FrameIO.SendAsync(
                stream,
                ZoneV2MsgType.Auth,
                ZoneV2Messages.EncodeAuth(account, password),
                ct).ConfigureAwait(false);

            ZoneV2Frame loginOk = await ReadExpectedAsync(stream, ZoneV2MsgType.LoginOk, ct).ConfigureAwait(false);
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

            ZoneV2Frame listFrame = await ReadExpectedAsync(stream, ZoneV2MsgType.CharacterList, ct).ConfigureAwait(false);
            ZoneV2CharacterSummary[] characters;
            if (!ZoneV2Messages.TryDecodeCharacterList(listFrame.Body, out characters))
                throw new InvalidDataException("Character list was not readable.");

            ZoneV2CharacterSummary selected;
            string pickError;
            FineageLoginPlan plan = Plan(characters, characterName, false, out selected, out pickError);
            if (plan == FineageLoginPlan.CharacterSelect)
            {
                int count = characters != null ? characters.Length : 0;
                if (log != null)
                    log("[FineageLocal] roster count=" + count + " → CharSelect");
                return FineageLoginSelection.CharacterSelect(canonical, characters);
            }

            if (plan != FineageLoginPlan.Handoff)
                throw new InvalidOperationException(string.IsNullOrEmpty(pickError) ? "Character was not selected." : pickError);

            if (log != null)
                log("[FineageLocal] select character=" + selected.Name + " id=" + selected.CharacterId);

            await ZoneV2FrameIO.SendAsync(
                stream,
                ZoneV2MsgType.SelectCharacter,
                ZoneV2Messages.EncodeSelectCharacter(selected.CharacterId, selected.Name),
                ct).ConfigureAwait(false);

            ZoneV2Frame handoffFrame = await ReadExpectedAsync(stream, ZoneV2MsgType.Handoff, ct).ConfigureAwait(false);
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

            return new FineageLoginSelection(true, handoff, handoff.Account, characters);
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
            return null;

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

    static async Task<ZoneV2Frame> ReadExpectedAsync(NetworkStream stream, ZoneV2MsgType expected, CancellationToken ct)
    {
        ZoneV2Frame? frame = await ZoneV2FrameIO.TryReadAsync(stream, ct).ConfigureAwait(false);
        if (frame == null)
            throw new IOException("Login server closed the connection while waiting for " + expected + ".");

        ZoneV2Frame got = frame.Value;
        if (got.Type == ZoneV2MsgType.Error)
        {
            string code;
            string message;
            ZoneV2Messages.TryDecodeError(got.Body, out code, out message);
            throw new InvalidOperationException(
                (string.IsNullOrEmpty(code) ? "login_error" : code) + ": " + message);
        }

        if (got.Type != expected)
            throw new InvalidOperationException("Unexpected login message " + (ushort)got.Type + " while waiting for " + expected + ".");

        return got;
    }
}
