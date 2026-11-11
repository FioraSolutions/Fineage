using UnityEngine;

public partial class CalcNewPosition
{
    public Vector2 GetNewPosition(Vector2 newPoint, float offset, float heightContent, bool insideRoot, bool isForceBelow)
    {
        return insideRoot
            ? CalculateInsidePosition(newPoint, offset, heightContent , isForceBelow)
            : CalculateOutsidePosition(newPoint, offset, isForceBelow);
    }

    private Vector2 CalculateInsidePosition(Vector2 point, float offset, float heightContent, bool forceBelow)
    {
        if (forceBelow)
        {
            float adjustedY1 = point.y + (offset + BorderOffset);
            return new Vector2(point.x, adjustedY1);
        }

        float adjustedY = point.y - heightContent - (offset + BorderOffset);
        return new Vector2(point.x, adjustedY);
    }

    private Vector2 CalculateOutsidePosition(Vector2 point, float offset, bool forceBelow)
    {
        if (forceBelow)
        {
            float adjustedY = point.y + (offset + BorderOffset);
            return new Vector2(point.x, adjustedY);
        }

        float adjustedX = point.x + (offset + BorderOffset);
        return new Vector2(adjustedX, 0);
    }
}
