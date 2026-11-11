using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetCommandSender
{
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

    public Task SendTargetCancelAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.TargetCancelRequest, Array.Empty<byte>(), _nextSeq());
    }

    public Task SendActionUseAsync(int actionId, bool ctrlPressed, bool shiftPressed)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestActionUse,
            NetCodec.EncodeActionUse(new ActionUseNetMsg
            {
                ActionId = actionId,
                CtrlPressed = ctrlPressed,
                ShiftPressed = shiftPressed
            }),
            _nextSeq());
    }

    public Task SendValidatePositionAsync(int x, int y, int z, int heading)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.ValidatePosition,
            NetCodec.EncodeValidatePosition(new ValidatePositionMsg
            {
                X = x,
                Y = y,
                Z = z,
                Heading = heading
            }),
            _nextSeq());
    }

    public Task SendSayAsync(string text, int chatType)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.Say,
            NetCodec.EncodeSay(new SayNetMsg { Text = text ?? "", ChatType = chatType }),
            _nextSeq());
    }

    public Task SendReviveRequestAsync(int action)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.ReviveRequest,
            NetCodec.EncodeReviveRequest(new ReviveRequestNetMsg { Action = action }),
            _nextSeq());
    }
}
