using System;
using Fineage.Net.Contract;
using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// The Net wire contract exists twice: `l2-unity/.../Net/Contract` (Unity client) and
/// `Fineage-Backend/src/Fineage.Net.Contract` (server). Nothing links them, so they drift
/// silently — the client was missing 15 opcodes, 12 message types and 10 codec methods that
/// the zone server already sends. Decoding an unknown opcode is not fatal, so the symptom is
/// missing gameplay (no hotbar echo, no own UserInfo sheet), not a crash.
///
/// These tests pin the payload layouts that must agree byte-for-byte. They intentionally do
/// not try to compare the two source trees — a golden byte layout catches a changed field
/// order or width on either side, which is what actually breaks the wire.
/// </summary>
public class NetWireContractTests
{
    [Fact]
    public void ValidatePosition_round_trips_as_four_int32()
    {
        byte[] payload = NetCodec.EncodeValidatePosition(new ValidatePositionMsg
        {
            X = 12345,
            Y = -6789,
            Z = -3250,
            Heading = 32768
        });

        Assert.Equal(16, payload.Length);

        ValidatePositionMsg decoded = NetCodec.DecodeValidatePosition(payload);
        Assert.Equal(12345, decoded.X);
        Assert.Equal(-6789, decoded.Y);
        Assert.Equal(-3250, decoded.Z);
        Assert.Equal(32768, decoded.Heading);
    }

    [Fact]
    public void ActionUse_carries_three_fields_with_flags_as_single_bytes()
    {
        byte[] payload = NetCodec.EncodeActionUse(new ActionUseNetMsg
        {
            ActionId = 1,
            CtrlPressed = true,
            ShiftPressed = false
        });

        Assert.Equal(6, payload.Length);

        ActionUseNetMsg decoded = NetCodec.DecodeActionUse(payload);
        Assert.Equal(1, decoded.ActionId);
        Assert.True(decoded.CtrlPressed);
        Assert.False(decoded.ShiftPressed);
    }

    [Fact]
    public void ChangeMoveType_carries_effective_speeds_and_water_flag()
    {
        byte[] payload = NetCodec.EncodeChangeMoveType(new ChangeMoveTypeNetMsg
        {
            EntityValue = ulong.MaxValue,
            EntityGeneration = 3,
            Running = false,
            RunSpeed = 124.5f,
            WalkSpeed = 80.25f,
            InWater = true
        });

        // 8 (entity) + 4 (generation) + 1 (running) + 4 + 4 (speeds) + 1 (water)
        Assert.Equal(22, payload.Length);

        ChangeMoveTypeNetMsg decoded = NetCodec.DecodeChangeMoveType(payload);
        Assert.Equal(ulong.MaxValue, decoded.EntityValue);
        Assert.Equal(3u, decoded.EntityGeneration);
        Assert.False(decoded.Running);
        Assert.Equal(124.5f, decoded.RunSpeed);
        Assert.Equal(80.25f, decoded.WalkSpeed);
        Assert.True(decoded.InWater);
    }

    [Fact]
    public void EtcStatusUpdate_keeps_mobius_field_order()
    {
        // Mobius EtcStatusUpdate.java:39-45 writes charges, weight, chat, danger,
        // grade, charm, deathPenalty — four byte flags in the middle.
        byte[] payload = NetCodec.EncodeEtcStatusUpdate(new EtcStatusUpdateNetMsg
        {
            EntityValue = 7,
            EntityGeneration = 1,
            Charges = 3,
            WeightPenalty = 4,
            ChatBlocked = false,
            DangerArea = true,
            GradePenalty = true,
            CharmOfCourage = false,
            DeathPenalty = 15
        });

        // 8 + 4 + 4 + 4 + 4 bytes of flags + 4
        Assert.Equal(28, payload.Length);

        EtcStatusUpdateNetMsg decoded = NetCodec.DecodeEtcStatusUpdate(payload);
        Assert.Equal(3, decoded.Charges);
        Assert.Equal(4, decoded.WeightPenalty);
        Assert.False(decoded.ChatBlocked);
        Assert.True(decoded.DangerArea);
        Assert.True(decoded.GradePenalty);
        Assert.False(decoded.CharmOfCourage);
        Assert.Equal(15, decoded.DeathPenalty);
    }

    [Fact]
    public void ShortCut_row_is_four_int32_in_type_slot_id_level_order()
    {
        byte[] payload = NetCodec.EncodeShortCut(new ShortCutNetMsg
        {
            Type = 2,
            Slot = 47,
            Id = 1011,
            Level = 3
        });

        Assert.Equal(16, payload.Length);

        ShortCutNetMsg decoded = NetCodec.DecodeShortCut(payload);
        Assert.Equal(2, decoded.Type);
        Assert.Equal(47, decoded.Slot);
        Assert.Equal(1011, decoded.Id);
        Assert.Equal(3, decoded.Level);
    }

    [Fact]
    public void ShortCutInit_prefixes_the_row_count()
    {
        byte[] payload = NetCodec.EncodeShortCutInit(new ShortCutInitNetMsg
        {
            Shortcuts = new[]
            {
                new ShortCutNetMsg { Type = 1, Slot = 0, Id = 100, Level = 0 },
                new ShortCutNetMsg { Type = 2, Slot = 11, Id = 3, Level = 9 }
            }
        });

        Assert.Equal(4 + 16 + 16, payload.Length);

        ShortCutInitNetMsg decoded = NetCodec.DecodeShortCutInit(payload);
        Assert.Equal(2, decoded.Shortcuts.Length);
        Assert.Equal(11, decoded.Shortcuts[1].Slot);
        Assert.Equal(3, decoded.Shortcuts[1].Id);
    }

    [Fact]
    public void ShortCutInit_rejects_an_absurd_row_count()
    {
        byte[] payload = new byte[4];
        System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(payload, 100_000);

        Assert.Throws<System.IO.InvalidDataException>(() => NetCodec.DecodeShortCutInit(payload));
    }

    [Fact]
    public void ShortCutDel_carries_one_world_slot()
    {
        byte[] payload = NetCodec.EncodeShortCutDel(new ShortCutDelNetMsg { Slot = 119 });

        Assert.Equal(4, payload.Length);
        Assert.Equal(119, NetCodec.DecodeShortCutDel(payload).Slot);
    }

    [Fact]
    public void UserInfo_round_trips_the_whole_sheet_and_paperdoll()
    {
        var msg = new UserInfoNetMsg
        {
            EntityValue = 1,
            EntityGeneration = 2,
            Name = "Elf",
            ClassId = 25,
            Race = 1,
            Sex = 0,
            X = 147450,
            Y = 27376,
            Z = -3612,
            RunSpeed = 124f,
            WalkSpeed = 80f,
            Running = true,
            Level = 20,
            Exp = 123456789L,
            Sp = 42,
            CurHp = 300,
            MaxHp = 400,
            CurMp = 200,
            MaxMp = 250,
            CurCp = 100,
            MaxCp = 100,
            Str = 20,
            Dex = 35,
            Con = 25,
            Int = 30,
            Wit = 20,
            Men = 25,
            PAtk = 50,
            MAtk = 60,
            PDef = 70,
            MDef = 80,
            PAtkSpd = 300,
            MAtkSpd = 333,
            Accuracy = 40,
            Evasion = 41,
            CritRate = 44,
            WeaponGradePenalty = 1,
            ArmorGradePenalty = 2,
            ClanId = 0,
            Paperdoll = new[]
            {
                new PaperdollSlotNet { ObjectId = 10, ItemId = 18, EnchantLevel = 4 },
                new PaperdollSlotNet { ObjectId = 11, ItemId = 22, EnchantLevel = 0 }
            }
        };

        UserInfoNetMsg decoded = NetCodec.DecodeUserInfo(NetCodec.EncodeUserInfo(msg));

        Assert.Equal(msg.Name, decoded.Name);
        Assert.Equal(msg.Level, decoded.Level);
        Assert.Equal(msg.Exp, decoded.Exp);
        Assert.Equal(msg.MaxHp, decoded.MaxHp);
        Assert.Equal(msg.CritRate, decoded.CritRate);
        Assert.Equal(msg.WeaponGradePenalty, decoded.WeaponGradePenalty);
        Assert.Equal(2, decoded.Paperdoll.Length);
        Assert.Equal(4, decoded.Paperdoll[0].EnchantLevel);
        Assert.Equal(22, decoded.Paperdoll[1].ItemId);
    }

    [Fact]
    public void UserInfo_rejects_an_absurd_paperdoll_count()
    {
        // Header is 8 + 4 + 4 (string length) + 4*2 + 4*3 + 4*2 + 1 + 4 + 8 + 4*25 + 4*3.
        // Build a valid prefix by encoding, then corrupt the paperdoll length field.
        byte[] payload = NetCodec.EncodeUserInfo(new UserInfoNetMsg { Name = "" });
        int paperdollLengthOffset = payload.Length - 4;

        System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(
            payload.AsSpan(paperdollLengthOffset),
            9999);

        Assert.Throws<System.IO.InvalidDataException>(() => NetCodec.DecodeUserInfo(payload));
    }

    [Fact]
    public void EntitySpawn_keeps_a_sparse_tail_compatible_with_older_peers()
    {
        // Paperdoll and Dead were appended, so an older payload is still readable.
        var msg = new EntitySpawnMsg
        {
            EntityValue = 5,
            EntityGeneration = 1,
            Kind = EntityKindNet.Player,
            IsSelf = false,
            Name = "Peer",
            TemplateId = -1,
            ClassId = 25,
            Race = 1,
            Sex = 0,
            X = 100,
            Y = 200,
            Z = 300,
            Heading = 1000,
            RunSpeed = 124f,
            WalkSpeed = 80f,
            Running = true,
            Attackable = false,
            Paperdoll = new[] { 18, 0, 0, 22 },
            Dead = true
        };

        EntitySpawnMsg decoded = NetCodec.DecodeEntitySpawn(NetCodec.EncodeEntitySpawn(msg));

        Assert.Equal(4, decoded.Paperdoll.Length);
        Assert.Equal(22, decoded.Paperdoll[3]);
        Assert.True(decoded.Dead);
    }

    [Fact]
    public void EntitySpawn_without_the_new_tail_still_decodes()
    {
        // Simulate a payload from a peer that predates Paperdoll/Dead by truncating them.
        byte[] full = NetCodec.EncodeEntitySpawn(new EntitySpawnMsg
        {
            EntityValue = 5,
            EntityGeneration = 1,
            Kind = EntityKindNet.Npc,
            Name = "Wolf",
            TemplateId = 20332,
            ClassId = 0,
            Race = 0,
            Sex = 0,
            X = 1,
            Y = 2,
            Z = 3,
            Heading = 4,
            RunSpeed = 100f,
            WalkSpeed = 60f,
            Running = true,
            Attackable = true
        });

        // Trailing bytes for an empty paperdoll array (ushort count 0) + the Dead flag.
        byte[] older = full.AsSpan(0, full.Length - 3).ToArray();

        EntitySpawnMsg decoded = NetCodec.DecodeEntitySpawn(older);
        Assert.Equal("Wolf", decoded.Name);
        Assert.True(decoded.Attackable);
        Assert.False(decoded.Dead);
        Assert.Empty(decoded.Paperdoll);
    }

    [Fact]
    public void Opcode_values_match_the_documented_net_protocol()
    {
        // Pinned so a renumbering on either side is caught here, not in a playtest.
        Assert.Equal(0x0350, NetOpcode.ValidatePosition);
        Assert.Equal(0x0351, NetOpcode.RequestActionUse);
        Assert.Equal(0x0352, NetOpcode.ChangeMoveType);
        Assert.Equal(0x0353, NetOpcode.AutoAttackStart);
        Assert.Equal(0x0354, NetOpcode.AutoAttackStop);
        Assert.Equal(0x0355, NetOpcode.RequestAutoSoulShot);
        Assert.Equal(0x0356, NetOpcode.AutoSoulShot);
        Assert.Equal(0x0357, NetOpcode.UserInfo);
        Assert.Equal(0x0358, NetOpcode.RequestShortCutReg);
        Assert.Equal(0x0359, NetOpcode.RequestShortCutDel);
        Assert.Equal(0x035A, NetOpcode.ShortCutInit);
        Assert.Equal(0x035B, NetOpcode.ShortCutRegister);
        Assert.Equal(0x035C, NetOpcode.ShortCutDelete);
        Assert.Equal(0x035D, NetOpcode.EtcStatusUpdate);
    }

    [Fact]
    public void Every_opcode_is_unique()
    {
        // A duplicate constant would silently route two messages to one handler.
        var seen = new System.Collections.Generic.Dictionary<ushort, string>();
        foreach (System.Reflection.FieldInfo field in typeof(NetOpcode).GetFields(
                     System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
        {
            if (field.FieldType != typeof(ushort))
                continue;

            var value = (ushort)field.GetValue(null)!;
            Assert.False(
                seen.ContainsKey(value),
                $"NetOpcode.{field.Name} reuses 0x{value:X4} from NetOpcode.{seen.GetValueOrDefault(value)}");
            seen[value] = field.Name;
        }

        Assert.True(seen.Count > 90, $"expected the full opcode set, saw {seen.Count}");
    }
}
