using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PackageSendableList)]
public sealed class PackageSendableListIncoming : IncomingWirePacket<PackageSendableListDto>
{
    public override void Apply(PackageSendableListDto packet)
    {
        GameIncomingPacketServices.Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetWindowName("Send a parcel");
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Shipping list");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.PackageSendableList);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.Items, true, packet.PlayerObject);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindow();
        });
    }
}
