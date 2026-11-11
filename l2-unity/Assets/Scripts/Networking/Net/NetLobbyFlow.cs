using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetLobbyFlow
{
    NetTransport _lobby;
    bool _helloLobby;
    /// <summary>True from successful TCP connect until roster UI or a surfaced login failure.</summary>
    bool _awaitingLoginUi;
    HandoffMsg _handoff;

    readonly Func<uint> _nextSeq;
    readonly Action<Action> _post;
    readonly Action _resetLobbySession;
    readonly Action<int> _setCharacterId;
    readonly Action _resetSessionObjectId;
    readonly Func<HandoffMsg, Task> _connectZoneForHandoff;
    readonly Action<AuthResultMsg> _authResult;
    readonly Action<CharacterRosterMsg> _rosterReceived;
    readonly Action<DeleteResultMsg> _deleteResultReceived;
    readonly Action<HandoffMsg> _handoffReceived;


    public bool HasHandoff => _handoff != null;
    public HandoffMsg LastHandoff => _handoff;








    /// <summary>Deletes a character by id (slot = -1) or by roster slot (id = 0).</summary>

    /// <summary>Clears a pending delete timer. Same identifying convention as <see cref="DeleteCharacter"/>.</summary>






}
