
using UnityEngine;

public partial class CharInfoDto
{
    private void ReadIdentity(PacketReader reader)
    {
        int x = reader.ReadI();
        int y = reader.ReadI();
        int z = reader.ReadI();
        Identity.SetL2jPos(x, y, z);

        reader.ReadI();

        Identity.Id = reader.ReadI();
        Identity.Name = reader.ReadOtherS();

        int race = reader.ReadI();
        PacketRace = race;
        Appearance.Race = (int)MapClassId.GetRace(race);
        Appearance.Sex = reader.ReadI();

        int classId = reader.ReadI();
        PacketClassFirst = classId;
        Appearance.BaseClass = classId;
        Identity.PlayerClass = classId;

        int hairAll = reader.ReadI();
        int head = reader.ReadI();
        int rHand1 = reader.ReadI();
        Appearance.RHand = rHand1;
        Appearance.LHand = reader.ReadI();
        Appearance.Gloves = reader.ReadI();
        Appearance.Chest = reader.ReadI();
        Appearance.Legs = reader.ReadI();
        Appearance.Feet = reader.ReadI();
        int cloak = reader.ReadI();
        int rHand2 = reader.ReadI();
        Appearance.RHand = rHand2;
        int hairItem = reader.ReadI();
        int faceItem = reader.ReadI();
        Appearance.PaperDoll.Item_Under = hairAll;
        Appearance.PaperDoll.Item_Head = head;
        Appearance.PaperDoll.Item_RHand = rHand2;
        Appearance.PaperDoll.Item_LHand = Appearance.LHand;
        Appearance.PaperDoll.Item_Gloves = Appearance.Gloves;
        Appearance.PaperDoll.Item_Chest = Appearance.Chest;
        Appearance.PaperDoll.Item_Legs = Appearance.Legs;
        Appearance.PaperDoll.Item_Feet = Appearance.Feet;
        Appearance.PaperDoll.Item_Cloak = cloak;
        Appearance.PaperDoll.Item_Hair = hairItem;
        Appearance.PaperDoll.Item_Face = faceItem;
        Appearance.PaperdollReady = true;

        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadI();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadI();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();
        reader.ReadH();

    }
}
