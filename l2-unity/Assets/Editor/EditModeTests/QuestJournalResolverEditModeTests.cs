using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class QuestJournalResolverEditModeTests
{
    [Test]
    public void StepProg_prefers_journal_word_over_cond()
    {
        var quest = QuestInstance.FromNet(new Fineage.Net.Contract.QuestJournalEntryNet
        {
            QuestId = 1,
            Cond = 2,
            JournalWord = 7,
            State = 1
        });
        Assert.AreEqual(7, QuestJournalResolver.StepProg(quest));
    }

    [Test]
    public void Missing_step_logs_concrete_error_without_crashing()
    {
        var table = new QuestNameTable();
        TablePacketServices.Bind(questNames: table);
        var quest = new QuestInstance(999_999, 3, 0);
        LogAssert.Expect(LogType.Error, new Regex("missing questname row quest id=999999 step=3"));
        QuestName row = QuestJournalResolver.ResolveStep(quest);
        Assert.IsNull(row);
    }
}