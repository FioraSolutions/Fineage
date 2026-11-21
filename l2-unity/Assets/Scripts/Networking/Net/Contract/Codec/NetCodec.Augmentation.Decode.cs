using System;

namespace Fineage.Net.Contract
{
    public static partial class NetCodec
    {
        public static VariationPutItemNetMsg DecodeVariationPutItem(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new VariationPutItemNetMsg
            {
                TargetObjectId = r.ReadInt32(),
                ItemId = r.ReadInt32(),
                Result = r.ReadInt32()
            };
        }

        public static VariationPutRefinerNetMsg DecodeVariationPutRefiner(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new VariationPutRefinerNetMsg
            {
                RefinerObjectId = r.ReadInt32(),
                ItemId = r.ReadInt32(),
                RefinerId = r.ReadInt32(),
                Result = r.ReadInt32()
            };
        }

        public static VariationPutGemNetMsg DecodeVariationPutGem(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new VariationPutGemNetMsg
            {
                GemstoneObjectId = r.ReadInt32(),
                RefinerItemId = r.ReadInt32(),
                GemstoneCount = r.ReadInt64(),
                Result = r.ReadInt32()
            };
        }

        public static VariationCancelItemNetMsg DecodeVariationCancelItem(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new VariationCancelItemNetMsg
            {
                TargetObjectId = r.ReadInt32(),
                ItemId = r.ReadInt32(),
                Result = r.ReadInt32()
            };
        }

        public static VariationCancelResultNetMsg DecodeVariationCancelResult(ReadOnlySpan<byte> p) =>
            new() { RefundAdena = new Reader(p).ReadInt32() };
    }
}