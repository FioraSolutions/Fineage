using UnityEngine;

public sealed partial class ItemDropPrefabLoader
{
    public static bool IsFxDropPropPrefab(GameObject prefab)
    {
        if (prefab == null)
            return false;
        return prefab.name.IndexOf("etcpotion", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool IsDropItemsPrefab(GameObject prefab)
    {
        if (prefab == null)
            return false;
        string name = prefab.name;
        if (name.IndexOf("coin00", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            name.IndexOf("coin01", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return false;
        return name.StartsWith("drop_", System.StringComparison.OrdinalIgnoreCase);
    }

    static GameObject LoadDropPropFallback(ModelTable models, string dropModel)
    {
        if (string.IsNullOrEmpty(dropModel))
            return null;

        string lower = dropModel.ToLowerInvariant();
        if (lower.IndexOf("dropitems", System.StringComparison.Ordinal) < 0)
            return null;

        if (lower.IndexOf("mfighter", System.StringComparison.Ordinal) >= 0 ||
            lower.IndexOf("ffighter", System.StringComparison.Ordinal) >= 0 ||
            lower.IndexOf("mmagic", System.StringComparison.Ordinal) >= 0 ||
            lower.IndexOf("fmagic", System.StringComparison.Ordinal) >= 0 ||
            lower.IndexOf("mdarkelf", System.StringComparison.Ordinal) >= 0 ||
            lower.IndexOf("fdarkelf", System.StringComparison.Ordinal) >= 0)
            return null;

        return models.GetOrLoadModel(PotionFallbackModel);
    }

    static GameObject LoadCoinDropFallback(ModelTable models, string dropModel)
    {
        if (!ItemDropGrpCatalog.IsCoinDropModel(dropModel))
            return null;

        GameObject pile = models.GetOrLoadModel(CoinPileModel);
        if (pile != null)
            return pile;

        return models.GetOrLoadModel(CoinSingleModel);
    }
}
