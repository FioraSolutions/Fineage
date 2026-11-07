using Fineage.Net.Contract;
using UnityEngine;

/// <summary>Fineage Net UserInfo wire → player vitals / paperdoll overlay.</summary>
public static class NetGearMap
{
    /// <summary>
    /// Copies UserInfo class, speeds, paperdoll, vitals, and template stats onto a spawn record.
    /// Position stays on the spawn packet.
    /// </summary>
    public static void OverlayUserInfo(PlayerInfoInterlude info, NetUserInfo user)
    {
        if (info.Identity == null || info.Appearance == null || info.Stats == null)
            return;

        info.Identity.PlayerClass = user.ClassId;
        info.Appearance.Race = (int)MapClassId.GetCharacterRace(user.ClassId);
        bool mage = CharacterClassParser.IsMage((CharacterClass)user.ClassId);
        info.Appearance.BaseClass = mage ? (int)BaseClass.MMagic : (int)BaseClass.Fighter;
        if (!string.IsNullOrEmpty(user.Name))
            info.Identity.Name = user.Name;
        if (user.RunSpeed > 0.001f)
        {
            int run = Mathf.RoundToInt(user.RunSpeed);
            info.Stats.BaseRunSpeed = run;
            info.Stats.RunRealSpeed = user.RunSpeed;
            info.Stats.Speed = run;
        }

        if (user.WalkSpeed > 0.001f)
        {
            info.Stats.BaseWalkingSpeed = Mathf.RoundToInt(user.WalkSpeed);
            info.Stats.WalkRealSpeed = user.WalkSpeed;
        }

        info.Appearance.Running = user.Running;
        info.Identity.IsRunning = user.Running;
        if (user.HasClan)
            info.Identity.ClanId = user.ClanId;
        if (user.HasPaperdoll)
            NetEntityMap.ApplyUserPaperdoll(info.Appearance, user.Paperdoll);

        if (user.HasVitals)
        {
            if (user.Level > 0)
                info.Stats.Level = user.Level;
            if (user.MaxHp > 0)
                info.Stats.MaxHp = user.MaxHp;
            if (user.MaxMp > 0)
                info.Stats.MaxMp = user.MaxMp;
            if (user.MaxCp > 0)
                info.Stats.MaxCp = user.MaxCp;
            info.Stats.Exp = user.Exp;
            info.Stats.Sp = user.Sp;
            if (info.Status != null)
            {
                if (user.MaxHp > 0 || user.CurHp > 0)
                    info.Status.SetHp(user.CurHp);
                if (user.MaxMp > 0 || user.CurMp > 0)
                    info.Status.SetMp(user.CurMp);
                info.Status.Cp = user.CurCp;
                PlayerResourceCaps.EnsureMaxAtLeastCurrent(info.Stats, info.Status);
            }
        }

        if (user.HasStats)
        {
            info.Stats.Str = user.Str;
            info.Stats.Dex = user.Dex;
            info.Stats.Con = user.Con;
            info.Stats.Int = user.Int;
            info.Stats.Wit = user.Wit;
            info.Stats.Men = user.Men;
            info.Stats.PAtk = user.PAtk;
            info.Stats.MAtk = user.MAtk;
            info.Stats.PDef = user.PDef;
            info.Stats.MDef = user.MDef;
            info.Stats.PAtkSpd = user.PAtkSpd;
            info.Stats.MAtkSpd = user.MAtkSpd;
            info.Stats.PAccuracy = user.Accuracy;
            info.Stats.PCritical = user.CritRate;
        }
    }

    /// <summary>
    /// <c>UserInfo</c> wire → <see cref="NetUserInfo"/>, the shape
    /// <see cref="NetGearSession.NoteUserInfo"/> and <see cref="OverlayUserInfo"/> consume.
    ///
    /// <para>
    /// Every flag is set from the message rather than guessed: the zone server always
    /// populates vitals, stats and clan (0 meaning "none"), and an empty paperdoll is a
    /// legitimate answer for an unequipped character, not a missing tail.
    /// </para>
    /// </summary>
    public static NetUserInfo ToUserInfo(UserInfoNetMsg msg)
    {
        var user = new NetUserInfo
        {
            ObjectId = (int)msg.EntityValue,
            Name = msg.Name,
            ClassId = msg.ClassId,
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

}
