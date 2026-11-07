
public class SocialActionDto : IWireDto
{
    private int _objectId;
    private int _actionId;

    public int ObjectId { get => _objectId; }
    public int ActionId { get => _actionId; }

    public SocialActionDto() { }

    SocialActionDto(int objectId, int actionId)
    {
        _objectId = objectId;
        _actionId = actionId;
    }

    public void ReadFrom(PacketReader reader)
    {
        _objectId = reader.ReadI();
        _actionId = reader.ReadI();
    }

    /// <summary>Net <c>SocialAction</c> (gesture id; 15 = the Mobius LEVEL_UP gesture).</summary>
    public static SocialActionDto FromNet(int objectId, int actionId) => new SocialActionDto(objectId, actionId);
}
