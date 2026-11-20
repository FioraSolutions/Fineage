using Fineage.Net.Contract;
using UnityEngine;

public static class NetQuestFxApply
{
    public static void ApplyQuestDirection(QuestDirectionNetMsg msg)
    {
        if (msg == null)
            return;
        QuestJournalState.ApplyDirection(msg);
        if (msg.Show && msg.X == 0 && msg.Y == 0)
            Debug.LogError($"[QuestRadar] show flag without coordinates quest id={msg.QuestId}.");
    }

    public static void ApplyNpcMarks(QuestNpcMarksNetMsg msg) => QuestJournalState.ApplyNpcMarks(msg);

    public static void ApplyPlaySound(PlaySoundNetMsg msg)
    {
        if (msg == null || string.IsNullOrWhiteSpace(msg.File))
        {
            Debug.LogError("[PlaySound] missing File on quest/tutorial sound packet.");
            return;
        }

        QuestInterludeSound.Play(msg.File, msg.Type);
    }

    public static void ApplyTutorial(TutorialUiNetMsg msg)
    {
        if (msg == null)
            return;
        TutorialPacketServices.Overlay?.Apply(msg);
    }
}