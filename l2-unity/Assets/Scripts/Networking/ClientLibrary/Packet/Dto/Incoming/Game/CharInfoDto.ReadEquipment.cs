using UnityEngine;

public partial class CharInfoDto
{
    private void ReadEquipment(PacketReader reader)
    {
        reader.ReadI();
        Stats.Karma = reader.ReadI();
        Stats.MAtkSpd = reader.ReadI();
        Stats.PAtkSpd = reader.ReadI();
        reader.ReadI();
        reader.ReadI();

        Stats.BaseRunSpeed = reader.ReadI();
        Stats.BaseWalkingSpeed = reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();

        float moveMultiplier = (float)reader.ReadD();
        reader.ReadD();

        Stats.Speed = Stats.BaseRunSpeed;
        Stats.WalkRealSpeed = GetRealSpeed(Stats.BaseWalkingSpeed, moveMultiplier);
        Stats.RunRealSpeed = GetRealSpeed(Stats.BaseRunSpeed, moveMultiplier);

        Appearance.CollisionRadius = (float)reader.ReadD();
        Appearance.CollisionHeight = (float)reader.ReadD();

        Appearance.HairStyle = ClampByte(reader.ReadI());
        Appearance.HairColor = ClampByte(reader.ReadI());
        Appearance.Face = ClampByte(reader.ReadI());

        Identity.Title = reader.ReadOtherS();
        Identity.ClanId = reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();
        reader.ReadI();

        reader.ReadB();
        bool running = reader.ReadB() == 1;
        Identity.IsRunning = running;
        Appearance.Running = running;
        reader.ReadB();
        AlikeDead = reader.ReadB() == 1;
        reader.ReadB();
        reader.ReadB();
        reader.ReadB();

        int cubicCount = reader.ReadH();
        for (int i = 0; i < cubicCount; i++)
            reader.ReadH();

        reader.ReadB();
        reader.ReadI();
        reader.ReadB();
        reader.ReadH();

        PacketClassSecond = reader.ReadI();
        Identity.PlayerClass = PacketClassSecond;

        Stats.MaxCp = reader.ReadI();
        Status.Cp = reader.ReadI();

    }
}
