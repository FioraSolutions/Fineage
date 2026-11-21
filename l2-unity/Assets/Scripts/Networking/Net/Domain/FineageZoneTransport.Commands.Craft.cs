using Fineage.Net.Contract;

public sealed partial class FineageZoneTransport
{
    bool TrySendCraftCommands(INetworkCommand command)
    {
        RequestRecipeBookOpenCommand open = command as RequestRecipeBookOpenCommand;
        if (open != null)
        {
            Session.SendRecipeBookOpen(open.IsDwarven);
            return true;
        }

        RequestRecipeBookDestroyCommand destroy = command as RequestRecipeBookDestroyCommand;
        if (destroy != null)
        {
            FineageLog.Warn(FineageLogCategory.GameNet, "TX dropped (no Net opcode): RequestRecipeBookDestroyCommand");
            return true;
        }

        RequestRecipeItemMakeSelfCommand makeSelf = command as RequestRecipeItemMakeSelfCommand;
        if (makeSelf != null)
        {
            Session.SendRecipeMakeSelf(makeSelf.RecipeId);
            return true;
        }

        RequestRecipeItemMakeInfoCommand makeInfo = command as RequestRecipeItemMakeInfoCommand;
        if (makeInfo != null)
        {
            NetCraftLocalPreview.OpenForRecipe(makeInfo.RecipeId);
            return true;
        }

        RequestSetRecipeShopCommand setShop = command as RequestSetRecipeShopCommand;
        if (setShop != null)
        {
            Session.SendSetRecipeShop(new SetRecipeShopNetMsg
            {
                Message = setShop.Message ?? "",
                Items = setShop.Items ?? System.Array.Empty<RecipeShopRowNet>()
            });
            return true;
        }

        RequestRecipeShopMakeInfoCommand shopInfo = command as RequestRecipeShopMakeInfoCommand;
        if (shopInfo != null)
        {
            NetCraftSession.ActiveShopPrice = 0;
            Session.SendRecipeShopMakeInfo(shopInfo.ManufacturerObjectId, shopInfo.RecipeId);
            return true;
        }

        RequestRecipeShopMakeItemCommand shopMake = command as RequestRecipeShopMakeItemCommand;
        if (shopMake != null)
        {
            Session.SendRecipeShopMakeItem(shopMake.ManufacturerObjectId, shopMake.RecipeId);
            return true;
        }

        RequestPrivateStoreListCommand list = command as RequestPrivateStoreListCommand;
        if (list != null)
        {
            Session.SendPrivateStoreListRequest(list.StoreObjectId);
            return true;
        }

        RequestPrivateStoreQuitCommand quit = command as RequestPrivateStoreQuitCommand;
        if (quit != null)
        {
            Session.SendPrivateStoreQuit();
            return true;
        }

        return false;
    }

    public static bool TrySendNetCraftUserCommand(int commandId)
    {
        switch (commandId)
        {
            case (int)PlayerCommand.DWARVENMANUFACTURE:
                new DwarvenManufactureAction().UseAction();
                return true;
            case (int)PlayerCommand.GENERALMANUFACTURE:
                new GeneralManufactureAction().UseAction();
                return true;
        }

        return false;
    }
}