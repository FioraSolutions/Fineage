using UnityEngine;

public partial class UserInfoDto
{
    private void ReadIdentityAndVitals(PacketReader reader)
    {
        int x = reader.ReadI();
        int y = reader.ReadI();
        int z = reader.ReadI();

        //Vector3 unityPos = VectorUtils.ConvertPosToUnity(new Vector3(x,y,z));
        _info.Identity.SetL2jPos(x, y, z);
        float heading = reader.ReadI();
        _info.Identity.OrigHeading = VectorUtils.HeadingToUnityQuaternionForNpc(heading);
        _info.Identity.Heading = Quaternion.Euler(0, _info.Identity.OrigHeading, 0); 
        _info.Identity.Id = reader.ReadI();
     
        _info.Identity.Name = reader.ReadOtherS();
        int reace = reader.ReadI();
        int female = reader.ReadI();
        _info.Appearance.Sex = female;
        _info.Appearance.Race = (int)MapClassId.GetRace(reace);
        _info.Appearance.BaseClass = reader.ReadI();
        _info.Stats.Level = reader.ReadI();
         long exp =  reader.ReadLOther();
        int ost = (int)exp - (int)_info.Stats.Exp;
       // if (ost > 0) StorageVariable.getInstance().AddS1Items(new VariableItem(ost.ToString(), _info.Identity.Id));
        _info.Stats.Exp = exp;
        _info.Stats.MaxExp  = LevelServer.GetExp(_info.Stats.Level + 1);
        _info.Stats.Str = reader.ReadI();
        _info.Stats.Dex = reader.ReadI();
        _info.Stats.Con = reader.ReadI();
        _info.Stats.Int = reader.ReadI();
        _info.Stats.Wit = reader.ReadI();
        _info.Stats.Men = reader.ReadI();

        _info.Stats.MaxHp = reader.ReadI();
        _info.Status.SetHp(reader.ReadI());
        _info.Stats.MaxMp = reader.ReadI();
        _info.Status.SetMp(reader.ReadI());
        int sp = reader.ReadI();
        int oldSp = (int)_info.Stats.OldSp;
        int ostSp = (int)sp - oldSp;
        //StorageVariable.getInstance().AddS2Items(new VariableItem(ostSp.ToString(), _info.Identity.Id));
        _info.Stats.OldSp = sp;
        _info.Stats.Sp = sp;
        _info.Stats.CurrWeight = reader.ReadI();
        _info.Stats.MaxWeight = reader.ReadI();  //the max weight that the Creature can load.
        int activeWeaponItem = reader.ReadI(); // 20 no weapon, 40 weapon equipped

        /**
 * Returns the objectID associated to the item in the paperdoll slot
 * @param slot : int pointing out the slot
 * @return int designating the objectID
 */
        var paperTest = _info.Appearance.PaperDoll;
        _info.Appearance.PaperDoll.Obj_Under = reader.ReadI();
        _info.Appearance.PaperDoll.Obj_Pear = reader.ReadI();

        _info.Appearance.PaperDoll.Obj_Lear = reader.ReadI();
        _info.Appearance.PaperDoll.Obj_Neck = reader.ReadI();

        _info.Appearance.PaperDoll.Obj_RFinger = reader.ReadI();
        _info.Appearance.PaperDoll.Obj_LFinger = reader.ReadI();

        _info.Appearance.PaperDoll.Obj_Head = reader.ReadI();
        _info.Appearance.PaperDoll.Obj_RHand = reader.ReadI();

        _info.Appearance.PaperDoll.Obj_LHand = reader.ReadI();
        _info.Appearance.PaperDoll.Obj_Gloves = reader.ReadI();

        _info.Appearance.PaperDoll.Obj_Chest = reader.ReadI();
        _info.Appearance.PaperDoll.Obj_Legs = reader.ReadI();

        _info.Appearance.PaperDoll.Obj_Feet = reader.ReadI();
        _info.Appearance.PaperDoll.Obj_Cloak = reader.ReadI();

        _info.Appearance.PaperDoll.Obj_RHand = reader.ReadI();
        _info.Appearance.PaperDoll.Obj_Hair = reader.ReadI();

        _info.Appearance.PaperDoll.Obj_Face = reader.ReadI();


        /**
 * Returns the ID of the item in the paperdoll slot
 * @param slot : int designating the slot
 * @return int designating the ID of the item
 */     
        _info.Appearance.PaperDoll.Item_Under = reader.ReadI();
        _info.Appearance.PaperDoll.Item_Rear = reader.ReadI();

        _info.Appearance.PaperDoll.Item_Lear = reader.ReadI();
        _info.Appearance.PaperDoll.Item_Neck = reader.ReadI();

        _info.Appearance.PaperDoll.Item_RFinger = reader.ReadI();
        _info.Appearance.PaperDoll.Item_LFinger = reader.ReadI();

        _info.Appearance.PaperDoll.Item_Head = reader.ReadI();
        _info.Appearance.PaperDoll.Item_RHand = reader.ReadI();

        _info.Appearance.PaperDoll.Item_LHand = reader.ReadI();
        _info.Appearance.PaperDoll.Item_Gloves = reader.ReadI();
        _info.Appearance.Gloves = _info.Appearance.PaperDoll.Item_Gloves;

        _info.Appearance.PaperDoll.Item_Chest = reader.ReadI();
        _info.Appearance.Chest = _info.Appearance.PaperDoll.Item_Chest;

        _info.Appearance.PaperDoll.Item_Legs = reader.ReadI();
        _info.Appearance.Legs = _info.Appearance.PaperDoll.Item_Legs;

        _info.Appearance.PaperDoll.Item_Feet = reader.ReadI();
        _info.Appearance.Feet = _info.Appearance.PaperDoll.Item_Feet;
        _info.Appearance.PaperDoll.Item_Cloak = reader.ReadI();

        _info.Appearance.PaperDoll.Item_RHand = reader.ReadI();
        _info.Appearance.PaperDoll.Item_Hair = reader.ReadI();
        _info.Appearance.PaperDoll.Item_Face = reader.ReadI();

        _info.Appearance.RHand = _info.Appearance.PaperDoll.Item_RHand;
        _info.Appearance.LHand = _info.Appearance.PaperDoll.Item_LHand;
    }
}
