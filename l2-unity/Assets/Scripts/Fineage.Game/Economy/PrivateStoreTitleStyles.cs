using UnityEngine;

/// <summary>Interlude head title colors for private store / manufacture messages (ARGB hex).</summary>
public static class PrivateStoreTitleStyles
{
    public const string Sell = "FF7700FF";
    public const string Buy = "5599FFFF";
    public const string Manufacture = "799BB0FF";

    public static string ColorForStoreType(int storeType)
    {
        return storeType switch
        {
            PrivateStoreProtocolClient.TypeSell => Sell,
            PrivateStoreProtocolClient.TypePackageSell => Sell,
            PrivateStoreProtocolClient.TypeBuy => Buy,
            PrivateStoreProtocolClient.TypeManufacture => Manufacture,
            _ => ""
        };
    }
}