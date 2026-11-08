using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{
    void ApplyEntitySpawn(EntitySpawnMsg spawn)
    {
        if (spawn.Kind == EntityKindNet.Npc)
        {
            ApplyNpcSpawn(spawn);
            return;
        }

        if (spawn.Kind != EntityKindNet.Player)
            return;

        if (!spawn.IsSelf)
        {
            ApplyRemotePlayerSpawn(spawn);
            return;
        }

        // Zone object id of the local player; stable only for this EnterWorld (see CharacterId).
        SessionObjectId = (int)spawn.EntityValue;

        var identity = new EntityIdentity
        {
            EntityType = EntityType.Player,
            Id = (int)spawn.EntityValue,
            Name = string.IsNullOrEmpty(spawn.Name) ? "Player" : spawn.Name,
            PlayerClass = spawn.ClassId,
            Owned = true,
            IsRunning = spawn.Running
        };
        identity.SetL2jPos(spawn.X, spawn.Y, spawn.Z);

        int maxHp = spawn.MaxHp > 0 ? spawn.MaxHp : 100;
        var status = new PlayerStatus();
        status.SetHp(maxHp);

        var stats = new PlayerStats
        {
            Level = spawn.Level > 0 ? spawn.Level : 1,
            MaxHp = maxHp,
            MaxMp = 0,
            MaxCp = 0,
            BaseRunSpeed = Mathf.RoundToInt(spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f),
            BaseWalkingSpeed = Mathf.RoundToInt(spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f),
            RunRealSpeed = spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f,
            WalkRealSpeed = spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f
        };

        var appearance = new PlayerAppearance
        {
            Race = spawn.Race >= 0 && spawn.Race <= (int)CharacterRace.Dwarf
                ? spawn.Race
                : (int)MapClassId.GetCharacterRace(spawn.ClassId),
            Sex = spawn.Sex,
            Running = spawn.Running,
            // BaseClass is Fighter/Mystic (0/1), not L2 classId — ResolveRaceAnimation uses PlayerClass.
            BaseClass = CharacterClassParser.IsMage(spawn.ClassId)
                ? (int)BaseClass.MMagic
                : (int)BaseClass.Fighter
        };

        // Stash PlayerInfo then OnCharacterSelect → LoadSWMap.
        // Spawning happens in OnWorldSceneLoaded once the tile is ready.
        // Without PlayerInfo.Identity, LoadSWMap returns immediately and loading never ends.
        IZoneTransport transport = ZoneCommands.Transport;
        if (transport != null)
        {
            transport.PlayerInfo = new PlayerInfoInterlude
            {
                Identity = identity,
                Status = status,
                Stats = stats,
                Appearance = appearance
            };
        }
        else
            Debug.LogWarning("[Net] Zone transport missing on EntitySpawn — world load will stall");

        GameManager gameManager = NetworkingPacketServices.Manager ?? GamePacketServices.Manager;
        if (gameManager != null)
            gameManager.OnCharacterSelect();
        else
            Debug.LogWarning("[Net] GameManager missing on EntitySpawn — world load will stall");

        EnteredWorld?.Invoke();
    }

    void ApplyRemotePlayerSpawn(EntitySpawnMsg spawn)
    {
        CharInfoDto dto = new CharInfoDto();
        dto.Identity.Id = (int)spawn.EntityValue;
        dto.Identity.Name = string.IsNullOrEmpty(spawn.Name) ? "Unknown" : spawn.Name;
        dto.Identity.EntityType = EntityType.User;
        dto.Identity.SetL2jPos(spawn.X, spawn.Y, spawn.Z);
        dto.Identity.PlayerClass = spawn.ClassId;
        dto.Identity.IsRunning = spawn.Running;
        dto.Appearance.Race = spawn.Race >= 0 && spawn.Race <= (int)CharacterRace.Dwarf
            ? spawn.Race
            : (int)MapClassId.GetCharacterRace(spawn.ClassId);
        dto.Appearance.Sex = spawn.Sex;
        dto.Appearance.BaseClass = CharacterClassParser.IsMage(spawn.ClassId)
            ? (int)BaseClass.MMagic
            : (int)BaseClass.Fighter;
        dto.Appearance.Running = spawn.Running;
        float run = spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f;
        float walk = spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f;
        dto.Stats.BaseRunSpeed = Mathf.RoundToInt(run);
        dto.Stats.BaseWalkingSpeed = Mathf.RoundToInt(walk);
        dto.Stats.RunRealSpeed = run;
        dto.Stats.WalkRealSpeed = walk;
        dto.Stats.Level = spawn.Level > 0 ? spawn.Level : 1;
        int maxHp = spawn.MaxHp > 0 ? spawn.MaxHp : 100;
        dto.Stats.MaxHp = maxHp;
        dto.Status.SetHp(spawn.Dead ? 0 : maxHp);
        dto.Status.SetMp(100);
        dto.Stats.MaxMp = 100;
        if (spawn.PAtkSpd > 0)
        {
            dto.Stats.PAtkSpd = spawn.PAtkSpd;
            dto.Stats.PAtkRealSpeed = spawn.PAtkSpd;
        }
        // Paperdoll is what makes peers visible in gear: without it every other player spawns
        // undressed while the wearer sees their own kit (UserInfo path).
        if (spawn.Paperdoll != null && spawn.Paperdoll.Length > 0)
            ApplySpawnPaperdollItemIds(dto.Appearance, spawn.Paperdoll);
        if (_presence != null) _presence.ApplyCharInfo(dto);
        else CharInfoApplyIncomingPacketServices.ApplyCharInfo(dto);
    }

    static void ApplySpawnPaperdollItemIds(PlayerAppearance appearance, int[] itemIds) =>
        NetPaperdollApply.ApplyItemIds(appearance, itemIds);

    void ApplyNpcSpawn(EntitySpawnMsg spawn)
    {
        int objectId = (int)spawn.EntityValue;
        NpcInfoDto dto = new NpcInfoDto();
        dto.Identity.Id = objectId;
        dto.Identity.NpcId = spawn.TemplateId;
        if (!string.IsNullOrEmpty(spawn.Name))
            dto.Identity.Name = spawn.Name;
        dto.Identity.SetHideHp(spawn.TemplateId);
        dto.Identity.EntityType = EntityType.NPC;
        dto.Identity.SetL2jPos(spawn.X, spawn.Y, spawn.Z);
        dto.Identity.IsRunning = spawn.Running;
        dto.Stats.Level = spawn.Level > 0 ? spawn.Level : 1;
        // Wire carries MaxHp only; alive NPCs spawn at full HP. Stubbing CurHp=100 while MaxHp is
        // the real template value made TargetWindow BarFillRatio stick at 1 until a StatusUpdate
        // arrived — and look broken when updates lagged (#522).
        int maxHp = spawn.MaxHp > 0 ? spawn.MaxHp : 100;
        dto.Stats.MaxHp = maxHp;
        dto.Status.SetHp(spawn.Dead ? 0 : maxHp);
        int pAtkSpd = spawn.PAtkSpd > 0 ? spawn.PAtkSpd : 253;
        dto.Stats.PAtkSpd = pAtkSpd;
        dto.Stats.PAtkRealSpeed = pAtkSpd;
        float run = spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f;
        float walk = spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f;
        dto.Stats.BaseRunSpeed = Mathf.RoundToInt(run);
        dto.Stats.BaseWalkingSpeed = Mathf.RoundToInt(walk);
        dto.Stats.WalkRealSpeed = walk;
        dto.Stats.RunRealSpeed = run;
        dto.Appearance.CollisionRadius = 10f;
        dto.Appearance.CollisionHeight = 22f;
        NetNpcSession.RememberNpc(objectId, spawn.Name, spawn.Attackable);
        if (_presence != null) _presence.ApplyNpcInfo(dto);
        else NpcInfoApplyIncomingPacketServices.ApplyNpcInfo(dto);
    }

    void ApplyEntityDespawn(EntityDespawnMsg despawn)
    {
        int objectId = (int)despawn.EntityValue;
        NetNpcSession.Forget(objectId);
        var dto = new DeleteObjectDto { ObjectId = objectId };
        DeleteObjectGroundApplyService deleteObjectGround = _deleteObjectGround;
        if (deleteObjectGround != null) deleteObjectGround.Apply(dto);
        else DeleteObjectGroundApplyIncomingPacketServices.ApplyDeleteObject(dto);
    }

    void ApplyTargetSelected(TargetSelectedNetMsg msg)
    {
        MyTargetSelectDto dto = MyTargetSelectDto.FromNet(msg.TargetObjectId, msg.ColorHint);
        if (_combatTarget != null) _combatTarget.ApplyMyTargetSelected(dto);
        else MyTargetSelectedApplyIncomingPacketServices.ApplyMyTargetSelected(dto);
    }

    /// <summary>
    /// The zone dropped the selection. Clear the target window and nameplate without echoing another
    /// <c>RequestTargetCanceld</c> back: this arrived because that request was already answered.
    /// </summary>
    void ApplyTargetUnselected(TargetUnselectedNetMsg msg)
    {
        if (_combatTarget != null) _combatTarget.ClearTargetFromZone();
        else TargetPacketServices.Targets?.ClearTargetSilently();
    }

    void ApplyAttackState(AttackStateMsg attack)
    {
        var dto = new AttackDto();
        dto.InitFromNet(
            (int)attack.AttackerValue,
            (int)attack.TargetValue,
            attack.Damage,
            attack.Flags,
            attack.AttackerX,
            attack.AttackerY,
            attack.AttackerZ,
            attack.TargetX,
            attack.TargetY,
            attack.TargetZ);
        if (_combatAttack != null) _combatAttack.Apply(dto);
        else AttackApplyIncomingPacketServices.ApplyAttack(dto);
    }

    /// <summary>
    /// Mobius <c>AutoAttackStart</c>: the pawn raises its weapon. The zone was already fanning this,
    /// but with no case here a Net client never put an actor into attack stance.
    /// </summary>
    void ApplyAutoAttackStart(int entityId)
    {
        AutoAttackStartDto dto = AutoAttackStartDto.FromNet(entityId);
        WorldApplyIncomingPacketServices.QueueWorld(apply => apply.AutoAttackStart(dto));
    }

    /// <summary>Mobius <c>AutoAttackStop</c>: stance ended, so the pawn drops back to its wait pose.</summary>
    void ApplyAutoAttackStop(int entityId)
    {
        AutoAttackStopDto dto = AutoAttackStopDto.FromNet(entityId);
        WorldApplyIncomingPacketServices.QueueWorld(apply => apply.AutoAttackStop(dto));
    }


    void ApplyStatusUpdate(StatusUpdateNetMsg status)
    {
        var attributes = new System.Collections.Generic.List<StatusUpdate.Attribute>();
        int n = status.AttributeIds == null || status.Values == null
            ? 0
            : Mathf.Min(status.AttributeIds.Length, status.Values.Length);
        for (int i = 0; i < n; i++)
            attributes.Add(new StatusUpdate.Attribute(status.AttributeIds[i], status.Values[i]));
        if (_statusUpdate != null) _statusUpdate.Apply((int)status.EntityValue, attributes);
        else StatusUpdateApplyIncomingPacketServices.ApplyStatusUpdate((int)status.EntityValue, attributes);
    }

    /// <summary>Mobius ChangeWaitType: sit/stand on the actor, self included so a sit is acknowledged.</summary>
    void ApplyChangeWaitType(ChangeWaitTypeNetMsg msg)
    {
        ChangeWaitTypeDto dto = ChangeWaitTypeDto.FromNet((int)msg.EntityValue, msg.WaitType, msg.X, msg.Y, msg.Z);
        WorldApplyIncomingPacketServices.QueueWorld(apply => apply.ChangeWaitType(dto));
    }

    /// <summary>Mobius SocialAction: a gesture on the actor (15 = level up).</summary>
    void ApplySocialAction(SocialActionNetMsg msg)
    {
        SocialActionDto dto = SocialActionDto.FromNet((int)msg.EntityValue, msg.ActionId);
        WorldApplyIncomingPacketServices.QueueWorld(apply => apply.SocialAction(dto));
    }

    void ApplyEntityDie(EntityDieMsg die)
    {
        DieDto dieDto = DieDto.FromZoneWire((int)die.EntityValue);
        if (_combatDie != null) _combatDie.Apply(dieDto);
        else DieApplyIncomingPacketServices.ApplyDie(dieDto);
    }
    void ApplyDropItem(DropItemNetMsg drop)
    {
        DropItemDto dto = DropItemDto.FromNet(
            drop.DropperObjectId,
            (int)drop.EntityValue,
            drop.ItemId,
            drop.X,
            drop.Y,
            drop.Z,
            drop.Stackable,
            drop.Count,
            0);
        if (_presence != null) _presence.ApplyDropItem(dto);
        else DropItemApplyIncomingPacketServices.ApplyDropItem(dto);
    }

    void ApplyInventoryUpdate(InventoryUpdateNetMsg update)
    {
        InventoryUpdateEntryNet[] src = update.Changes ?? System.Array.Empty<InventoryUpdateEntryNet>();
        var rows = new NetInventoryChange[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            InventoryUpdateEntryNet row = src[i];
            rows[i] = new NetInventoryChange
            {
                Change = row.Change,
                ObjectId = row.ObjectId,
                ItemId = row.ItemId,
                Count = row.Count,
                EquippedSlot = row.EquippedSlot,
                EnchantLevel = row.EnchantLevel,
                BodySlot = "",
                Kind = "",
                Icon = "",
                WeaponType = "",
                ArmorType = ""
            };
        }

        NetGearSession.NoteInventoryUpdate(rows);
    }

    void ApplyCreatureSay(CreatureSayNetMsg say)
    {
        NetChatFeed.NoteSay(say.Name, say.Text, say.ChatType);
        // Chat log does not use ObjectId; overhead bubble does (#521).
        OverheadChatSay.Show(say.ObjectId, say.Text, say.ChatType);
    }

    void ApplySystemMessage(SystemMessageNetMsg message)
    {
        string shown = NetSystemLines.Resolve(message.Id, message.Text, message.Params);
        NetChatFeed.NoteSystem(shown);
    }

    /// <summary>A refused action now says why in chat instead of only logging.</summary>
    static void NoteReject(string code, string message)
    {
        string line = NetRejectLines.Resolve(code, message);
        if (!string.IsNullOrEmpty(line))
            NetChatFeed.NoteSystem(line);
    }

    void ApplyMagicSkillUse(MagicSkillUseNetMsg use)
    {
        var mapped = new NetMagicSkillUse
        {
            CasterObjectId = use.CasterObjectId,
            TargetObjectId = use.TargetObjectId,
            SkillId = use.SkillId,
            SkillLevel = use.SkillLevel,
            HitTimeMs = use.HitTimeMs,
            ReuseDelayMs = use.ReuseDelayMs
        };
        var dto = new MagicSkillUseDto();
        dto.InitFromNet(
            mapped.CasterObjectId,
            mapped.TargetObjectId,
            mapped.SkillId,
            mapped.SkillLevel,
            mapped.HitTimeMs,
            mapped.ReuseDelayMs);
        if (_skills != null) _skills.ApplyMagicSkillUse(dto);
        else MagicSkillUseApplyIncomingPacketServices.ApplyMagicSkillUse(dto);
    }

    void ApplyRevive(ReviveNetMsg revive)
    {
        var dto = new ReviveDto { ObjectId = revive.ObjectId };
        if (_presence != null) _presence.ApplyRevive(dto);
        else ReviveApplyIncomingPacketServices.ApplyRevive(dto);
    }

    void ApplyTeleportToLocation(TeleportToLocationNetMsg teleport)
    {
        TeleportToLocationDto dto = TeleportToLocationDto.FromZoneWire(
            teleport.ObjectId, teleport.X, teleport.Y, teleport.Z, teleport.Heading);
        if (_presence != null) _presence.ApplyTeleportToLocation(dto);
        else TeleportToLocationApplyIncomingPacketServices.ApplyTeleportToLocation(dto);
    }

    void ApplyAbnormalStatusUpdate(AbnormalStatusUpdateNetMsg update)
    {
        // FanoutAbnormal goes to owner + knownlist. Buff bar is self-only — peer snapshots
        // (including empty) must not wipe local icons.
        PlayerEntity local = DomainPacketServices.LocalPlayer;
        if (local != null && local.Identity != null && update.ObjectId != 0
            && update.ObjectId != local.Identity.Id)
            return;

        AbnormalEffectNet[] src = update.Effects ?? System.Array.Empty<AbnormalEffectNet>();
        var effects = new NetAbnormalEffect[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            AbnormalEffectNet e = src[i] ?? new AbnormalEffectNet();
            effects[i] = new NetAbnormalEffect
            {
                Id = e.Id,
                Value = e.Value,
                DurationSeconds = e.DurationSeconds
            };
        }

        AbnormalStatusUpdateDto dto = AbnormalStatusUpdateDto.FromZoneEffects(effects);
        if (_status != null) _status.ApplyAbnormalStatusUpdate(dto);
        else AbnormalStatusUpdateApplyIncomingPacketServices.ApplyAbnormalStatusUpdate(dto);
    }

    void ApplyNpcHtml(NpcHtmlNetMsg html)
    {
        HtmlWindow window = GameWindowPacketServices.Html;
        if (window == null)
            return;
        window.ShowNpcDialog(html.NpcObjectId, html.NpcId, html.Html ?? "");
    }

    void ApplyBuyList(BuyListNetMsg list)
    {
        BuyListItemNet[] src = list.Items ?? System.Array.Empty<BuyListItemNet>();
        var items = new NetBuyItem[src.Length];
        for (int i = 0; i < src.Length; i++)
            items[i] = new NetBuyItem { ItemId = src[i].ItemId, Price = src[i].Price, Count = src[i].Count };
        NetNpcWindows.ApplyBuyList(new NetBuyList
        {
            NpcObjectId = list.NpcObjectId,
            ListId = list.ListId,
            BaseTaxPercent = list.BaseTaxPercent,
            CastleTaxPercent = list.CastleTaxPercent,
            Items = items
        });
    }

    void ApplySellList(SellListNetMsg list)
    {
        SellListItemNet[] src = list.Items ?? System.Array.Empty<SellListItemNet>();
        var items = new NetSellItem[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            items[i] = new NetSellItem
            {
                ObjectId = src[i].ObjectId,
                ItemId = src[i].ItemId,
                Count = src[i].Count,
                Price = src[i].Price,
                EnchantLevel = src[i].EnchantLevel
            };
        }

        NetNpcWindows.ApplySellList(new NetSellList { NpcObjectId = list.NpcObjectId, Items = items });
    }

    void ApplyMultiSellList(MultiSellListNetMsg list)
    {
        MultiSellEntryNet[] src = list.Entries ?? System.Array.Empty<MultiSellEntryNet>();
        var entries = new NetMultiSellEntry[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            entries[i] = new NetMultiSellEntry
            {
                EntryId = src[i].EntryId,
                Ingredients = MapMultiParts(src[i].Ingredients),
                Products = MapMultiParts(src[i].Products)
            };
        }

        NetNpcWindows.ApplyMultiSellList(new NetMultiSellList
        {
            NpcObjectId = list.NpcObjectId,
            ListId = list.ListId,
            MaintainEnchantment = list.MaintainEnchantment,
            InventoryOnly = list.InventoryOnly,
            CastleTaxPercent = list.CastleTaxPercent,
            Entries = entries
        });
    }

    static NetMultiPart[] MapMultiParts(MultiSellPartNet[] parts)
    {
        if (parts == null || parts.Length == 0)
            return System.Array.Empty<NetMultiPart>();
        var rows = new NetMultiPart[parts.Length];
        for (int i = 0; i < parts.Length; i++)
            rows[i] = new NetMultiPart
            {
                ItemId = parts[i].ItemId,
                Count = parts[i].Count,
                EnchantLevel = parts[i].EnchantLevel
            };
        return rows;
    }

    void ApplyShopBalance(ShopBalanceNetMsg balance)
    {
        NetNpcWindows.ApplyShopBalance(new NetShopBalance
        {
            Adena = balance.Adena,
            Weight = balance.Weight,
            MaxWeight = balance.MaxWeight,
            Slots = balance.Slots,
            MaxSlots = balance.MaxSlots,
            BaseTaxPercent = balance.BaseTaxPercent,
            CastleTaxPercent = balance.CastleTaxPercent
        });
    }

    void ApplyWarehouseDepositList(WarehouseDepositListNetMsg list)
    {
        NetNpcWindows.ApplyWarehouseDepositList(new NetWarehouseDepositList
        {
            NpcObjectId = list.NpcObjectId,
            FeePerLine = list.FeePerLine,
            SlotsUsed = list.SlotsUsed,
            SlotsMax = list.SlotsMax,
            Adena = list.Adena,
            Items = MapWarehouseItems(list.Items)
        });
    }

    void ApplyWarehouseWithdrawList(WarehouseWithdrawListNetMsg list)
    {
        NetNpcWindows.ApplyWarehouseWithdrawList(new NetWarehouseWithdrawList
        {
            NpcObjectId = list.NpcObjectId,
            SlotsUsed = list.SlotsUsed,
            SlotsMax = list.SlotsMax,
            InventorySlotsUsed = list.InventorySlotsUsed,
            InventorySlotsMax = list.InventorySlotsMax,
            CurrentWeight = list.CurrentWeight,
            MaxWeight = list.MaxWeight,
            Items = MapWarehouseItems(list.Items)
        });
    }

    void ApplyQuestList(QuestListNetMsg list)
    {
        QuestJournalEntryNet[] active = list.Active ?? System.Array.Empty<QuestJournalEntryNet>();
        var rows = new NetQuestJournal[active.Length];
        for (int i = 0; i < active.Length; i++)
        {
            QuestJournalEntryNet row = active[i] ?? new QuestJournalEntryNet();
            rows[i] = new NetQuestJournal
            {
                QuestId = row.QuestId,
                Name = row.Name ?? "",
                State = row.State,
                Cond = row.Cond,
                ProgressFlags = row.ProgressFlags,
                JournalWord = row.JournalWord,
                Repeatable = row.Repeatable
            };
        }

        NetNpcWindows.ApplyQuestList(new NetQuestList
        {
            Active = rows,
            CompletedMask = list.CompletedMask ?? System.Array.Empty<byte>(),
            CompletedQuestIds = list.CompletedQuestIds ?? System.Array.Empty<int>(),
            MaxActive = list.MaxActive
        });
    }

    void ApplyAcquireSkillList(AcquireSkillListNetMsg list)
    {
        AcquireSkillEntryNet[] rows = list.Skills ?? System.Array.Empty<AcquireSkillEntryNet>();
        var ids = new int[rows.Length];
        var next = new int[rows.Length];
        var max = new int[rows.Length];
        var sp = new int[rows.Length];
        var requirements = new int[rows.Length];
        for (int i = 0; i < rows.Length; i++)
        {
            AcquireSkillEntryNet row = rows[i] ?? new AcquireSkillEntryNet();
            ids[i] = row.SkillId;
            next[i] = row.NextLevel;
            max[i] = row.MaxLevel;
            sp[i] = row.SpCost;
            requirements[i] = row.Requirements;
        }

        NetNpcWindows.ApplyAcquireSkillList(
            AcquireSkillListDto.FromNet(list.SkillType, ids, next, max, sp, requirements));
    }

    void ApplyAcquireSkillInfo(AcquireSkillInfoNetMsg info)
    {
        AcquireSkillRequirementNet[] reqs = info.Requirements ?? System.Array.Empty<AcquireSkillRequirementNet>();
        var types = new int[reqs.Length];
        var itemIds = new int[reqs.Length];
        var counts = new long[reqs.Length];
        var unks = new int[reqs.Length];
        for (int i = 0; i < reqs.Length; i++)
        {
            AcquireSkillRequirementNet req = reqs[i] ?? new AcquireSkillRequirementNet();
            types[i] = req.Type;
            itemIds[i] = req.ItemId;
            counts[i] = req.Count;
            unks[i] = req.Unk;
        }

        NetNpcWindows.ApplyAcquireSkillInfo(
            AcquireSkillInfoDto.FromNet(info.SkillId, info.Level, info.SpCost, info.SkillType, types, itemIds, counts, unks));
    }

    void ApplyEnchantResult(EnchantResultNetMsg result)
    {
        NetNpcWindows.ApplyEnchantResult(new NetEnchantResult
        {
            Result = result.Result,
            EnchantLevel = result.EnchantLevel,
            ObjectId = result.ObjectId,
            CrystalItemId = result.CrystalItemId,
            CrystalCount = result.CrystalCount
        });
    }

    void ApplyPledgeStatus(PledgeStatusNetMsg status)
    {
        ClanPacketServices.ApplyStatus(new NetPledgeStatus
        {
            ClanId = status.ClanId,
            Name = status.Name ?? "",
            Level = status.Level,
            LeaderObjectId = status.LeaderObjectId,
            CrestId = status.CrestId,
            MemberCount = status.MemberCount,
            DissolvingExpiryUnixMs = status.DissolvingExpiryUnixMs,
            CharPenaltyExpiryUnixMs = status.CharPenaltyExpiryUnixMs
        });
    }

    void ApplyPledgeMemberList(PledgeMemberListNetMsg list)
    {
        PledgeMemberNet[] rows = list.Members ?? System.Array.Empty<PledgeMemberNet>();
        var members = new NetPledgeMember[rows.Length];
        for (int i = 0; i < rows.Length; i++)
        {
            PledgeMemberNet row = rows[i] ?? new PledgeMemberNet();
            members[i] = new NetPledgeMember
            {
                ObjectId = row.ObjectId,
                Name = row.Name ?? "",
                ClassId = row.ClassId,
                Level = row.Level,
                Online = row.Online,
                PledgeType = row.PledgeType
            };
        }

        ClanPacketServices.ApplyMembers(new NetPledgeMemberList
        {
            ClanId = list.ClanId,
            LeaderObjectId = list.LeaderObjectId,
            Members = members
        });
    }

    void ApplyClanAskJoin(ClanAskJoinNetMsg ask)
    {
        ClanPacketServices.ApplyAskJoin(new NetClanAskJoin
        {
            RequestorObjectId = ask.RequestorObjectId,
            RequestorName = ask.RequestorName ?? "",
            ClanId = ask.ClanId,
            ClanName = ask.ClanName ?? "",
            PledgeType = ask.PledgeType
        });
    }

    void ApplyClanJoinResult(ClanJoinResultNetMsg result)
    {
        ClanPacketServices.ApplyJoinResult(result.Response);
    }

    void ApplyPartyAskJoin(PartyAskJoinNetMsg ask)
    {
        var dto = AskJoinPartyDto.FromNet(ask.RequestorObjectId, ask.RequestorName ?? "", ask.ItemDistribution);
        if (_party != null) _party.ApplyAskJoinParty(dto);
        else AskJoinPartyApplyIncomingPacketServices.ApplyAskJoinParty(dto);
    }

    void ApplyPartyJoinResult(PartyJoinResultNetMsg result)
    {
        if (_party != null) _party.ApplyPartyJoinResult(result.Response);
        else PartyJoinResultApplyIncomingPacketServices.ApplyPartyJoinResult(result.Response);
    }

    void ApplyPartyMemberList(PartyMemberListNetMsg list)
    {
        PartyMemberEntryNet[] members = list.Members ?? System.Array.Empty<PartyMemberEntryNet>();
        var rows = new NetPartyMemberEntry[members.Length];
        for (int i = 0; i < members.Length; i++)
        {
            PartyMemberEntryNet row = members[i] ?? new PartyMemberEntryNet();
            rows[i] = new NetPartyMemberEntry
            {
                ObjectId = row.ObjectId,
                Name = row.Name ?? "",
                ClassId = row.ClassId,
                Level = row.Level,
                CurHp = row.CurHp,
                MaxHp = row.MaxHp,
                CurMp = row.CurMp,
                MaxMp = row.MaxMp
            };
        }

        var roster = new NetPartyMemberList
        {
            LeaderObjectId = list.LeaderObjectId,
            PartyId = list.PartyId,
            Members = rows
        };
        if (_party != null) _party.ApplyPartyMemberList(roster);
        else PartyMemberListApplyIncomingPacketServices.ApplyPartyMemberList(roster);
    }

    void ApplyTradeAsk(TradeAskNetMsg ask)
    {
        SendTradeRequestDto dto = SendTradeRequestDto.FromNet(ask.RequestorObjectId, ask.RequestorName ?? "");
        if (_tradeRequest != null) _tradeRequest.ApplySendTradeRequest(dto);
        else SendTradeRequestApplyIncomingPacketServices.ApplySendTradeRequest(dto);
    }

    void ApplyTradeRequestResult(TradeRequestResultNetMsg result)
    {
        if (result.Response == 0)
            NetChatFeed.NoteSystem("Trade request declined.");
    }

    void ApplyTradeStart(TradeStartNetMsg start)
    {
        TradeStartDto dto = TradeStartDto.FromNet(start.PartnerObjectId, start.PartnerName ?? "");
        if (_trade != null) _trade.ApplyStart(dto);
        else TradeStartApplyIncomingPacketServices.ApplyTradeStart(dto);
    }

    void ApplyTradeOfferUpdate(TradeOfferUpdateNetMsg update)
    {
        int count = (int)update.Count;
        ItemInstance item = new ItemInstance(
            update.InventoryObjectId,
            update.ItemId,
            ItemLocation.Trade,
            0,
            count < 1 ? 1 : count,
            ItemCategory.Item,
            false,
            ItemSlot.none,
            update.EnchantLevel,
            9999);
        if (update.IsOwn != 0)
            if (_trade != null) _trade.ApplyOwnAdd(item);
            else TradeOwnAddApplyIncomingPacketServices.ApplyOwnAdd(item);
        else
            if (_trade != null) _trade.ApplyOtherAdd(item);
            else TradeOtherAddApplyIncomingPacketServices.ApplyOtherAdd(item);
    }

    void ApplyTradeConfirmState(TradeConfirmStateNetMsg state)
    {
        if (_trade != null) _trade.ApplyPressOwnOk(state.SelfConfirmed != 0);
        else TradePressOwnOkApplyIncomingPacketServices.ApplyPressOwnOk(state.SelfConfirmed != 0);
        if (_trade != null) _trade.ApplyPressOtherOk(state.PartnerConfirmed != 0);
        else TradePressOtherOkApplyIncomingPacketServices.ApplyPressOtherOk(state.PartnerConfirmed != 0);
    }

    void ApplyTradeFinished(TradeFinishedNetMsg _)
    {
        if (_trade != null) _trade.ApplyDone();
        else TradeDoneApplyIncomingPacketServices.ApplyTradeDone();
    }

    static NetItemEntry[] MapWarehouseItems(ItemListEntryNet[] items)
    {
        if (items == null || items.Length == 0)
            return System.Array.Empty<NetItemEntry>();
        var rows = new NetItemEntry[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            rows[i] = new NetItemEntry
            {
                ObjectId = items[i].ObjectId,
                ItemId = items[i].ItemId,
                Count = items[i].Count,
                EquippedSlot = items[i].EquippedSlot,
                EnchantLevel = items[i].EnchantLevel,
                BodySlot = "none",
                Kind = "etc",
                Icon = "",
                WeaponType = "",
                ArmorType = ""
            };
        }

        return rows;
    }

    void ApplyPlayerStats(PlayerStatsMsg msg)
    {
        // Prefer live entity; if world tile not loaded yet, patch stashed PlayerInfo for spawn.
        PlayerEntity local = DomainPacketServices.LocalPlayer;
        PlayerStats stats = local != null ? local.Stats as PlayerStats : null;
        PlayerStatus status = local != null ? local.Status as PlayerStatus : null;

        IZoneTransport transport = ZoneCommands.Transport;
        if (stats == null && transport != null && transport.PlayerInfo.Stats != null)
            stats = transport.PlayerInfo.Stats;
        if (status == null && transport != null && transport.PlayerInfo.Status != null)
            status = transport.PlayerInfo.Status;

        if (stats == null)
            return;

        stats.Level = msg.Level;
        stats.Exp = msg.Exp;
        stats.Sp = msg.Sp;
        stats.MaxHp = msg.MaxHp;
        stats.MaxMp = msg.MaxMp;
        stats.MaxCp = msg.MaxCp;
        stats.Cp = msg.Cp;
        if (status != null)
        {
            status.SetHp(msg.Hp);
            status.SetMp(msg.Mp);
            status.Cp = msg.Cp;
        }
    }

    void ApplyUserInfo(UserInfoNetMsg msg)
    {
        // The zone server sends UserInfo right after the self EntitySpawn, while the
        // world scene is still loading. NoteUserInfo stashes it and overlays the live
        // player / stashed PlayerInfo as those appear, so the paperdoll and stats are
        // not lost to the load order.
        NetUserInfo user = MapUserInfo(msg);
        NetGearSession.NoteUserInfo(user);
    }

    void ApplyItemList(ItemListNetMsg list)
    {
        NetItemEntry[] rows = MapItemList(list);
        NetGearSession.NoteItemList(rows);
        Debug.Log("[Net] ItemList count=" + rows.Length);
    }

    void ApplySkillList(SkillListNetMsg list)
    {
        NetSkillEntry[] rows = MapSkillList(list);
        SkillListDto dto = MapSkillListToDto(rows);
        if (_skills != null) _skills.ApplySkillList(dto);
        else SkillListApplyIncomingPacketServices.ApplySkillList(dto);
        Debug.Log("[Net] SkillList count=" + rows.Length);
    }

    /// <summary>
    /// <c>UserInfo</c> wire → <see cref="NetUserInfo"/> for <see cref="NetGearSession.NoteUserInfo"/>.
    ///
    /// <para>
    /// Every flag is set from the message rather than guessed: the zone server always
    /// populates vitals, stats and clan (0 meaning "none"), and an empty paperdoll is a
    /// legitimate answer for an unequipped character, not a missing tail.
    /// </para>
    /// </summary>
    static NetUserInfo MapUserInfo(UserInfoNetMsg msg)
    {
        var user = new NetUserInfo
        {
            ObjectId = (int)msg.EntityValue,
            Name = msg.Name,
            ClassId = msg.ClassId,
            Race = msg.Race,
            Sex = msg.Sex,
            X = msg.X,
            Y = msg.Y,
            Z = msg.Z,
            RunSpeed = msg.RunSpeed,
            WalkSpeed = msg.WalkSpeed,
            Running = msg.Running,
            HasClan = true,
            ClanId = msg.ClanId,
            HasVitals = true,
            Level = msg.Level,
            CurHp = msg.CurHp,
            MaxHp = msg.MaxHp,
            CurMp = msg.CurMp,
            MaxMp = msg.MaxMp,
            CurCp = msg.CurCp,
            MaxCp = msg.MaxCp,
            Exp = msg.Exp,
            Sp = msg.Sp,
            HasStats = true,
            Str = msg.Str,
            Dex = msg.Dex,
            Con = msg.Con,
            Int = msg.Int,
            Wit = msg.Wit,
            Men = msg.Men,
            PAtk = msg.PAtk,
            MAtk = msg.MAtk,
            PDef = msg.PDef,
            MDef = msg.MDef,
            PAtkSpd = msg.PAtkSpd,
            MAtkSpd = msg.MAtkSpd,
            Accuracy = msg.Accuracy,
            CritRate = msg.CritRate
        };

        PaperdollSlotNet[] slots = msg.Paperdoll;
        if (slots == null || slots.Length == 0)
        {
            user.HasPaperdoll = false;
            user.Paperdoll = System.Array.Empty<NetPaperdollSlot>();
            return user;
        }

        var paperdoll = new NetPaperdollSlot[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            PaperdollSlotNet src = slots[i];
            paperdoll[i] = new NetPaperdollSlot
            {
                ObjectId = src?.ObjectId ?? 0,
                ItemId = src?.ItemId ?? 0,
                EnchantLevel = src?.EnchantLevel ?? 0
            };
        }

        user.HasPaperdoll = true;
        user.Paperdoll = paperdoll;
        return user;
    }


    static NetItemEntry[] MapItemList(ItemListNetMsg list)
    {
        if (list?.Items == null || list.Items.Length == 0)
            return System.Array.Empty<NetItemEntry>();
        var rows = new NetItemEntry[list.Items.Length];
        for (int i = 0; i < list.Items.Length; i++)
        {
            ItemListEntryNet src = list.Items[i];
            rows[i] = new NetItemEntry
            {
                ObjectId = src.ObjectId,
                ItemId = src.ItemId,
                Count = src.Count,
                EquippedSlot = src.EquippedSlot,
                EnchantLevel = src.EnchantLevel,
                BodySlot = src.BodySlot ?? "",
                Kind = src.Kind ?? "",
                Icon = src.Icon ?? "",
                WeaponType = src.WeaponType ?? "",
                ArmorType = src.ArmorType ?? ""
            };
        }

        return rows;
    }

    static NetSkillEntry[] MapSkillList(SkillListNetMsg list)
    {
        if (list?.Skills == null || list.Skills.Length == 0)
            return System.Array.Empty<NetSkillEntry>();
        var rows = new NetSkillEntry[list.Skills.Length];
        for (int i = 0; i < list.Skills.Length; i++)
        {
            SkillListEntryNet src = list.Skills[i];
            rows[i] = new NetSkillEntry
            {
                SkillId = src.SkillId,
                Level = src.Level,
                ReuseDelayMs = src.ReuseRemainingMs
            };
        }

        return rows;
    }

    static SkillListDto MapSkillListToDto(NetSkillEntry[] rows)
    {
        var dto = new SkillListDto();
        if (rows == null)
            return dto;
        for (int i = 0; i < rows.Length; i++)
        {
            NetSkillEntry row = rows[i];
            bool passive = IsPassiveSkill(row.SkillId, row.Level);
            bool disabled = row.ReuseDelayMs > 0;
            dto.Skills.Add(new SkillInstance(row.SkillId, row.Level, passive, disabled));
        }

        return dto;
    }

    static bool IsPassiveSkill(int skillId, int level)
    {
        Skillgrp skill = TablePacketServices.SkillGrps != null ? TablePacketServices.SkillGrps.GetSkill(skillId, level) : null;
        return skill != null && skill.OperateType == 2;
    }

    void ApplyMoveState(MoveStateMsg move)
    {
        int objectId = (int)move.EntityValue;
        Vector3 dest = VectorUtils.ConvertPosToUnity(new Vector3(move.DestX, move.DestY, move.DestZ));
        Vector3 origin = VectorUtils.ConvertPosToUnity(new Vector3(move.OriginX, move.OriginY, move.OriginZ));
        var domain = new DomainMoveTo
        {
            ObjectId = objectId,
            Destination = dest,
            Origin = origin,
            // The intention layer (NewMoveToIntention) reads the Interlude DTO and bails out when it is
            // null, so the local player stayed put even though MoveState arrived. Synthesize it here.
            InterludeDto = CharMoveToLocationDto.FromUnity(objectId, dest, origin)
        };
        if (_sink != null)
            _sink.ApplyMoveTo(domain);
        else if (WorldApplyPacketServices.WorldApply != null)
            WorldApplyPacketServices.WorldApply.MoveTo(objectId, dest, origin, domain.InterludeDto);
    }

    void ApplyCorrection(PositionCorrectionMsg corr)
    {
        int objectId = (int)corr.EntityValue;
        Vector3 pos = VectorUtils.ConvertPosToUnity(new Vector3(corr.X, corr.Y, corr.Z));
        // Domain apply — never INTENTION_MOVE_TO with dest==origin (fights approach MoveState).
        PostCritical(() =>
        {
            if (_sink != null)
                _sink.ApplyPositionCorrection(objectId, pos);
            else if (WorldApplyPacketServices.WorldApply != null)
                WorldApplyPacketServices.WorldApply.ApplyPositionCorrection(objectId, pos);
        });
    }
}
