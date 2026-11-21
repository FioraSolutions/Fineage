/// <summary>
/// How dealer window panels exchange rows. Interlude NPC buy keeps catalog stock;
/// sell / warehouse / trade consumes the player's bag rows.
/// </summary>
public enum ShopTransferMode
{
    NpcMerchantBuy,
    PlayerBagCommit,
}