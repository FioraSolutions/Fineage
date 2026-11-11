using System;

public partial class L2GameUI
{
    protected override void LoadUI()
    {
        base.LoadUI();

        if (LobbyUiPacketServices.LoginUi != null)
        {
            StartLoading();
        }

        if (MessagePacketServices.Chat != null)
        {
            MessagePacketServices.Chat.AddWindow(_rootVisualContainer);
        }
        if (SystemUIPacketServices.Menu != null)
        {
            SystemUIPacketServices.Menu.AddWindow(_rootVisualContainer);
        }

        if (GameWindowPacketServices.Status != null)
        {
            GameWindowPacketServices.Status.AddWindow(_rootVisualContainer);
        }
        if (GameWindowPacketServices.Dead != null)
        {
            GameWindowPacketServices.Dead.AddWindow(_rootVisualContainer);
            GameWindowPacketServices.Dead.HideWindow();
        }
        if (AuxiliaryGameUIPacketServices.Buffer != null)
        {
            AuxiliaryGameUIPacketServices.Buffer.AddWindow(_rootVisualContainer);
            AuxiliaryGameUIPacketServices.Buffer.ShowWindow();
        }
        if (SkillbarPacketServices.Skillbar != null)
        {
            SkillbarPacketServices.Skillbar.AddWindow(_rootVisualContainer);
        }
        if (InventoryPacketServices.Window != null)
        {
            InventoryPacketServices.Window.AddWindow(_rootVisualContainer);
            InventoryPacketServices.Window.HideWindow();
        }
        if (HudCorePacketServices.CharacterInfo != null)
        {
            HudCorePacketServices.CharacterInfo.AddWindow(_rootVisualContainer);
            HudCorePacketServices.CharacterInfo.HideWindow();
        }
        if (HudCorePacketServices.Action != null)
        {
            HudCorePacketServices.Action.AddWindow(_rootVisualContainer);
            HudCorePacketServices.Action.HideWindow();
        }
        if (HudCorePacketServices.Target != null)
        {
            HudCorePacketServices.Target.AddWindow(_rootVisualContainer);
        }
        if (SystemUIPacketServices.Exit != null)
        {
            SystemUIPacketServices.Exit.AddWindow(_rootVisualContainer);
        }
        if (HudCorePacketServices.ToolTip != null)
        {
            HudCorePacketServices.ToolTip.AddWindow(_tooltipVisualContainer);
            HudCorePacketServices.ToolTip.HideWindow();
        }
        if (HudCorePacketServices.Slots != null)
        {
            HudCorePacketServices.Slots.AddWindow(_slotVisualContainer);
            HudCorePacketServices.Slots.HideWindow();
        }
        if (HudCorePacketServices.Menu != null)
        {
            HudCorePacketServices.Menu.AddWindow(_rootVisualContainer);
        }
        if(GameWindowPacketServices.Html != null)
        {
            GameWindowPacketServices.Html.AddWindow(_rootVisualContainer);
            GameWindowPacketServices.Html.HideWindow();
        }
        if (MerchantPacketServices.Dealer != null)
        {
            MerchantPacketServices.Dealer.AddWindow(_rootVisualContainer);
            MerchantPacketServices.Dealer.HideWindow();
        }
        if (ToolTipSimplePacketServices.Window != null)
        {
            ToolTipSimplePacketServices.Window.AddWindow(_rootVisualContainer);
            ToolTipSimplePacketServices.Window.HideWindow();
        }
        if (QuantityInputPacketServices.Window != null)
        {
            QuantityInputPacketServices.Window.AddWindow(_rootVisualContainer);
            QuantityInputPacketServices.Window.HideWindow();
        }
        if (SystemMessagePacketServices.Window != null)
        {
            SystemMessagePacketServices.Window.AddWindow(_rootVisualContainer);
            SystemMessagePacketServices.Window.HideWindow();
        }

        if (ItemEnchantPacketServices.Enchant != null)
        {
            ItemEnchantPacketServices.Enchant.AddWindow(_rootVisualContainer);
            ItemEnchantPacketServices.Enchant.HideWindow();
        }

        if (MultiSellPacketServices.MultiSell != null)
        {
            MultiSellPacketServices.MultiSell.AddWindow(_rootVisualContainer);
            MultiSellPacketServices.MultiSell.HideWindow();
        }

        if (SkillLearnPacketServices.SkillLearn != null)
        {
            SkillLearnPacketServices.SkillLearn.AddWindow(_rootVisualContainer);
            SkillLearnPacketServices.SkillLearn.HideWindow();
        }

        if (SkillLearnPacketServices.SkillList != null)
        {
            SkillLearnPacketServices.SkillList.AddWindow(_rootVisualContainer);
            SkillLearnPacketServices.SkillList.HideWindow();
        }

        if (SkillLearnPacketServices.SkillDesc != null)
        {
            SkillLearnPacketServices.SkillDesc.AddWindow(_rootVisualContainer);
            SkillLearnPacketServices.SkillDesc.HideWindow();
        }
        if (GameWindowPacketServices.ShowList != null)
        {
            GameWindowPacketServices.ShowList.AddWindow(_rootVisualContainer);
            GameWindowPacketServices.ShowList.HideWindow();
        }
        if (QuestPacketServices.Quest != null)
        {
            QuestPacketServices.Quest.AddWindow(_rootVisualContainer);
            QuestPacketServices.Quest.HideWindow();
        }
        if (ClanPacketServices.Clan != null)
        {
            ClanPacketServices.Clan.AddWindow(_rootVisualContainer);
            ClanPacketServices.Clan.HideWindow();
        }

        if (AuxiliaryGameUIPacketServices.SellCrop != null)
        {
            AuxiliaryGameUIPacketServices.SellCrop.AddWindow(_rootVisualContainer);
            AuxiliaryGameUIPacketServices.SellCrop.HideWindow();
        }
        if (AuxiliaryGameUIPacketServices.SeedInfo != null)
        {
            AuxiliaryGameUIPacketServices.SeedInfo.AddWindow(_rootVisualContainer);
            AuxiliaryGameUIPacketServices.SeedInfo.HideWindow();
        }

        if (HudCorePacketServices.Map != null)
        {
            HudCorePacketServices.Map.AddWindow(_rootVisualContainer);
            HudCorePacketServices.Map.HideWindow();
        }

        if (CraftPacketServices.RecipeBook != null)
        {
            CraftPacketServices.RecipeBook.AddWindow(_rootVisualContainer);
            CraftPacketServices.RecipeBook.HideWindow();
        }

        if (CraftPacketServices.Crafting != null)
        {
            CraftPacketServices.Crafting.AddWindow(_rootVisualContainer);
            CraftPacketServices.Crafting.HideWindow();
        }


        if (PartyPacketServices.PartyInvite != null)
        {
            PartyPacketServices.PartyInvite.AddWindow(_rootVisualContainer);
            PartyPacketServices.PartyInvite.HideWindow();
        }

        EnsurePartyMemberListWindow();
        if (PartyPacketServices.MemberList != null)
        {
            PartyPacketServices.MemberList.AddWindow(_rootVisualContainer);
        }

        if (ClanPacketServices.Ask != null)
        {
            ClanPacketServices.Ask.AddWindow(_rootVisualContainer);
            ClanPacketServices.Ask.HideWindow();
        }
        
        if (TradePacketServices.TradeRequest != null)
        {
            TradePacketServices.TradeRequest.AddWindow(_rootVisualContainer);
            TradePacketServices.TradeRequest.HideWindow();
        }
        
        if (TradePacketServices.Trade != null)
        {
            TradePacketServices.Trade.AddWindow(_rootVisualContainer);
            TradePacketServices.Trade.HideWindow();
        }
    }
}
