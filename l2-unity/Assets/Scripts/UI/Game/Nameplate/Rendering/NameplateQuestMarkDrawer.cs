using System.Collections.Generic;
using UnityEngine;

public sealed class NameplateQuestMarkDrawer
{
    // LineageDecosTex Q_t00 / e_t00 are 128x256 two-frame sheets (top = rest, bottom = squash).
    const float SizePx = 18f;
    const float FrameSec = 0.35f;

    public void Draw(List<NameplatePaintItem> paintList)
    {
        if (paintList == null)
            return;
        int frame = Mathf.FloorToInt(Time.unscaledTime / FrameSec) & 1;
        // Unity UVs have y=0 at the bottom of the texture, so the top (rest) frame is v=0.5.
        var uv = new Rect(0f, frame == 0 ? 0.5f : 0f, 1f, 0.5f);
        for (int i = 0; i < paintList.Count; i++)
        {
            NameplatePaintItem item = paintList[i];
            if (!item.ScreenValid || item.QuestMark == QuestNpcMarkKind.None)
                continue;
            Texture2D tex = QuestNpcMarkTextureCatalog.Load(item.QuestMark);
            if (tex == null)
                continue;

            float x = item.AnchorX - SizePx * 0.5f;
            float y = Screen.height - item.YNameTop - SizePx - 6f;
            var prev = GUI.color;
            GUI.color = QuestNpcMarkTextureCatalog.Tint(item.QuestMark);
            GUI.DrawTextureWithTexCoords(new Rect(x, y, SizePx, SizePx), tex, uv);
            GUI.color = prev;
        }
    }
}
