using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

/// <summary>
/// Outbound zone command sends for movement and combat. NetSession still owns the
/// <see cref="NetTransport"/> and request sequence; this type holds the encode/send bodies
/// so the session does not absorb every <c>Send*</c> implementation.
/// </summary>
public sealed class NetCommandSender
{
    readonly Func<NetTransport> _zone;
    readonly Func<uint> _nextSeq;

    public NetCommandSender(Func<NetTransport> zone, Func<uint> nextSeq)
    {
        _zone = zone ?? throw new ArgumentNullException(nameof(zone));
        _nextSeq = nextSeq ?? throw new ArgumentNullException(nameof(nextSeq));
    }

    public Task SendMoveAsync(int destX, int destY, int destZ)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.MoveInput,
            NetCodec.EncodeMoveInput(new MoveInputMsg { DestX = destX, DestY = destY, DestZ = destZ }),
            _nextSeq());
    }

    public Task SendSelectTargetAsync(int targetObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.SelectTarget,
            NetCodec.EncodeSelectTarget(new SelectTargetNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendAttackRequestAsync(int targetObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.AttackRequest,
            NetCodec.EncodeAttackRequest(new AttackRequestNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendUseSkillAsync(int skillId, int targetObjectId, bool ctrlPressed)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.UseSkill,
            NetCodec.EncodeUseSkill(new UseSkillNetMsg
            {
                SkillId = skillId,
                TargetObjectId = targetObjectId,
                CtrlPressed = ctrlPressed
            }),
            _nextSeq());
    }

    bool TryGetZone(out NetTransport zone)
    {
        zone = _zone();
        return zone != null && zone.Connected;
    }
}
