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
        if (BufferPanel.Instance != null)
        {
            BufferPanel.Instance.AddWindow(_rootVisualContainer);
            BufferPanel.Instance.ShowWindow();
        }
        if (SkillbarWindow.Instance != null)
        {
            SkillbarWindow.Instance.AddWindow(_rootVisualContainer);
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
        if (DealerWindow.Instance != null)
        {
            DealerWindow.Instance.AddWindow(_rootVisualContainer);
            DealerWindow.Instance.HideWindow();
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

        if (EnchantWindow.Instance != null)
        {
            EnchantWindow.Instance.AddWindow(_rootVisualContainer);
            EnchantWindow.Instance.HideWindow();
        }

        if (MultiSellWindow.Instance != null)
        {
            MultiSellWindow.Instance.AddWindow(_rootVisualContainer);
            MultiSellWindow.Instance.HideWindow();
        }

        if (SkillLearnWindow.Instance != null)
        {
            SkillLearnWindow.Instance.AddWindow(_rootVisualContainer);
            SkillLearnWindow.Instance.HideWindow();
        }

        if (SkillListWindow.Instance != null)
        {
            SkillListWindow.Instance.AddWindow(_rootVisualContainer);
            SkillListWindow.Instance.HideWindow();
        }

        if (DescriptionSkillWindow.Instance != null)
        {
            DescriptionSkillWindow.Instance.AddWindow(_rootVisualContainer);
            DescriptionSkillWindow.Instance.HideWindow();
        }
        if (GameWindowPacketServices.ShowList != null)
        {
            GameWindowPacketServices.ShowList.AddWindow(_rootVisualContainer);
            GameWindowPacketServices.ShowList.HideWindow();
        }
        if (QuestWindow.Instance != null)
        {
            QuestWindow.Instance.AddWindow(_rootVisualContainer);
            QuestWindow.Instance.HideWindow();
        }
        if (ClanWindow.Instance != null)
        {
            ClanWindow.Instance.AddWindow(_rootVisualContainer);
            ClanWindow.Instance.HideWindow();
        }

        if (SellCropListWindow.Instance != null)
        {
            SellCropListWindow.Instance.AddWindow(_rootVisualContainer);
            SellCropListWindow.Instance.HideWindow();
        }
        if (SeedInfoWindow.Instance != null)
        {
            SeedInfoWindow.Instance.AddWindow(_rootVisualContainer);
            SeedInfoWindow.Instance.HideWindow();
        }

        if (MapWindow.Instance != null)
        {
            MapWindow.Instance.AddWindow(_rootVisualContainer);
            MapWindow.Instance.HideWindow();
        }

        if (RecipeBookWindow.Instance != null)
        {
            RecipeBookWindow.Instance.AddWindow(_rootVisualContainer);
            RecipeBookWindow.Instance.HideWindow();
        }

        if (CraftingItemWindow.Instance != null)
        {
            CraftingItemWindow.Instance.AddWindow(_rootVisualContainer);
            CraftingItemWindow.Instance.HideWindow();
        }


        if (PartyInvitationWindow.Instance != null)
        {
            PartyInvitationWindow.Instance.AddWindow(_rootVisualContainer);
            PartyInvitationWindow.Instance.HideWindow();
        }

        EnsurePartyMemberListWindow();
        if (PartyMemberListWindow.Instance != null)
        {
            PartyMemberListWindow.Instance.AddWindow(_rootVisualContainer);
        }

        if (ClanAskJoinWindow.Instance != null)
        {
            ClanAskJoinWindow.Instance.AddWindow(_rootVisualContainer);
            ClanAskJoinWindow.Instance.HideWindow();
        }
        
        if (TradeRequestWindow.Instance != null)
        {
            TradeRequestWindow.Instance.AddWindow(_rootVisualContainer);
            TradeRequestWindow.Instance.HideWindow();
        }
        
        if (TradeWindow.Instance != null)
        {
            TradeWindow.Instance.AddWindow(_rootVisualContainer);
            TradeWindow.Instance.HideWindow();
        }
    }

    public bool AreWindowsReady()
    {
        return UILoaded
            && ClanWindow.Instance != null && ClanWindow.Instance.IsReady
            && SkillListWindow.Instance != null && SkillListWindow.Instance.IsReady;
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
        if (PartyMemberListWindow.Instance != null || Instance == null)
            return;
        Instance.gameObject.AddComponent<PartyMemberListWindow>();
    }
}
