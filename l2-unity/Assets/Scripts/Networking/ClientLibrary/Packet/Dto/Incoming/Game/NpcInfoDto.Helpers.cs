using UnityEngine;

public partial class NpcInfoDto : IWireDto
{
    public void ReadPet(PacketReader reader)
    {
        reader.ReadI();
        Identity.Id = reader.ReadI();
        Identity.NpcId = reader.ReadI() - 1000000;
        Identity.Owned = true;
        Identity.SetHideHp(Identity.NpcId);
        reader.ReadI();

        int x = reader.ReadI();
        int y = reader.ReadI();
        int z = reader.ReadI();
        Identity.SetL2jPos(x, y, z);
        float heading = reader.ReadI();
        Identity.OrigHeading = VectorUtils.HeadingToUnityQuaternionForNpc(heading);
        Identity.Heading = Quaternion.Euler(0, Identity.OrigHeading, 0);
        reader.ReadI();

        Stats.MAtkSpd = reader.ReadI();
        Stats.PAtkSpd = reader.ReadI();
        Stats.BaseRunSpeed = reader.ReadI();
        Stats.BaseWalkingSpeed = reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();

        double moveMultiplier = reader.ReadD();
        Stats.WalkRealSpeed = GetRealSpeed(Stats.BaseWalkingSpeed, (float)moveMultiplier);
        Stats.RunRealSpeed = GetRealSpeed(Stats.BaseRunSpeed, (float)moveMultiplier);
        reader.ReadD();
        Appearance.CollisionRadius = (float)reader.ReadD();
        Appearance.CollisionHeight = (float)reader.ReadD();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadB();
        reader.ReadB();
        reader.ReadB();
        reader.ReadB();
        Identity.SummonState = reader.ReadB();
        Identity.IsRunning = true;
        Identity.Name = reader.ReadOtherS();
        Identity.Title = reader.ReadOtherS();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        int hp = reader.ReadI();
        Stats.MaxHp = reader.ReadI();
        int mp = reader.ReadI();
        Stats.MaxMp = reader.ReadI();
        Status.SetHp(hp);
        Status.SetMp(mp);
        reader.ReadI();
        Stats.Level = reader.ReadI();
    }

    private float GetRealSpeed(int baseSpeed, float speedMultiplier)
    {
        return baseSpeed * speedMultiplier;
    }
}
