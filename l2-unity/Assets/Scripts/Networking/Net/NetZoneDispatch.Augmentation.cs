using Fineage.Net.Contract;

public sealed partial class NetZoneDispatch
{
    static bool HandleAugmentation(ZoneApplyPacket packet) => packet.Opcode switch
    {
        NetOpcode.AugmentShowMakeWindow => Apply(NetAugmentInbound.ShowMakeWindow),
        NetOpcode.AugmentShowCancelWindow => Apply(NetAugmentInbound.ShowCancelWindow),
        NetOpcode.AugmentPutItemResult => Apply(NetAugmentInbound.PutItemResult, (VariationPutItemNetMsg)packet.Body),
        NetOpcode.AugmentPutRefinerResult => Apply(NetAugmentInbound.PutRefinerResult, (VariationPutRefinerNetMsg)packet.Body),
        NetOpcode.AugmentPutGemResult => Apply(NetAugmentInbound.PutGemResult, (VariationPutGemNetMsg)packet.Body),
        NetOpcode.AugmentVariationResult => Apply(NetAugmentInbound.VariationResult, (VariationResultNetMsg)packet.Body),
        NetOpcode.AugmentPutCancelItemResult => Apply(NetAugmentInbound.PutCancelItemResult, (VariationCancelItemNetMsg)packet.Body),
        NetOpcode.AugmentCancelResult => Apply(NetAugmentInbound.CancelResult, (VariationCancelResultNetMsg)packet.Body),
        _ => false
    };

    static bool Apply(System.Action fn)
    {
        fn();
        return true;
    }

    static bool Apply<T>(System.Action<T> fn, T value)
    {
        fn(value);
        return true;
    }
}