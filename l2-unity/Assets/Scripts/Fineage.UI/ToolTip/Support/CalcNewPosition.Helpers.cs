using UnityEngine;

public partial class CalcNewPosition
{
    public Vector2 HighestPoint(Vector2 newPoint, float elementHeight)
    {
        float adjustedY = newPoint.y - (elementHeight + TooltipVerticalOffset);
        return new Vector2(newPoint.x, adjustedY);
    }

    public Vector2 GetHorizontalOffsetPosition(Vector2 point, float offset)
    {
        return new Vector2(point.x + offset, point.y);
    }

    public Vector2 GetVerticalOffsetPosition(Vector2 point, float offset)
    {
        return new Vector2(point.x, point.y + offset);
    }

    public Vector2 GetCenteredPosition(Vector2 point, float elementWidth)
    {
        float centerX = point.x - (elementWidth / 2f);
        return new Vector2(centerX, point.y);
    }
}
