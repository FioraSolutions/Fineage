using NUnit.Framework;

public sealed class ForceChargeBuffLabelEditModeTests
{
    [Test]
    public void TraySkill4271_shows_charge_count_on_label()
    {
        Assert.IsTrue(
            AbnormalEffectTooltipBuilder.TryFormatChargeLabel(
                AbnormalEffectTooltipBuilder.ForceChargeTraySkillId,
                7,
                out string label));
        Assert.AreEqual("7", label);
    }
}