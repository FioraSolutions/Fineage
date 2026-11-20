using System.Collections.Generic;
using NUnit.Framework;

/// <summary>
/// Interlude AVE_STEALTH mapping from zone AbnormalStatusUpdate rows (#stealth semi-transparency).
/// </summary>
public sealed class AbnormalVisualStealthEditModeTests
{
    [Test]
    public void ResolveFlags_maps_stealth_from_visual_and_slot()
    {
        var byVisual = new List<EffectHolder>
        {
            new EffectHolder(221, 1, 120, "", "STEALTH"),
        };
        Assert.AreEqual(AbnormalAveFlags.Stealth, AbnormalVisualCatalog.ResolveFlags(byVisual));

        var bySlot = new List<EffectHolder>
        {
            new EffectHolder(411, 1, 120, "STEALTH", ""),
        };
        Assert.AreEqual(AbnormalAveFlags.Stealth, AbnormalVisualCatalog.ResolveFlags(bySlot));
    }

    [Test]
    public void FromZoneEffects_preserves_stealth_visual_for_self_bar_sync()
    {
        var effects = new[]
        {
            new NetAbnormalEffect
            {
                Id = 366,
                Value = 1,
                DurationSeconds = 60,
                Slot = "DANCE_OF_SHADOW",
                Visual = "STEALTH",
            },
        };

        AbnormalStatusUpdateDto dto = AbnormalStatusUpdateDto.FromZoneEffects(42, effects);
        Assert.IsTrue(dto.FullEffectSnapshot);
        Assert.AreEqual(1, dto.ListEffect.Count);
        Assert.AreEqual("STEALTH", dto.ListEffect[0].Visual);
        Assert.AreEqual(AbnormalAveFlags.Stealth, AbnormalVisualCatalog.ResolveFlags(dto.ListEffect));
    }
}