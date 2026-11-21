using System;
using Fineage.Net.Contract;

public static partial class NetZonePacketDecoder
{
    static bool TryDecodeAugment(ushort opcode, ReadOnlySpan<byte> payload, out object body)
    {
        switch (opcode)
        {
            case NetOpcode.AugmentShowMakeWindow:
            case NetOpcode.AugmentShowCancelWindow:
                body = null;
                return true;
            case NetOpcode.AugmentPutItemResult:
                body = NetCodec.DecodeVariationPutItem(payload);
                return true;
            case NetOpcode.AugmentPutRefinerResult:
                body = NetCodec.DecodeVariationPutRefiner(payload);
                return true;
            case NetOpcode.AugmentPutGemResult:
                body = NetCodec.DecodeVariationPutGem(payload);
                return true;
            case NetOpcode.AugmentVariationResult:
                body = NetCodec.DecodeVariationResult(payload);
                return true;
            case NetOpcode.AugmentPutCancelItemResult:
                body = NetCodec.DecodeVariationCancelItem(payload);
                return true;
            case NetOpcode.AugmentCancelResult:
                body = NetCodec.DecodeVariationCancelResult(payload);
                return true;
            case NetOpcode.CursedWeaponList:
                body = NetCodec.DecodeCursedWeaponList(payload);
                return true;
            case NetOpcode.CursedWeaponLocation:
                body = NetCodec.DecodeCursedWeaponLocation(payload);
                return true;
            default:
                body = null;
                return false;
        }
    }
}