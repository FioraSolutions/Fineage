using System.Collections.Generic;
using UnityEngine;

/// <summary>Live cart adena total and weight delta (Mobius prices in BuyList are already taxed).</summary>
public static class ShopCartTotals
{
    public static int SumUnitPriceTimesCount(IReadOnlyList<Product> cart)
    {
        if (cart == null || cart.Count == 0)
            return 0;

        long total = 0;
        for (int i = 0; i < cart.Count; i++)
        {
            Product row = cart[i];
            if (row == null)
            {
                Debug.LogError("[Shop] cart row null at index " + i);
                continue;
            }

            if (row.ItemId <= 0)
            {
                Debug.LogError("[Shop] cart row itemId=" + row.ItemId + " at index " + i);
                continue;
            }

            int qty = row.Count;
            if (qty <= 0)
            {
                Debug.LogError("[Shop] cart row itemId=" + row.ItemId + " count=" + qty + " at index " + i);
                continue;
            }

            total += (long)row.Price * qty;
            if (total > int.MaxValue)
                return int.MaxValue;
        }

        return (int)total;
    }

    public static int SumStackWeight(IReadOnlyList<Product> cart)
    {
        if (cart == null || cart.Count == 0)
            return 0;

        long total = 0;
        for (int i = 0; i < cart.Count; i++)
        {
            Product row = cart[i];
            if (row == null)
                continue;
            total += row.GetWeight();
            if (total > int.MaxValue)
                return int.MaxValue;
        }

        return (int)total;
    }
}