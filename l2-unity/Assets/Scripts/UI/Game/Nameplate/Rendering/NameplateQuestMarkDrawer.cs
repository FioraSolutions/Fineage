using System.Collections.Generic;
using UnityEngine;

public sealed class NameplateQuestMarkDrawer
{
    public void Draw(List<NameplatePaintItem> paintList)
    {
        if (paintList == null)
            return;
        for (int i = 0; i < paintList.Count; i++)
        {
            NameplatePaintItem item = paintList[i];
            if (!item.ScreenValid || item.QuestMark == QuestNpcMarkKind.None)
                continue;
            Texture2D tex = QuestNpcMarkTextureCatalog.Load(item.QuestMark);
            if (tex == null)
                continue;

            const float size = 18f;
            float x = item.AnchorX - size * 0.5f;
            float y = Screen.height - item.YNameTop - size - 6f;
            var prev = GUI.color;
            GUI.color = QuestNpcMarkTextureCatalog.Tint(item.QuestMark);
            GUI.DrawTexture(new Rect(x, y, size, size), tex);
            GUI.color = prev;
        }
    }
}