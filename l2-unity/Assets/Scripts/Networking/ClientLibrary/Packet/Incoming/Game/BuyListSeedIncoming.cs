using UnityEngine;

[IncomingGamePacket(GameServerPacketType.BuyListSeed)]
public sealed class BuyListSeedIncoming : IncomingWirePacket<BuyListSeedDto>
{
    public override void Apply(BuyListSeedDto packet)
    {
        GameIncomingPacketServices.Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetWindowName("Estate");
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sale");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Purchase");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.BUY_SEED);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.List, true, packet.ManorId);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindow();
        });
    }
}
