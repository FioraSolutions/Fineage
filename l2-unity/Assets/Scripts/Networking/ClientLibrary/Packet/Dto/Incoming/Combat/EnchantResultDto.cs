public class EnchantResultDto : IWireDto
{
    private EnchantResult _result;

    public EnchantResult Result { get { return _result; } }

    public void ReadFrom(PacketReader reader)
    {
        int result = reader.ReadI();
        _result = ParseResult(result);
    }

    private static EnchantResult ParseResult(int resultType)
    {
        switch (resultType)
        {
            case (int)EnchantResult.Success:
                return EnchantResult.Success;
            case (int)EnchantResult.Unk_Result_1:
                return EnchantResult.Unk_Result_1;
            case (int)EnchantResult.Cancelled:
                return EnchantResult.Cancelled;
            case (int)EnchantResult.UnSuccess:
                return EnchantResult.UnSuccess;
            case (int)EnchantResult.Unk_Result_4:
                return EnchantResult.Unk_Result_4;
        }
        return EnchantResult.None;
    }
}
