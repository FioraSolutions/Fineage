using NUnit.Framework;

public sealed class L2SkillsPanelLayoutEditModeTests
{
    [Test]
    public void ComputeColumns_fits_six_slots_in_mainwnd_skills_host()
    {
        // CharacterInfo #SkillsHost content width after skills chrome padding (~248px).
        Assert.AreEqual(6, L2SkillsPanelLayout.ComputeColumns(248f));
    }

    [Test]
    public void ComputePanelWidth_matches_six_column_interlude_row()
    {
        float width = L2SkillsPanelLayout.ComputePanelWidth(6);
        Assert.AreEqual(216f, width, 0.01f);
    }

    [Test]
    public void RowFrameResource_uses_6x1_art_for_six_or_fewer_columns()
    {
        Assert.AreEqual(L2SkillsPanelLayout.RowFrame6, L2SkillsPanelLayout.RowFrameResourceForColumns(6));
        Assert.AreEqual(L2SkillsPanelLayout.RowFrame7, L2SkillsPanelLayout.RowFrameResourceForColumns(7));
    }
}