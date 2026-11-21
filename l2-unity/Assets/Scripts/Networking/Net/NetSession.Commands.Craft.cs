using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetSession
{
    public Task SendRecipeBookOpen(int dwarven) =>
        _commandSender.SendRecipeBookOpenAsync(dwarven);

    public Task SendRecipeMakeSelf(int recipeId) =>
        _commandSender.SendRecipeMakeSelfAsync(recipeId);

    public Task SendSetRecipeShop(SetRecipeShopNetMsg msg) =>
        _commandSender.SendSetRecipeShopAsync(msg);

    public Task SendRecipeShopMakeInfo(int manufacturerObjectId, int recipeId) =>
        _commandSender.SendRecipeShopMakeInfoAsync(manufacturerObjectId, recipeId);

    public Task SendRecipeShopMakeItem(int manufacturerObjectId, int recipeId) =>
        _commandSender.SendRecipeShopMakeItemAsync(manufacturerObjectId, recipeId);

    public Task SendPrivateStoreListRequest(int storeObjectId) =>
        _commandSender.SendPrivateStoreListRequestAsync(storeObjectId);

    public Task SendPrivateStoreQuit() =>
        _commandSender.SendPrivateStoreQuitAsync();
}