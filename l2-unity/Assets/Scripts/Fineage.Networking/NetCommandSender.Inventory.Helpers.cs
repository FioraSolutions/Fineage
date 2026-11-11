using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetCommandSender
{
    public Task SendRequestAutoSoulShotAsync(int itemId, bool enabled)
    {
        if (itemId <= 0 || !TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestAutoSoulShot,
            NetCodec.EncodeAutoSoulShot(new AutoSoulShotNetMsg { ItemId = itemId, Enabled = enabled }),
            _nextSeq());
    }

    public Task SendShortCutRegAsync(int type, int worldSlot, int id, int level)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestShortCutReg,
            NetCodec.EncodeShortCut(new ShortCutNetMsg
            {
                Type = type,
                Slot = worldSlot,
                Id = id,
                Level = level
            }),
            _nextSeq());
    }

    public Task SendShortCutDelAsync(int worldSlot)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestShortCutDel,
            NetCodec.EncodeShortCutDel(new ShortCutDelNetMsg { Slot = worldSlot }),
            _nextSeq());
    }

    public Task SendEnchantRequestAsync(int scrollObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.EnchantRequest,
            NetCodec.EncodeEnchantRequest(new EnchantRequestNetMsg { ScrollObjectId = scrollObjectId }),
            _nextSeq());
    }

    public Task SendEnchantConfirmAsync(int targetObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.EnchantConfirm,
            NetCodec.EncodeEnchantConfirm(new EnchantConfirmNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendEnchantCancelAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.EnchantCancel, Array.Empty<byte>(), _nextSeq());
    }
}
