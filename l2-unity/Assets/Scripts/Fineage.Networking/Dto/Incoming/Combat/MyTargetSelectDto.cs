using UnityEngine;

public class MyTargetSelectDto : IWireDto
{
    private int _objectId;
    private string _color;
  

    public int ObjectId { get => _objectId; }
    public string Color { get => _color; }
    

    /// <summary>Fineage Net <c>MyTargetSelected</c> (1008) color hint.</summary>
    public static MyTargetSelectDto FromNet(int objectId, int colorHint)
    {
        var dto = new MyTargetSelectDto();
        dto._objectId = objectId;
        dto._color = ColorFromInterludeHint(colorHint);
        return dto;
    }

    /// <summary>
    /// Mobius <c>MyTargetSelected</c> hints the <em>level difference</em> (player level − target level) for anything
    /// the player can auto-attack, and 0 for everything else (own pawn, merchants, guides). Retail shades the
    /// nameplate by that difference: 0 = white, weaker target = blue, stronger target = red.
    /// </summary>
    static string ColorFromInterludeHint(int color)
    {
        if (color == 0)
            return "#ffffff";
        if (color == 11)
            return "#1410b7";
        if (color == 5)
            return "#a2fbab";
        if (color == 7)
            return "#a2a5fc";

        // colour = playerLevel − targetLevel: positive means the target is weaker (blue ramp),
        // negative means it is stronger (red ramp). The magnitude picks the shade.
        bool weaker = color > 0;
        int steps = weaker ? color : -color;
        return steps switch
        {
            1 => weaker ? "#a2a5fc" : "#f5a2a2",
            2 => weaker ? "#7f86f4" : "#ef8383",
            3 => weaker ? "#5f68ea" : "#e86a6a",
            _ => weaker ? "#3d47cf" : "#d94f4f"
        };
    }

    public void ReadFrom(PacketReader reader)
    {
        _objectId = reader.ReadI();
        _color = ParceColor(reader.ReadSh());
        reader.ReadI();
    }

    private string ParceColor(int color) => ColorFromInterludeHint(color);


}
