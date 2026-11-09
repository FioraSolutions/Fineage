using System;

﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class L2GameUI : L2UI
{
    private NativeFunctions.NativeCoords _lastMousePosition;
    [SerializeField] private bool _mouseEnabled = true;

    private List<L2PopupWindow> _openedWindows;

    private static L2GameUI _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static L2GameUI Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DiBootstrap.EnsureGameScope();
            WireSystemUIPacketServices();
        }
        else
        {
            Destroy(this);
        }

        _openedWindows = new List<L2PopupWindow>();
    }

    private void Start()
    {
        WireSystemUIPacketServices();
    }

    private void WireSystemUIPacketServices()
    {
        SystemUIPacketServices.Bind(ui: this);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    protected override void Update()
    {
        base.Update();

        InputManager input = ControllerPacketServices.Input;
        if (input != null && input.TurnCamera)
        {
            DisableMouse();
        }
        else
        {
            EnableMouse();
        }

        CheckUIInputs();
    }

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

    public bool AreWindowsReady()
    {
        return UILoaded
            && ClanPacketServices.Clan != null && ClanPacketServices.Clan.IsReady
            && SkillLearnPacketServices.SkillList != null && SkillLearnPacketServices.SkillList.IsReady;
    }

    public void EnableMouse()
    {
        if (!_mouseEnabled)
        {
            _mouseEnabled = true;
#if UNITY_STANDALONE_WIN
            NativeFunctions.SetCursorPos(_lastMousePosition.X, _lastMousePosition.Y);
#endif
        }
    }

    public void DisableMouse()
    {
        if (_mouseEnabled)
        {
#if UNITY_STANDALONE_WIN
            NativeFunctions.GetCursorPos(out _lastMousePosition);
#endif
            _mouseEnabled = false;
        }
    }

    public void CheckUIInputs()
    {
        InputManager input = ControllerPacketServices.Input;
        if (input == null)
            return;

        if (input.OpenCharacerStatus)
        {
            if (HudCorePacketServices.CharacterInfo != null)
            {
                HudCorePacketServices.CharacterInfo.ToggleHideWindow();
            }
        }

        if (input.OpenInventory)
        {
            if (InventoryPacketServices.Window != null)
            {
                InventoryPacketServices.Window.ToggleHideWindowManual();
            }
        }

        if (input.OpenSystemMenu)
        {
            if (SystemUIPacketServices.Menu != null)
            {
                SystemUIPacketServices.Menu.ToggleHideWindow();
            }
        }

        if (input.OpenActions)
        {
            if (HudCorePacketServices.Action != null)
            {
                HudCorePacketServices.Action.ToggleHideWindow();
            }
        }

        if (input.OpenMap)
        {
            if (HudCorePacketServices.Map != null)
                HudCorePacketServices.Map.ToggleHideWindow();
        }

        if (input.CloseWindow)
        {
            if (MessagePacketServices.Chat != null && MessagePacketServices.Chat.ChatOpened)
            {
                MessagePacketServices.Chat.CloseChat(false);
                return;
            }

            if (_openedWindows != null && _openedWindows.Count > 0)
            {
                _openedWindows[_openedWindows.Count - 1].HideWindow();
            }
            else
            {
                SystemUIPacketServices.Menu.ToggleHideWindow();
            }
        }
    }

    public void WindowOpened(L2PopupWindow popupWindow)
    {
        _openedWindows.Add(popupWindow);
    }

    public void WindowClosed(L2PopupWindow popupWindow)
    {
        _openedWindows.Remove(popupWindow);
    }

    public void OnGUI()
    {
        if (_mouseEnabled)
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        }
    }

    static void EnsurePartyMemberListWindow()
    {
        if (PartyPacketServices.MemberList != null || Instance == null)
            return;
        Instance.gameObject.AddComponent<PartyMemberListWindow>();
    }
}
