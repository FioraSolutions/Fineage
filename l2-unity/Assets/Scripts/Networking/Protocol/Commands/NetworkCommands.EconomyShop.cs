using System.Collections.Generic;
using UnityEngine;

public sealed class MultiSellChooseCommand : INetworkCommand
{
    public int ListId;
    public int EntryId;
    public int Amount;

    public MultiSellChooseCommand(int listId, int entryId, int amount)
    {
        ListId = listId;
        EntryId = entryId;
        Amount = amount;
    }
}

public sealed class RequestBuyItemCommand : INetworkCommand
{
    public int ListId;
    public List<Product> BuyList;

    public RequestBuyItemCommand(int listId, List<Product> buyList)
    {
        ListId = listId;
        BuyList = buyList;
    }
}

public sealed class RequestSellItemCommand : INetworkCommand
{
    public int ListId;
    public List<Product> SellList;

    public RequestSellItemCommand(int listId, List<Product> sellList)
    {
        ListId = listId;
        SellList = sellList;
    }
}

public sealed class SendWarehouseDepositListCommand : INetworkCommand
{
    public List<Product> SellList;

    public SendWarehouseDepositListCommand(List<Product> sellList)
    {
        SellList = sellList;
    }
}

public sealed class SendWarehouseWithdrawListCommand : INetworkCommand
{
    public List<Product> SellList;

    public SendWarehouseWithdrawListCommand(List<Product> sellList)
    {
        SellList = sellList;
    }
}

public sealed class RequestPackageSendCommand : INetworkCommand
{
    public int ObjectId;
    public List<Product> BuyList;

    public RequestPackageSendCommand(int objectId, List<Product> buyList)
    {
        ObjectId = objectId;
        BuyList = buyList;
    }
}

public sealed class RequestPackageSendableItemListCommand : INetworkCommand
{
    public int ObjectId;

    public RequestPackageSendableItemListCommand(int objectId)
    {
        ObjectId = objectId;
    }
}

