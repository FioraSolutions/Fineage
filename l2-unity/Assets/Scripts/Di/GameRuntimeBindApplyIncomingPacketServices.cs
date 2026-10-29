/// <summary>
/// Game runtime incoming handler DI bind wiring (#142 batch slice 175).
/// </summary>
public static class GameRuntimeBindApplyIncomingPacketServices
{
    public static void BindIncoming(
        GameRuntime runtime,
        WorldPacketApply worldApply,
        MessagePacketApply messageApply,
        IWorldDomainSink domain) =>
        BindApplyIncomingPacketServices.Bind(
            game: runtime.Game,
            manager: runtime.Manager,
            gameWorld: runtime.World,
            worldApply: worldApply,
            messageApply: messageApply,
            domain: domain,
            inventory: runtime.Inventory,
            targets: runtime.Targets,
            shortcuts: runtime.Shortcuts,
            positionValidation: runtime.PositionValidation,
            html: runtime.Html,
            dealer: runtime.Dealer,
            multiSell: runtime.MultiSell,
            trade: runtime.Trade,
            tradeRequest: runtime.TradeRequest,
            partyInvite: runtime.PartyInvite,
            enchant: runtime.Enchant,
            recipeBook: runtime.RecipeBook,
            crafting: runtime.Crafting,
            skillLearn: runtime.SkillLearn,
            skillDesc: runtime.SkillDesc,
            skillList: runtime.SkillList,
            quest: runtime.Quest,
            questList: runtime.QuestList,
            clan: runtime.Clan,
            showList: runtime.ShowList,
            seedInfo: runtime.SeedInfo,
            sellCrop: runtime.SellCrop,
            buffer: runtime.Buffer);
}
