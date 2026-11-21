using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>Maps Fineage Net craft packets into legacy UI DTOs (materials from <see cref="RecipeTable"/>).</summary>
public static class NetCraftMappings
{
    public static RecipeBookItemListDto ToRecipeBookListDto(RecipeBookListNetMsg msg)
    {
        var dto = new RecipeBookItemListDto();
        if (msg == null)
            return dto;

        NetCraftSession.LastBookDwarven = msg.Dwarven;
        dto.SetBookTab(msg.Dwarven);
        int[] ids = msg.RecipeIds ?? System.Array.Empty<int>();
        for (int i = 0; i < ids.Length; i++)
        {
            int recipeId = ids[i];
            RecipeData data = TablePacketServices.Recipes.GetRecipeData(recipeId);
            if (data == null)
            {
                FineageLog.Error(
                    FineageLogCategory.Net,
                    $"RecipeBookList: missing RecipeData for recipe id_mk={recipeId}");
                continue;
            }

            int position = i + 1;
            var recipe = new RecipeInstance(data, position);
            dto.ListItemInstance.Add(recipe.GetItemInstance());
            dto.ListRecipes.Add(recipe);
        }

        return dto;
    }

    public static RecipeItemMakeInfoDto ToMakeInfoDto(RecipeItemMakeInfoNetMsg msg, bool shopMode = false)
    {
        if (msg == null)
            return null;

        RecipeData data = TablePacketServices.Recipes.GetRecipeData(msg.RecipeId);
        if (data == null)
        {
            FineageLog.Error(
                FineageLogCategory.Net,
                $"RecipeItemMakeInfo: missing RecipeData for recipe id_mk={msg.RecipeId}");
            return null;
        }

        var dto = new RecipeItemMakeInfoDto();
        dto.ApplyFromNet(msg, data, NetCraftSession.LastBookDwarven, shopMode);
        return dto;
    }

    public static RecipeItemMakeInfoDto ToShopMakeInfoDto(RecipeShopItemInfoNetMsg msg)
    {
        if (msg == null)
            return null;

        RecipeData data = TablePacketServices.Recipes.GetRecipeData(msg.RecipeId);
        if (data == null)
        {
            FineageLog.Error(
                FineageLogCategory.Net,
                $"RecipeShopItemInfo: missing RecipeData for recipe id_mk={msg.RecipeId}");
            return null;
        }

        var makeInfo = new RecipeItemMakeInfoNetMsg
        {
            RecipeId = msg.RecipeId,
            Result = msg.Result,
            CurrentMp = msg.CurrentMp,
            MaxMp = msg.MaxMp
        };
        var dto = new RecipeItemMakeInfoDto();
        dto.ApplyFromNet(makeInfo, data, NetCraftSession.LastBookDwarven, shopMode: true);
        dto.SetShopContext(msg.ManufacturerObjectId, NetCraftSession.ActiveShopPrice);
        return dto;
    }

    public static List<RecipeShopOfferDto> ToShopOffers(RecipeShopSellListNetMsg msg)
    {
        var rows = new List<RecipeShopOfferDto>();
        if (msg == null)
            return rows;

        NetCraftSession.ActiveManufacturerObjectId = msg.ManufacturerObjectId;
        RecipeShopRowNet[] items = msg.Items ?? System.Array.Empty<RecipeShopRowNet>();
        for (int i = 0; i < items.Length; i++)
        {
            RecipeShopRowNet row = items[i];
            if (row == null)
                continue;
            RecipeData data = TablePacketServices.Recipes.GetRecipeData(row.RecipeId);
            if (data == null)
            {
                FineageLog.Error(
                    FineageLogCategory.Net,
                    $"RecipeShopSellList: missing RecipeData for recipe id_mk={row.RecipeId}");
                continue;
            }

            rows.Add(new RecipeShopOfferDto
            {
                RecipeId = row.RecipeId,
                Price = row.Price,
                RecipeData = data,
                Position = i + 1
            });
        }

        return rows;
    }
}

public sealed class RecipeShopOfferDto
{
    public int RecipeId;
    public long Price;
    public RecipeData RecipeData;
    public int Position;
}