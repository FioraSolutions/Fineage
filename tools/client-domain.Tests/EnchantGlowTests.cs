using System.Collections.Generic;
using Xunit;

namespace Fineage.Client.Domain.Tests;

public class EnchantGlowTests
{
    public EnchantGlowTests()
    {
        EnchantGlow.Reset();
        EnchantGlow.LoadText(EnchantGlow.DefaultLevels);
    }

    [Fact]
    public void Cap_clamps_to_the_official_0_127_range()
    {
        Assert.Equal(0, EnchantGlow.Cap(-3));
        Assert.Equal(15, EnchantGlow.Cap(15));
        Assert.Equal(127, EnchantGlow.Cap(200));
    }

    [Fact]
    public void TryStyle_hides_glow_below_plus_four()
    {
        Assert.False(EnchantGlow.TryStyle(EnchantGlow.KindWeapon, 0, out _));
        Assert.False(EnchantGlow.TryStyle(EnchantGlow.KindWeapon, 3, out _));
        Assert.False(EnchantGlow.TryStyle(EnchantGlow.KindArmor, 3, out _));
    }

    [Fact]
    public void TryStyle_follows_the_Interlude_color_steps()
    {
        Assert.True(EnchantGlow.TryStyle(EnchantGlow.KindWeapon, 4, out EnchantGlowStyle blue));
        Assert.Equal(4, blue.Level);
        Assert.True(blue.B > blue.R);

        Assert.True(EnchantGlow.TryStyle(EnchantGlow.KindWeapon, 7, out EnchantGlowStyle purple));
        Assert.Equal(7, purple.Level);
        Assert.True(purple.B > 0.8f && purple.R > 0.5f);

        Assert.True(EnchantGlow.TryStyle(EnchantGlow.KindWeapon, 10, out EnchantGlowStyle red));
        Assert.Equal(10, red.Level);
        Assert.True(red.R > red.B);

        Assert.True(EnchantGlow.TryStyle(EnchantGlow.KindWeapon, 15, out EnchantGlowStyle gold));
        Assert.Equal(15, gold.Level);
        Assert.True(gold.R > 0.9f && gold.G > 0.7f);

        Assert.True(EnchantGlow.TryStyle(EnchantGlow.KindWeapon, 20, out EnchantGlowStyle high));
        Assert.Equal(16, high.Level);
        Assert.True(EnchantGlow.TryStyle(EnchantGlow.KindWeapon, 127, out EnchantGlowStyle cap));
        Assert.Equal(16, cap.Level);
    }

    [Fact]
    public void Armor_uses_the_same_hue_at_a_lower_intensity()
    {
        Assert.True(EnchantGlow.TryStyle(EnchantGlow.KindWeapon, 7, out EnchantGlowStyle weapon));
        Assert.True(EnchantGlow.TryStyle(EnchantGlow.KindArmor, 7, out EnchantGlowStyle armor));
        Assert.Equal(weapon.R, armor.R);
        Assert.Equal(weapon.G, armor.G);
        Assert.Equal(weapon.B, armor.B);
        Assert.True(armor.Intensity < weapon.Intensity);
    }

    [Fact]
    public void ParseClassic_reads_the_squire_sword_rangesample()
    {
        const string raw =
            "Enchanted={{[LineageWeapons.rangesample];{0.95;0.55;0.55};{11.0;0.0;0.0};{0.0;0.0;0.0};0.0;0.0;0.0;{0.0;0.0;0.0};{0.0;0.0;0.0};{0.0;0.0;0.0}}}";
        Assert.True(EnchantGlow.TryParseClassic(raw, out EnchantGlowPlacement right, out EnchantGlowPlacement left));
        Assert.True(right.Present);
        Assert.Equal(EnchantGlow.DefaultMesh, right.Mesh);
        Assert.Equal(0.95f, right.ScaleX, 5);
        Assert.Equal(0.55f, right.ScaleY, 5);
        Assert.Equal(11f, right.OffsetX, 5);
        Assert.Equal(0f, right.OffsetY, 5);
        Assert.False(left.Present);
    }

    [Fact]
    public void ParseClassic_ignores_an_explicit_None_mesh()
    {
        const string raw =
            "Enchanted={{[None];{1.0;1.0;1.0};{0.0;0.0;0.0};{0.0;0.0;0.0};0.0;0.0;0.0;{0.0;0.0;0.0};{0.0;0.0;0.0};{0.0;0.0;0.0}}}";
        Assert.True(EnchantGlow.TryParseClassic(raw, out EnchantGlowPlacement right, out EnchantGlowPlacement left));
        Assert.False(right.Present);
        Assert.False(left.Present);
    }

    [Fact]
    public void ParseClassic_reads_a_second_hand()
    {
        const string raw =
            "{{[LineageWeapons.rangesample];{0.65;0.65;0.65};{0.0;4.0;0.0};{0.0;3.0;0.0};0.6;0.1;0.0;{0.0;0.0;0.0};{0.0;0.0;0.0};{0.0;0.0;0.0}};{[LineageWeapons.rangesample];{0.6;0.6;0.6};{0.0;-4.0;0.0};{0.0;0.0;0.0};0.0;0.0;0.0;{0.0;0.0;0.0};{0.0;0.0;0.0};{0.0;0.0;0.0}}}";
        Assert.True(EnchantGlow.TryParseClassic(raw, out EnchantGlowPlacement right, out EnchantGlowPlacement left));
        Assert.True(right.Present);
        Assert.True(left.Present);
        Assert.Equal(4f, right.OffsetY, 5);
        Assert.Equal(-4f, left.OffsetY, 5);
        EnchantGlowPlacement picked = EnchantGlow.PickHand(right, left, true);
        Assert.Equal(-4f, picked.OffsetY, 5);
    }

    [Fact]
    public void ParseInterludeRow_uses_the_documented_rangeA_columns()
    {
        Assert.Equal(70, EnchantGlow.InterludeRangeA);
        Assert.Equal(71, EnchantGlow.InterludeRangeB);
        Assert.Equal(72, EnchantGlow.InterludeScaleAX);
        Assert.Equal(75, EnchantGlow.InterludeOffsetAX);

        var cols = new string[85];
        for (int i = 0; i < cols.Length; i++)
            cols[i] = "";
        cols[70] = "LineageWeapons.rangesample";
        cols[72] = "0.94999999";
        cols[73] = "0.55000001";
        cols[74] = "0.55000001";
        cols[75] = "11.00000000";
        cols[76] = "0.00000000";
        cols[77] = "0.00000000";

        Assert.True(EnchantGlow.TryParseInterludeRow(cols, out EnchantGlowPlacement right, out EnchantGlowPlacement left));
        Assert.True(right.Present);
        Assert.False(left.Present);
        Assert.Equal(EnchantGlow.DefaultMesh, right.Mesh);
        Assert.Equal(0.94999999f, right.ScaleX, 5);
        Assert.Equal(11f, right.OffsetX, 5);
        Assert.Equal(0.01f, EnchantGlow.UuToMeters);
    }

    [Fact]
    public void DefaultLevels_parse_as_weapon_and_armor_tables()
    {
        Dictionary<string, List<EnchantGlowStyle>> tables = EnchantGlow.ParseLevels(EnchantGlow.DefaultLevels);
        Assert.True(tables.ContainsKey(EnchantGlow.KindWeapon));
        Assert.True(tables.ContainsKey(EnchantGlow.KindArmor));
        Assert.Equal(4, tables[EnchantGlow.KindWeapon][0].Level);
        Assert.Equal(16, tables[EnchantGlow.KindWeapon][^1].Level);
    }
}
