namespace Fineage.Net.Contract
{
    /// <summary>Wedding v2 opcodes (client-only until Backend adds them; after Package freight 0x03C6–0x03C9).</summary>
    public static class WeddingNetOpcode
    {
        /// <summary>Client → zone: engage answer (<c>WeddingEngageAnswerNetMsg</c>).</summary>
        public const ushort WeddingEngageAnswer = 0x03CB;
        /// <summary>Zone → client: engage proposal (<c>WeddingEngageAskNetMsg</c>).</summary>
        public const ushort WeddingEngageAsk = 0x03CC;
    }
}