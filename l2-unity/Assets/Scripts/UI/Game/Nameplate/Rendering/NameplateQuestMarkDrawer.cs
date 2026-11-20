using System.Collections.Generic;
using UnityEngine;

public sealed class NameplateQuestMarkDrawer
{
    private Texture2D _question;
    private Texture2D _exclamation;

    public void Draw(List<NameplatePaintItem> paintList)
    {
        if (paintList == null)
            return;
        EnsureTextures();
        for (int i = 0; i < paintList.Count; i++)
        {
            NameplatePaintItem item = paintList[i];
            if (!item.ScreenValid || item.QuestMark == QuestNpcMarkKind.None)
                continue;
            Texture2D tex = item.QuestMark == QuestNpcMarkKind.InProgress ? _exclamation : _question;
            if (tex == null)
            {
                Debug.LogError($"[QuestNpcMark] missing quest marker texture mark={item.QuestMark} npcPaintId={item.Id}.");
                continue;
            }

            const float size = 18f;
            float x = item.AnchorX - size * 0.5f;
            float y = Screen.height - item.YNameTop - size - 6f;
            GUI.DrawTexture(new Rect(x, y, size, size), tex);
        }
    }

    void EnsureTextures()
    {
        _question ??= Resources.Load<Texture2D>("Data/SysTextures/Icon/etc_question_mark_i00");
        _exclamation ??= Resources.Load<Texture2D>("Data/SysTextures/Icon/icon.etc_quest_exclamation_i00")
                          ?? Resources.Load<Texture2D>("Data/SysTextures/Icon/etc_question_mark_i00");
    }
}