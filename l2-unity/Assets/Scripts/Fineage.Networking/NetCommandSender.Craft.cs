using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetCommandSender
{
    public Task SendRecipeBookOpenAsync(int dwarven) =>
        SendPayload(NetOpcode.RecipeBookOpen, NetCodec.EncodeRecipeBookOpen(new RecipeBookOpenNetMsg { Dwarven = dwarven }));

    public Task SendRecipeMakeSelfAsync(int recipeId) =>
        SendPayload(NetOpcode.RecipeMakeSelf, NetCodec.EncodeRecipeMakeSelf(new RecipeMakeSelfNetMsg { RecipeId = recipeId }));

    public Task SendSetRecipeShopAsync(SetRecipeShopNetMsg msg) =>
        SendPayload(NetOpcode.SetRecipeShop, NetCodec.EncodeSetRecipeShop(msg ?? new SetRecipeShopNetMsg()));

    public Task SendRecipeShopMakeInfoAsync(int manufacturerObjectId, int recipeId) =>
        SendPayload(
            NetOpcode.RecipeShopMakeInfo,
            NetCodec.EncodeRecipeShopMakeInfo(new RecipeShopMakeInfoNetMsg
            {
                ManufacturerObjectId = manufacturerObjectId,
                RecipeId = recipeId
            }));

    public Task SendRecipeShopMakeItemAsync(int manufacturerObjectId, int recipeId) =>
        SendPayload(
            NetOpcode.RecipeShopMakeItem,
            NetCodec.EncodeRecipeShopMakeItem(new RecipeShopMakeItemNetMsg
            {
                ManufacturerObjectId = manufacturerObjectId,
                RecipeId = recipeId
            }));

    public Task SendPrivateStoreListRequestAsync(int storeObjectId) =>
        SendPayload(
            NetOpcode.PrivateStoreListRequest,
            NetCodec.EncodePrivateStoreListRequest(new PrivateStoreListRequestNetMsg { StoreObjectId = storeObjectId }));

    public Task SendPrivateStoreQuitAsync() =>
        SendPayload(NetOpcode.PrivateStoreQuit, System.Array.Empty<byte>());

    Task SendPayload(ushort opcode, byte[] payload)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(opcode, payload, _nextSeq());
    }
}