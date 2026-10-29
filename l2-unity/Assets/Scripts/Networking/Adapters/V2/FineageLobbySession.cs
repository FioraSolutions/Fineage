using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Long-lived Fineage-Backend login TCP session: auth, then list / select / delete / restore / create.
/// The account password is sent only in the login Auth frame. It is not stored and never goes to the zone.
/// </summary>
public sealed class FineageLobbySession
{
    readonly CancellationToken _ct;
    readonly object _sendLock = new object();
    TcpClient _tcp;
    NetworkStream _stream;
    bool _closed;
    bool _handedOff;

    public Action<ZoneV2CharacterSummary[]> Roster;
    public Action<ZoneV2Handoff> Handoff;
    public Action<string, string> Failed;
    public Action<ZoneV2CharacterCreated> Created;
    public Action<ZoneV2CharacterDeleted> Deleted;
    public Action<ZoneV2CharacterDeleteFailed> DeleteFailed;
    public Action<string> Lost;

    public string Account { get; private set; }

    public FineageLobbySession(CancellationToken ct)
    {
        _ct = ct;
    }

    public async Task ConnectAsync(HostPort login, string account, string password)
    {
        _tcp = await FineageTcp.ConnectAsync("login", login, _ct).ConfigureAwait(false);
        _stream = _tcp.GetStream();

        await ZoneV2FrameIO.SendAsync(
            _stream,
            ZoneV2MsgType.Hello,
            ZoneV2Messages.EncodeHello(1, "unity"),
            _ct).ConfigureAwait(false);
        await ReadExpectedAsync(_stream, ZoneV2MsgType.HelloAck, _ct).ConfigureAwait(false);

        await ZoneV2FrameIO.SendAsync(
            _stream,
            ZoneV2MsgType.Auth,
            ZoneV2Messages.EncodeAuth(account, password),
            _ct).ConfigureAwait(false);

        ZoneV2Frame loginOk = await ReadExpectedAsync(_stream, ZoneV2MsgType.LoginOk, _ct).ConfigureAwait(false);
        string canonical;
        if (!ZoneV2Messages.TryDecodeLoginOk(loginOk.Body, out canonical) || string.IsNullOrEmpty(canonical))
            canonical = account;
        Account = canonical;

        _ = ReadLoop();
        Send(ZoneV2MsgType.ListCharacters, Array.Empty<byte>());
    }

    public void SendSelect(int characterId, string name, int slot)
    {
        Send(ZoneV2MsgType.SelectCharacter, ZoneV2Messages.EncodeSelectCharacter(characterId, name, slot));
    }

    public void SendDelete(int slot, int characterId)
    {
        Send(ZoneV2MsgType.DeleteCharacter, ZoneV2Messages.EncodeDeleteCharacter(slot, characterId));
    }

    public void SendRestore(int slot, int characterId)
    {
        Send(ZoneV2MsgType.RestoreCharacter, ZoneV2Messages.EncodeRestoreCharacter(slot, characterId));
    }

    public void SendCreate(string name, int classId, int race, int sex, int hairStyle, int hairColor, int face)
    {
        Send(
            ZoneV2MsgType.CreateCharacter,
            ZoneV2Messages.EncodeCreateCharacter(name, classId, race, sex, hairStyle, hairColor, face));
    }

    public void Close()
    {
        if (_closed)
            return;
        _closed = true;
        try
        {
            if (_stream != null)
                _stream.Close();
        }
        catch
        {
            /* ignore */
        }

        try
        {
            if (_tcp != null)
                _tcp.Close();
        }
        catch
        {
            /* ignore */
        }

        _stream = null;
        _tcp = null;
    }

    void Send(ZoneV2MsgType type, byte[] body)
    {
        if (_closed || _stream == null)
            return;
        if (body == null)
            body = Array.Empty<byte>();
        int total = ZoneV2FrameIO.HeaderSize + body.Length;
        if (total > ushort.MaxValue)
            return;

        byte[] packet = new byte[total];
        packet[0] = (byte)(total & 0xff);
        packet[1] = (byte)((total >> 8) & 0xff);
        ushort t = (ushort)type;
        packet[2] = (byte)(t & 0xff);
        packet[3] = (byte)((t >> 8) & 0xff);
        if (body.Length > 0)
            Buffer.BlockCopy(body, 0, packet, ZoneV2FrameIO.HeaderSize, body.Length);

        lock (_sendLock)
        {
            if (_closed || _stream == null)
                return;
            try
            {
                _stream.Write(packet, 0, packet.Length);
                _stream.Flush();
            }
            catch (Exception ex)
            {
                if (!_closed && !_handedOff)
                    RaiseLost(ex.Message);
            }
        }
    }

    async Task ReadLoop()
    {
        try
        {
            while (!_closed && !_ct.IsCancellationRequested)
            {
                ZoneV2Frame? frame = await ZoneV2FrameIO.TryReadAsync(_stream, _ct).ConfigureAwait(false);
                if (frame == null)
                    break;
                if (_closed)
                    break;
                Dispatch(frame.Value);
                if (_closed || _handedOff)
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            /* disconnect */
        }
        catch (Exception ex)
        {
            if (!_closed && !_handedOff)
                RaiseLost(ex.Message);
            return;
        }

        if (!_closed && !_handedOff)
            RaiseLost("The login server closed the connection.");
    }

    void Dispatch(ZoneV2Frame frame)
    {
        switch (frame.Type)
        {
            case ZoneV2MsgType.CharacterList:
                ZoneV2CharacterSummary[] rows;
                if (!ZoneV2Messages.TryDecodeCharacterList(frame.Body, out rows))
                {
                    RaiseFailed("bad_request", "Character list was not readable.");
                    return;
                }

                Action<ZoneV2CharacterSummary[]> roster = Roster;
                if (roster != null)
                    roster(rows);
                return;

            case ZoneV2MsgType.Handoff:
                ZoneV2Handoff handoff;
                if (!ZoneV2Messages.TryDecodeHandoff(frame.Body, out handoff))
                {
                    RaiseFailed("bad_request", "Handoff ticket was not readable.");
                    return;
                }

                _handedOff = true;
                if (string.IsNullOrEmpty(handoff.Account))
                    handoff.Account = Account;
                Action<ZoneV2Handoff> handoffHandler = Handoff;
                if (handoffHandler != null)
                    handoffHandler(handoff);
                Close();
                return;

            case ZoneV2MsgType.CharacterCreated:
                ZoneV2CharacterCreated created;
                if (ZoneV2Messages.TryDecodeCharacterCreated(frame.Body, out created))
                {
                    Action<ZoneV2CharacterCreated> createdHandler = Created;
                    if (createdHandler != null)
                        createdHandler(created);
                }

                return;

            case ZoneV2MsgType.CharacterDeleted:
                ZoneV2CharacterDeleted deleted;
                if (ZoneV2Messages.TryDecodeCharacterDeleted(frame.Body, out deleted))
                {
                    Action<ZoneV2CharacterDeleted> deletedHandler = Deleted;
                    if (deletedHandler != null)
                        deletedHandler(deleted);
                }

                return;

            case ZoneV2MsgType.CharacterDeleteFailed:
                ZoneV2CharacterDeleteFailed failed;
                if (!ZoneV2Messages.TryDecodeCharacterDeleteFailed(frame.Body, out failed))
                {
                    RaiseFailed("delete_failed", "Could not delete that character.");
                    return;
                }

                Action<ZoneV2CharacterDeleteFailed> deleteFailed = DeleteFailed;
                if (deleteFailed != null)
                    deleteFailed(failed);
                else
                    RaiseFailed(failed.Code, failed.Message);
                return;

            case ZoneV2MsgType.Error:
                string code;
                string message;
                ZoneV2Messages.TryDecodeError(frame.Body, out code, out message);
                RaiseFailed(code, message);
                return;

            default:
                return;
        }
    }

    void RaiseFailed(string code, string message)
    {
        Action<string, string> failed = Failed;
        if (failed != null)
            failed(code, message);
    }

    void RaiseLost(string reason)
    {
        Action<string> lost = Lost;
        if (lost != null)
            lost(reason);
    }

    public static async Task<ZoneV2Frame> ReadExpectedAsync(NetworkStream stream, ZoneV2MsgType expected, CancellationToken ct)
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
