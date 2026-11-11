using UnityEngine;

public partial class CharInfoDto
{
    private void ReadSocial(PacketReader reader)
    {
        Appearance.WeaponEnchant = reader.ReadB();
        reader.ReadB();
        reader.ReadI();
        reader.ReadB();
        reader.ReadB();
        reader.ReadB();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();

        float heading = reader.ReadI();
        Identity.OrigHeading = VectorUtils.HeadingToUnityQuaternionForNpc(heading);
        Identity.Heading = Quaternion.Euler(0, Identity.OrigHeading, 0);

        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();

        ApplyStubHpFromAlikeDead();

        // Debug.Log("[EntityAction:User] nick=" + Identity.Name +
            // " CharInfo parse id=" + Identity.Id +
            // " alikeDead=" + AlikeDead +
            // " stubHp=" + Status.GetHp() +
            // " maxHp=" + Stats.MaxHp +
            // " cp=" + Status.Cp +
            // " maxCp=" + Stats.MaxCp +
            // " " + GearFlowLog.Paperdoll(Appearance) +
            // " rHand1=" + rHand1 + " rHand2=" + rHand2 + " cloak=" + cloak);

        // Debug.Log("[SPD] PKT OnCharInfo parse nick=" + Identity.Name +
            // " id=" + Identity.Id +
            // " runBase=" + Stats.BaseRunSpeed +
            // " walkBase=" + Stats.BaseWalkingSpeed +
            // " moveMul=" + moveMultiplier.ToString("F5") +
            // " runReal=" + Stats.RunRealSpeed.ToString("F3") +
            // " walkReal=" + Stats.WalkRealSpeed.ToString("F3") +
            // " groundFromMul=" + (Stats.BaseRunSpeed * moveMultiplier).ToString("F3") +
            // " unityRun=" + NumberUtils.ScaleToUnity(Stats.RunRealSpeed).ToString("F4") +
            // " l2RunMs=" + (Stats.RunRealSpeed / 52.5f).ToString("F4") +
            // " running=" + (running ? 1 : 0) +
            // " locL2=(" + x + "," + y + "," + z + ")");

        int baseBefore = Appearance.BaseClass;
        bool mageByFirst = CharacterClassParser.IsMage(PacketClassFirst);
        bool mageBySecond = CharacterClassParser.IsMage(Identity.PlayerClass);
        if (Appearance.BaseClass != (int)BaseClass.Fighter && Appearance.BaseClass != (int)BaseClass.MMagic)
        {
            Appearance.BaseClass = mageBySecond
                ? (int)BaseClass.MMagic
                : (int)BaseClass.Fighter;
        }

        // Debug.Log("[CHARINFO_CLASS] nick=" + Identity.Name +
            // " id=" + Identity.Id +
            // " raceRaw=" + PacketRace +
            // " raceMapped=" + Appearance.Race +
            // " sex=" + Appearance.Sex +
            // " classFirst=" + PacketClassFirst +
            // " classSecond=" + PacketClassSecond +
            // " mageFirst=" + mageByFirst +
            // " mageSecond=" + mageBySecond +
            // " baseBefore=" + baseBefore +
            // " baseAfter=" + Appearance.BaseClass +
            // " chest=" + Appearance.Chest +
            // " rhand=" + Appearance.RHand);
    }
}
