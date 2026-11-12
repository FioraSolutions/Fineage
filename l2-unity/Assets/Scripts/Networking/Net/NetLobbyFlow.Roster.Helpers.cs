using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetLobbyFlow
{
    public async Task CreateCharacter(
        string name,
        int classId,
        int race,
        int sex,
        int hairStyle = 0,
        int hairColor = 0,
        int face = 0)
    {
        if (_lobby == null || !_lobby.Connected)
        {
            Debug.LogWarning("[Net] CreateCharacter: lobby not connected");
            return;
        }

        await _lobby.SendAsync(
            NetOpcode.CreateCharacter,
            NetCodec.EncodeCreateCharacter(new CreateCharacterMsg
            {
                Name = name ?? "",
                ClassId = classId,
                Race = race,
                Sex = sex,
                HairStyle = hairStyle,
                HairColor = hairColor,
                Face = face
            }),
            _nextSeq());
    }

    public async Task DeleteCharacter(int characterId, int slot = -1)
    {
        if (_lobby == null || !_lobby.Connected)
        {
            Debug.LogWarning("[Net] DeleteCharacter: lobby not connected");
            return;
        }

        await _lobby.SendAsync(
            NetOpcode.DeleteCharacter,
            NetCodec.EncodeDeleteCharacter(new DeleteCharacterMsg
            {
                CharacterId = characterId,
                Slot = slot
            }),
            _nextSeq());
    }

    public async Task RestoreCharacter(int characterId, int slot = -1)
    {
        if (_lobby == null || !_lobby.Connected)
        {
            Debug.LogWarning("[Net] RestoreCharacter: lobby not connected");
            return;
        }

        await _lobby.SendAsync(
            NetOpcode.RestoreCharacter,
            NetCodec.EncodeRestoreCharacter(new RestoreCharacterMsg
            {
                CharacterId = characterId,
                Slot = slot
            }),
            _nextSeq());
    }

    static void ApplyRosterUi(CharacterRosterMsg roster)
    {
        LobbyCharacter[] characters = LobbyCharacterMap.FromNetRoster(roster);
        if (LoginPacketServices.CharSelect != null)
            LoginPacketServices.CharSelect.SetLobbyCharacters(characters);
        CharacterSelector selector = LoginPacketServices.Characters;
        if (selector != null)
            selector.SetLobbyCharacters(characters, 0);
        if (LoginPacketServices.CharSelect != null)
            LoginPacketServices.CharSelect.SelectLobbySlot(LobbyCharacterMap.PreferredIndex(characters, 0));
        LoginWindow loginWindow = LoginPacketServices.LoginWindow;
        if (loginWindow != null)
            loginWindow.ShowErrorText("");
        // GamePacketServices.Manager already prefers NetGameHost then NetworkingPacketServices (A09).
        IGameManager manager = NetGameHost.TryGet() ?? GamePacketServices.Manager;
        if (manager != null)
            manager.OnAuthAllowed();
        Debug.Log("[Net] roster count=" + characters.Length);
    }
}
