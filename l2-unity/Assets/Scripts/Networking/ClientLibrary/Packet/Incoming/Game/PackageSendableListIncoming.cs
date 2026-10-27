using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PackageSendableList)]
public sealed class PackageSendableListIncoming : IncomingWirePacket<PackageSendableListDto>
{
    public override void Apply(PackageSendableListDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            IncomingPacketServices.Dealer.SetWindowName("Send a parcel");
            IncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            IncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Shipping list");
            IncomingPacketServices.Dealer.SetProductType(ProductType.PackageSendableList);
            IncomingPacketServices.Dealer.UpdateBuyData(packet.Items, true, packet.PlayerObject);
            IncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            IncomingPacketServices.Dealer.ShowWindow();
        });
    }
}
