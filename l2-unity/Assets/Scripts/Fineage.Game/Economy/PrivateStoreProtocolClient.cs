/// <summary>Mobius <c>PrivateStoreType</c> bytes (matches Fineage.Protocol.PrivateStoreProtocol).</summary>
public static class PrivateStoreProtocolClient
{
    public const byte TypeNone = 0;
    public const byte TypeSell = 1;
    public const byte TypeBuy = 2;
    public const byte TypePackageSell = 3;
    public const byte TypeManufacture = 5;

    public static bool IsOpen(byte storeType) => storeType != TypeNone;

    public static bool IsManufacture(byte storeType) => storeType == TypeManufacture;
}