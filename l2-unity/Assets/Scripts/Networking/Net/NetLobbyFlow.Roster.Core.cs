using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetLobbyFlow
{
    public async Task BeginLobby(string account, string password, bool registerIfMissing = false)
    {
        _resetLobbySession();
        // Dispose of a previous lobby may fire OnLobbyClosed — ignore until the new connect sticks.
        _awaitingLoginUi = false;
        try
        {
            _lobby?.Dispose();
            _helloLobby = false;
            _lobby = new NetTransport(OnLobbyPacket, reason => _post(() => OnLobbyClosed(reason)));
            await _lobby.ConnectAsync(FineageWireConfig.LoginHost, FineageWireConfig.LoginPort);
            _awaitingLoginUi = true;
            await SendHelloAsync(_lobby, "unity-net");
            await WaitUntil(() => _helloLobby, 5f);
            await _lobby.SendAsync(
                NetOpcode.Auth,
                NetCodec.EncodeAuth(new AuthMsg
                {
                    Account = account,
                    Password = password,
                    RegisterIfMissing = registerIfMissing
                }),
                _nextSeq());
        }
        catch (Exception ex)
        {
            Debug.LogError("[Net] BeginLobby failed: " + ex.Message);
            _awaitingLoginUi = false;
            _post(() => ShowLoginFailure(ex.Message));
        }
    }

    static void ShowLoginFailure(string message)
    {
        LoginWindow loginWindow = LoginPacketServices.LoginWindow;
        if (loginWindow == null)
            return;
        string text = string.IsNullOrWhiteSpace(message) ? "Login failed." : message.Trim();
        loginWindow.ShowErrorText(text);
    }

    public async Task RequestRoster()
    {
        if (_lobby == null || !_lobby.Connected)
            return;
        await _lobby.SendAsync(
            NetOpcode.ListCharacters,
            NetCodec.EncodeListCharacters(new ListCharactersMsg()),
            _nextSeq());
    }

    public async Task SelectCharacter(int characterId, string name = "", int slot = -1)
    {
        if (_lobby == null || !_lobby.Connected)
            return;
        await _lobby.SendAsync(
            NetOpcode.SelectCharacter,
            NetCodec.EncodeSelectCharacter(new SelectCharacterMsg
            {
                CharacterId = characterId,
                Name = name ?? "",
                Slot = slot
            }),
            _nextSeq());
    }
}
