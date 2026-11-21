/// <summary>Fineage Net WeddingEngageAnswer (0x03CB) — 1 accept, 0 decline.</summary>
public sealed class WeddingEngageAnswerCommand : INetworkCommand
{
    public int Response { get; }

    public WeddingEngageAnswerCommand(int response)
    {
        Response = response;
    }
}