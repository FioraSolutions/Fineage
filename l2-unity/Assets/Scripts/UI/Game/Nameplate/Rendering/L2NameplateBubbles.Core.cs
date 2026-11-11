using UnityEngine;

public static partial class L2NameplateBubbles
{
    public static void AppendPair(
        L2NameplateScreenBatch batch,
        float nameLeft,
        float nameW,
        float yNameTop,
        float depth,
        float screenH,
        float extraPad,
        Color color)
    {
        if (batch == null)
        {
            return;
        }

        float pad = Mathf.Max(0f, extraPad);
        float y0 = yNameTop - BubbleYOffset;
        float y1 = y0 + BubbleHeight;

        // PNG top-left content → Unity UV (V flipped).
        float u0 = 0f;
        float u1 = ContentU / AtlasSize;
        float v1 = 1f;
        float v0 = 1f - (ContentV / AtlasSize);

        float leftX1 = nameLeft - BubbleWidth - pad;
        float leftX2 = nameLeft - pad;
        batch.AppendQuad(leftX1, y0, leftX2, y1, depth, screenH, color, true, u0, v0, u1, v1);

        float rightX1 = nameLeft + nameW;
        float rightX2 = rightX1 + BubbleWidth;
        batch.AppendQuad(rightX1, y0, rightX2, y1, depth, screenH, color, true, u0, v0, u1, v1);
    }
}
