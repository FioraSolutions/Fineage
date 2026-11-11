using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetCommandSender
{
    public Task SendRequestAcquireSkillInfoAsync(int skillId, int level, int skillType)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestAcquireSkillInfo,
            NetCodec.EncodeAcquireSkillRequest(skillId, level, skillType),
            _nextSeq());
    }

    public Task SendRequestAcquireSkillAsync(int skillId, int level, int skillType)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestAcquireSkill,
            NetCodec.EncodeAcquireSkillRequest(skillId, level, skillType),
            _nextSeq());
    }

    public Task SendNpcTalkAsync(int npcObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.NpcTalk,
            NetCodec.EncodeNpcTalk(new NpcTalkNetMsg { NpcObjectId = npcObjectId }),
            _nextSeq());
    }

    public Task SendBypassRequestAsync(int npcObjectId, string command)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.BypassRequest,
            NetCodec.EncodeBypassRequest(new BypassRequestNetMsg
            {
                NpcObjectId = npcObjectId,
                Command = command ?? ""
            }),
            _nextSeq());
    }

    public Task SendRequestMultiSellChooseAsync(int npcObjectId, int listId, int entryId, int amount)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestMultiSellChoose,
            NetCodec.EncodeRequestMultiSellChoose(new RequestMultiSellChooseNetMsg
            {
                NpcObjectId = npcObjectId,
                ListId = listId,
                EntryId = entryId,
                Amount = amount
            }),
            _nextSeq());
    }
}
