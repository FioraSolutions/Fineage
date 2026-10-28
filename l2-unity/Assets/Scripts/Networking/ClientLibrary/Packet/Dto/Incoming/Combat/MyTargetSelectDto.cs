public class MyTargetSelectDto : IWireDto
{
    private int _objectId;
    private string _color;
  

    public int ObjectId { get => _objectId; }
    public string Color { get => _color; }
    

    /// <summary>Zone V2 <c>MyTargetSelected</c> (1008) color hint.</summary>
    public static MyTargetSelectDto FromZoneV2(int objectId, int colorHint)
    {
        var dto = new MyTargetSelectDto();
        dto._objectId = objectId;
        dto._color = ColorFromInterludeHint(colorHint);
        return dto;
    }

    static string ColorFromInterludeHint(int color)
    {
        if (color == 11)
            return "#1410b7";
        if (color == 0)
            return "#ffffff";
        if (color == 5)
            return "#a2fbab";
        if (color == 7)
            return "#a2a5fc";
        return "#ffffff";
    }

    public void ReadFrom(PacketReader reader)
    {
        _objectId = reader.ReadI();
        _color = ParceColor(reader.ReadSh());
        reader.ReadI();
    }

    private string ParceColor(int color) => ColorFromInterludeHint(color);


}
