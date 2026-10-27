using UnityEngine;

[IncomingGamePacket(GameServerPacketType.BuyListSeed)]
public sealed class BuyListSeedIncoming : IncomingWirePacket<BuyListSeedDto>
{
    public override void Apply(BuyListSeedDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            IncomingPacketServices.Dealer.SetWindowName("Estate");
            IncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sale");
            IncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Purchase");
            IncomingPacketServices.Dealer.SetProductType(ProductType.BUY_SEED);
            IncomingPacketServices.Dealer.UpdateBuyData(packet.List, true, packet.ManorId);
            IncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            IncomingPacketServices.Dealer.ShowWindow();
        });
    }
}
