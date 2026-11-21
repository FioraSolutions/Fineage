namespace Fineage.Net.Contract
{
    /// <summary>Wedding opcodes ahead of Backend <c>dev</c> contract sync (see <c>docs/wedding-v2-protocol.md</c>).</summary>
    public static partial class NetOpcode
    {
        /// <summary>Client → zone: engage answer (<c>WeddingEngageAnswerNetMsg</c>).</summary>
        public const ushort WeddingEngageAnswer = 0x03C6;
        /// <summary>Zone → client: engage proposal (<c>WeddingEngageAskNetMsg</c>).</summary>
        public const ushort WeddingEngageAsk = 0x03C7;
    }
}