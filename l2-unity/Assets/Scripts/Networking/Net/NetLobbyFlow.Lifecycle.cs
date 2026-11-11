using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetLobbyFlow
{
    public NetLobbyFlow(
        Func<uint> nextSeq,
        Action<Action> post,
        Action resetLobbySession,
        Action<int> setCharacterId,
        Action resetSessionObjectId,
        Func<HandoffMsg, Task> connectZoneForHandoff,
        Action<AuthResultMsg> authResult,
        Action<CharacterRosterMsg> rosterReceived,
        Action<DeleteResultMsg> deleteResultReceived,
        Action<HandoffMsg> handoffReceived)
    {
        _nextSeq = nextSeq ?? throw new ArgumentNullException(nameof(nextSeq));
        _post = post ?? throw new ArgumentNullException(nameof(post));
        _resetLobbySession = resetLobbySession ?? throw new ArgumentNullException(nameof(resetLobbySession));
        _setCharacterId = setCharacterId ?? throw new ArgumentNullException(nameof(setCharacterId));
        _resetSessionObjectId = resetSessionObjectId ?? throw new ArgumentNullException(nameof(resetSessionObjectId));
        _connectZoneForHandoff = connectZoneForHandoff ?? throw new ArgumentNullException(nameof(connectZoneForHandoff));
        _authResult = authResult;
        _rosterReceived = rosterReceived;
        _deleteResultReceived = deleteResultReceived;
        _handoffReceived = handoffReceived;
    }

    public void Dispose()
    {
        _lobby?.Dispose();
        _lobby = null;
    }

    void OnLobbyClosed(string reason)
    {
        Debug.Log("[Net] lobby " + reason);
        if (!_awaitingLoginUi)
            return;
        _awaitingLoginUi = false;
        ShowLoginFailure("Connection lost.");
    }
}
