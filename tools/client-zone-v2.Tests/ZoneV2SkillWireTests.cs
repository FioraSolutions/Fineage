using MessagePack;
using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

/// <summary>
/// Skill list and skill use wire. Layout matches Fineage-Backend <c>SkillProtocol</c> / <c>Messages.cs</c>.
/// </summary>
public class ZoneV2SkillWireTests
{
    [Fact]
    public void Opcodes_match_the_backend_skill_contract()
    {
        Assert.Equal(50, (ushort)ZoneV2MsgType.SkillUseRequest);
        Assert.Equal(1065, (ushort)ZoneV2MsgType.SkillList);
        Assert.Equal(1066, (ushort)ZoneV2MsgType.MagicSkillUse);
        for (ushort id = 1061; id <= 1064; id++)
            Assert.False(Enum.IsDefined(typeof(ZoneV2MsgType), id));
    }

    [Fact]
    public void Skill_use_request_matches_backend_keys()
    {
        byte[] body = ZoneV2Messages.EncodeSkillUseRequest(3, 100_008, true);
        var msg = MessagePackSerializer.Deserialize<SkillUseRequestTestMsg>(body);
        Assert.Equal(3, msg.SkillId);
        Assert.Equal(100_008, msg.TargetObjectId);
        Assert.True(msg.CtrlPressed);
    }

    [Fact]
    public void Skill_list_reads_rows_and_reuse()
    {
        var list = new SkillListTestMsg
        {
            Skills =
            [
                new SkillListEntryTestMsg { SkillId = 194, Level = 1, ReuseDelayMs = 0 },
                new SkillListEntryTestMsg { SkillId = 3, Level = 1, ReuseDelayMs = 1200 }
            ]
        };

        Assert.True(ZoneV2Messages.TryDecodeSkillList(Pack(list), out ZoneV2SkillEntry[] rows));
        Assert.Equal(2, rows.Length);
        Assert.Equal(194, rows[0].SkillId);
        Assert.Equal(0, rows[0].ReuseDelayMs);
        Assert.Equal(3, rows[1].SkillId);
        Assert.Equal(1200, rows[1].ReuseDelayMs);
    }

    [Fact]
    public void Skill_list_ignores_a_trailing_entry_key()
    {
        var extra = new SkillListEntryExtraTestMsg
        {
            SkillId = 99,
            Level = 2,
            ReuseDelayMs = 0,
            Future = "later"
        };
        var list = new SkillListExtraTestMsg { Skills = [extra] };

        Assert.True(ZoneV2Messages.TryDecodeSkillList(Pack(list), out ZoneV2SkillEntry[] rows));
        Assert.Single(rows);
        Assert.Equal(99, rows[0].SkillId);
        Assert.Equal(2, rows[0].Level);
    }

    [Fact]
    public void Magic_skill_use_reads_hit_time_and_reuse()
    {
        var msg = new MagicSkillUseTestMsg
        {
            CasterObjectId = 1,
            TargetObjectId = 100_008,
            SkillId = 3,
            SkillLevel = 1,
            HitTimeMs = 800,
            ReuseDelayMs = 3000
        };

        Assert.True(ZoneV2Messages.TryDecodeMagicSkillUse(Pack(msg), out ZoneV2MagicSkillUse use));
        Assert.Equal(1, use.CasterObjectId);
        Assert.Equal(100_008, use.TargetObjectId);
        Assert.Equal(3, use.SkillId);
        Assert.Equal(800, use.HitTimeMs);
        Assert.Equal(3000, use.ReuseDelayMs);
    }

    [Fact]
    public void Skill_error_notices_map_known_codes()
    {
        Assert.Equal("Not enough MP.", ZoneV2SkillNotices.Notice("skill_mp", ""));
        Assert.True(ZoneV2SkillNotices.TryLine("skill_reuse", null, out string line));
        Assert.Equal("Skill is not ready yet.", line);
        Assert.False(ZoneV2SkillNotices.TryLine("party_full", "", out _));
    }

    static byte[] Pack<T>(T message) => MessagePackSerializer.Serialize(message);
}

[MessagePackObject]
public sealed class SkillUseRequestTestMsg
{
    [Key(0)] public int SkillId { get; set; }
    [Key(1)] public int TargetObjectId { get; set; }
    [Key(2)] public bool CtrlPressed { get; set; }
}

[MessagePackObject]
public sealed class SkillListEntryTestMsg
{
    [Key(0)] public int SkillId { get; set; }
    [Key(1)] public int Level { get; set; }
    [Key(2)] public int ReuseDelayMs { get; set; }
}

[MessagePackObject]
public sealed class SkillListEntryExtraTestMsg
{
    [Key(0)] public int SkillId { get; set; }
    [Key(1)] public int Level { get; set; }
    [Key(2)] public int ReuseDelayMs { get; set; }
    [Key(3)] public string Future { get; set; } = "";
}

[MessagePackObject]
public sealed class SkillListTestMsg
{
    [Key(0)] public SkillListEntryTestMsg[] Skills { get; set; } = [];
}

[MessagePackObject]
public sealed class SkillListExtraTestMsg
{
    [Key(0)] public SkillListEntryExtraTestMsg[] Skills { get; set; } = [];
}

[MessagePackObject]
public sealed class MagicSkillUseTestMsg
{
    [Key(0)] public int CasterObjectId { get; set; }
    [Key(1)] public int TargetObjectId { get; set; }
    [Key(2)] public int SkillId { get; set; }
    [Key(3)] public int SkillLevel { get; set; }
    [Key(4)] public int HitTimeMs { get; set; }
    [Key(5)] public int ReuseDelayMs { get; set; }
}
