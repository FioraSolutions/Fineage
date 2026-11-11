using UnityEngine;

public partial class UserInfoDto
{
    private void ReadSocialAndFlags(PacketReader reader)
    {
        float collision = (float)reader.ReadD();
        _info.Appearance.CollisionHeight = collision;
        _info.Appearance.HairStyle = reader.ReadI();
        _info.Appearance.HairColor = reader.ReadI();
        _info.Appearance.Face = reader.ReadI();
        int isGm = reader.ReadI();
        _info.Identity.Title = reader.ReadOtherS();
        //_info.Identity.Title = "My title";
        _info.Identity.ClanId = reader.ReadI();
        int clanCrestId = reader.ReadI();
        int allyId = reader.ReadI();
        int allyCrestId = reader.ReadI();
        // 0x40 leader rights
        // siege flags: attacker - 0x180 sword over name, defender - 0x80 shield, 0xC0 crown (|leader), 0x1C0 flag (|leader)
        int relation = reader.ReadI();
        byte mountType = reader.ReadB();
        byte privateStoreType = reader.ReadB();
        byte hasDwarvenCraft = reader.ReadB();

        _info.Stats.PkKills = reader.ReadI();
        _info.Stats.PvpKills = reader.ReadI();
        int cubics_size = reader.ReadSh();

        for(int i=0; i < cubics_size; i++)
        {
            reader.ReadSh();
        }

        byte isInPartyMatchRoom = reader.ReadB();
        int isInvisible = reader.ReadI();
        byte isInsideZone = reader.ReadB();
        int clanPrivileges = reader.ReadI();
        int recomLeft = reader.ReadSh();
        int recomHave = reader.ReadSh();
        int mountNpcId = reader.ReadI();
        int inventoryLimit = reader.ReadSh();
        int class_id = reader.ReadI();
        _info.Identity.PlayerClass = class_id;
        int unknow = reader.ReadI();//// special effects? circles around player...
        _info.Stats.MaxCp = reader.ReadI();
        int cp = reader.ReadI();
        _info.Status.Cp = cp;
        _info.Appearance.WeaponEnchant = reader.ReadB();
        _info.Appearance.PaperdollReady = true;
        byte teamId = reader.ReadB();
        int clanCrestLargeId = reader.ReadI();
        byte isNoble = reader.ReadB();
        byte isHero = reader.ReadB();
        byte isFishing = reader.ReadB();
        int fishingX = reader.ReadI();
        int fishingY = reader.ReadI();
        int fishingZ = reader.ReadI();
        int colorName = reader.ReadI();
        byte isRunning = reader.ReadB();// changes the Speed display on Status Window
        _info.Appearance.Running = isRunning == 1;
        
        int pledgeClass = reader.ReadI();
        int PledgeType = reader.ReadI();   
        int titleColor = reader.ReadI();
        int isCursedWeaponEquipped = reader.ReadI();
    }

    private float GetRealSpeed(int baseSpeed , float speedMultiplier)
    {
       return  baseSpeed * speedMultiplier;
    }

    private float GetRealSpeed(double baseSpeed, float speedMultiplier)
    {
        return (float)baseSpeed * speedMultiplier;
    }

}
