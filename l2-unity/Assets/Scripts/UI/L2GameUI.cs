using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class L2GameUI : L2UI
{
    private NativeFunctions.NativeCoords _lastMousePosition;
    [SerializeField] private bool _mouseEnabled = true;

    private List<L2PopupWindow> _openedWindows;

    private static L2GameUI _instance;
    public static L2GameUI Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DiBootstrap.EnsureGameScope();
        }
        else
        {
            Destroy(this);
        }

        _openedWindows = new List<L2PopupWindow>();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void Update()
    {
        base.Update();

        if (InputManager.Instance != null && InputManager.Instance.TurnCamera)
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

        if (L2LoginUI.Instance != null)
        {
            StartLoading();
        }

        if (ChatWindow.Instance != null)
        {
            ChatWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (SystemMenuWindow.Instance != null)
        {
            SystemMenuWindow.Instance.AddWindow(_rootVisualContainer);
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
        if (InventoryWindow.Instance != null)
        {
            InventoryWindow.Instance.AddWindow(_rootVisualContainer);
            InventoryWindow.Instance.HideWindow();
        }
        if (CharacterInfoWindow.Instance != null)
        {
            CharacterInfoWindow.Instance.AddWindow(_rootVisualContainer);
            CharacterInfoWindow.Instance.HideWindow();
        }
        if (ActionWindow.Instance != null)
        {
            ActionWindow.Instance.AddWindow(_rootVisualContainer);
            ActionWindow.Instance.HideWindow();
        }
        if (TargetWindow.Instance != null)
        {
            TargetWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (ExitWindow.Instance != null)
        {
            ExitWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (L2ToolTip.Instance != null)
        {
            L2ToolTip.Instance.AddWindow(_tooltipVisualContainer);
            L2ToolTip.Instance.HideWindow();
        }
        if (L2SlotManager.Instance != null)
        {
            L2SlotManager.Instance.AddWindow(_slotVisualContainer);
            L2SlotManager.Instance.HideWindow();
        }
        if (MenuWindow.Instance != null)
        {
            MenuWindow.Instance.AddWindow(_rootVisualContainer);
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

        if (MapWindow.Instance != null)
        {
            MapWindow.Instance.AddWindow(_rootVisualContainer);
            MapWindow.Instance.HideWindow();
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
        if (InputManager.Instance == null)
            return;

        if (InputManager.Instance.OpenCharacerStatus)
        {
            if (CharacterInfoWindow.Instance != null)
            {
                CharacterInfoWindow.Instance.ToggleHideWindow();
            }
        }

        if (InputManager.Instance.OpenInventory)
        {
            if (InventoryWindow.Instance != null)
            {
                InventoryWindow.Instance.ToggleHideWindowManual();
            }
        }

        if (InputManager.Instance.OpenSystemMenu)
        {
            if (SystemMenuWindow.Instance != null)
            {
                SystemMenuWindow.Instance.ToggleHideWindow();
            }
        }

        if (InputManager.Instance.OpenActions)
        {
            if (ActionWindow.Instance != null)
            {
                ActionWindow.Instance.ToggleHideWindow();
            }
        }

        if (InputManager.Instance.OpenMap)
        {
            if (MapWindow.Instance != null)
                MapWindow.Instance.ToggleHideWindow();
        }

        if (InputManager.Instance.CloseWindow)
        {
            if (ChatWindow.Instance != null && ChatWindow.Instance.ChatOpened)
            {
                ChatWindow.Instance.CloseChat(false);
                return;
            }

            if (_openedWindows != null && _openedWindows.Count > 0)
            {
                _openedWindows[_openedWindows.Count - 1].HideWindow();
            }
            else
            {
                SystemMenuWindow.Instance.ToggleHideWindow();
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
