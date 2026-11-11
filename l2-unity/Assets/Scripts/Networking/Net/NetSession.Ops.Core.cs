using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;
using System.Collections.Concurrent;
using VContainer;

public sealed partial class NetSession : MonoBehaviour
{
    [VContainer.Inject]
    public void ConstructApplyServices(
        CombatAttackApplyService combatAttack,
        CombatDieApplyService combatDie,
        CombatTargetApplyService combatTarget,
        StatusUpdateApplyService statusUpdate,
        DeleteObjectGroundApplyService deleteObjectGround,
        StatusApplyService status,
        SkillApplyService skills,
        PresenceApplyService presence,
        InventoryApplyService inventory,
        PartyApplyService party,
        TradeApplyService trade,
        TradeRequestApplyService tradeRequest,
        IWorldDomainSink domain)
    {
        _combatAttack = combatAttack;
        _combatDie = combatDie;
        _combatTarget = combatTarget;
        _statusUpdate = statusUpdate;
        _deleteObjectGround = deleteObjectGround;
        _status = status;
        _skills = skills;
        _presence = presence;
        _inventoryApply = inventory;
        _party = party;
        _trade = trade;
        _tradeRequest = tradeRequest;
        // The sink used to be injected into the static facades only, so this seam never ran.
        // Wire it here as well: with a sink bound, NetSession applies domain moves directly.
        if (domain != null)
            BindDomainSink(domain);
    }

    public Task BeginLobby(string account, string password, bool registerIfMissing = false) =>
        _lobbyFlow.BeginLobby(account, password, registerIfMissing);

    public Task RequestRoster() => _lobbyFlow.RequestRoster();

    public Task SelectCharacter(int characterId, string name = "", int slot = -1) =>
        _lobbyFlow.SelectCharacter(characterId, name, slot);

    public Task CreateCharacter(
        string name,
        int classId,
        int race,
        int sex,
        int hairStyle = 0,
        int hairColor = 0,
        int face = 0) =>
        _lobbyFlow.CreateCharacter(name, classId, race, sex, hairStyle, hairColor, face);

    /// <summary>Deletes a character by id (slot = -1) or by roster slot (id = 0).</summary>
    public Task DeleteCharacter(int characterId, int slot = -1) =>
        _lobbyFlow.DeleteCharacter(characterId, slot);

    /// <summary>Clears a pending delete timer. Same identifying convention as <see cref="DeleteCharacter"/>.</summary>
    public Task RestoreCharacter(int characterId, int slot = -1) =>
        _lobbyFlow.RestoreCharacter(characterId, slot);

    public Task EnterZoneWithHandoff(HandoffMsg handoff) =>
        _lobbyFlow.EnterZoneWithHandoff(handoff);


    void OnZonePacket(PacketEnvelope envelope)
    {
        if (envelope.Opcode == NetOpcode.HelloAck)
            _helloZone = true;
        if (IsCriticalZoneOpcode(envelope.Opcode))
            PostCritical(() => HandleZone(envelope));
        else
            Post(() => HandleZone(envelope));
    }
}
