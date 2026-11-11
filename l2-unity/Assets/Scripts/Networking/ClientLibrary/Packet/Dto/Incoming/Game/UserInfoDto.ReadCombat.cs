
using UnityEngine;

public partial class UserInfoDto
{
    private void ReadPaperDollAndCombat(PacketReader reader)
    {

        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        //buffer.writeInt(_player.getInventory().getPaperdollAugmentationId(Inventory.PAPERDOLL_RHAND));
        int rhandAugmentationId = reader.ReadI();

        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        // buffer.writeInt(_player.getInventory().getPaperdollAugmentationId(Inventory.PAPERDOLL_RHAND));
        int rhandAugmentationId2 = reader.ReadI();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        reader.ReadSh();
        
        _info.Stats.PAtk = reader.ReadI();
        int atackspped = reader.ReadI();
        _info.Stats.BasePAtkSpeed = atackspped;
        //_info.Stats.PAtkSpd = atackspped;
        // Debug.Log("PAAAAAAAAAAAATACK SPEED " + atackspped);
        _info.Stats.PAtkSpd = atackspped;
        _info.Stats.PDef = reader.ReadI();
        _info.Stats.PEvasion = reader.ReadI();
        // int accuracy = reader.ReadI();
        _info.Stats.MAccuracy = reader.ReadI();
        int criticalHit = reader.ReadI();
        _info.Stats.MAtk = reader.ReadI();
        _info.Stats.MAtkSpd = reader.ReadI();
        int pAttackSpd2 = reader.ReadI();
        _info.Stats.MDef = reader.ReadI();
        int pvpFlag = reader.ReadI();
        _info.Stats.Karma = reader.ReadI();
        int runSpeed = reader.ReadI();
        _info.Stats.Speed = runSpeed;
        //_info.Stats.WalkingSpeed = reader.ReadI();
        _info.Stats.BaseWalkingSpeed = reader.ReadI();
        _info.Stats.BaseRunSpeed = runSpeed;
        




        int swimRunSpd = reader.ReadI();
        int swimWalkSpd = reader.ReadI();
        int flyRunSpd = reader.ReadI();
        int flyWalkSpd = reader.ReadI();
        int flyRunSpd2 = reader.ReadI();
        int flyWalkSpd2 = reader.ReadI();
        double moveMultiplier = reader.ReadD();
        double attackSpeedMultiplier = reader.ReadD();

        _info.Stats.WalkRealSpeed = GetRealSpeed(_info.Stats.BaseWalkingSpeed, (float) moveMultiplier);
        _info.Stats.RunRealSpeed = GetRealSpeed(_info.Stats.BaseRunSpeed, (float)moveMultiplier);
        _info.Stats.PAtkRealSpeed = GetRealSpeed(_info.Stats.PAtkSpd, (float)attackSpeedMultiplier);

        // Debug.Log("BasePatakSpeed R " + _info.Stats.PAtkRealSpeed);
        // Debug.Log("BasePatakSpeed B" + _info.Stats.BasePAtkSpeed);
        // Debug.Log("BasePatakSpeed Spd" + attackSpeedMultiplier);


        _info.Appearance.CollisionRadius = (float)reader.ReadD();
    }
}
