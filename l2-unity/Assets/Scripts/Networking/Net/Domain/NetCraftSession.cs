/// <summary>Client craft/manufacture context carried across Net apply (book tab, active shop).</summary>
public static class NetCraftSession
{
    /// <summary>Last RecipeBookList dwarven flag (1 = dwarven, 0 = common).</summary>
    public static int LastBookDwarven { get; set; }
    public static int LastCurMp { get; set; }
    public static int LastMaxMp { get; set; }
    public static int PendingMakeSelfRecipeId { get; set; }

    public static int ActiveManufacturerObjectId { get; set; }
    public static long ActiveShopPrice { get; set; }

    public static void ClearShopContext()
    {
        ActiveManufacturerObjectId = 0;
        ActiveShopPrice = 0;
    }

    public static void UpdateMpFromPlayer(int curMp, int maxMp)
    {
        LastCurMp = curMp;
        LastMaxMp = maxMp;
    }

    public static void Reset()
    {
        LastBookDwarven = 0;
        LastCurMp = 0;
        LastMaxMp = 0;
        PendingMakeSelfRecipeId = 0;
        ClearShopContext();
    }
}