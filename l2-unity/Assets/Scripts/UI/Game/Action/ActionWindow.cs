using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionWindow : L2PopupWindow
{
    private const int SLOTS_PER_ROW = 8;
    private VisualTreeAsset _slotTemplate;
    private VisualElement _basicContainer;
    private VisualElement _partyContainer;
    private VisualElement _tokenContainer;
    private VisualElement _socialContainer;
    private List<ActionSlot> _slots;

    private static ActionWindow _instance;
    public static ActionWindow Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireHudCorePacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    
    private void Start()
    {
        WireHudCorePacketServices();
    }

    private void WireHudCorePacketServices()
    {
        HudCorePacketServices.Bind(action: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ActionWindow");
        _slotTemplate = LoadAsset("Data/UI/_Elements/Template/ActionSlot");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        VisualElement dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);

        Label _windowName = (Label)GetElementById("windows-name-label");
        _windowName.text = "Actions";

        _basicContainer = GetElementById("BasicSlots");
        _partyContainer = GetElementById("PartySlots");
        _tokenContainer = GetElementById("TokenSlots");
        _socialContainer = GetElementById("SocialSlots");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        // Center window
        _windowEle.style.left = new Length(50, LengthUnit.Percent);
        _windowEle.style.top = new Length(50, LengthUnit.Percent);
        _windowEle.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent)));

        _slots = new List<ActionSlot>();

        int position = 0;
        for (int i = 0; i < SLOTS_PER_ROW * 4; i++)
        {
            AddSlot(position++, _basicContainer);
        }

        for (int i = 0; i < SLOTS_PER_ROW * 2; i++)
        {
            AddSlot(position++, _partyContainer);
        }

        for (int i = 0; i < SLOTS_PER_ROW * 2; i++)
        {
            AddSlot(position++, _tokenContainer);
        }

        for (int i = 0; i < SLOTS_PER_ROW * 3; i++)
        {
            AddSlot(position++, _socialContainer);
        }

        _slots[0].AssignAction(ActionType.Sit);
        _slots[1].AssignAction(ActionType.WalkRun);
        _slots[2].AssignAction(ActionType.Attack);
        _slots[3].AssignAction(ActionType.NextTarget);
        _slots[4].AssignAction(ActionType.Pickup);
        _slots[5].AssignAction(ActionType.Assist);
        _slots[6].AssignAction(ActionType.RideStrider);
    }

    private void AddSlot(int position, VisualElement container)
    {
        VisualElement slotElement = _slotTemplate.Instantiate()[0];
        container.Add(slotElement);

        ActionSlot slot = new ActionSlot(slotElement, position, L2Slot.SlotType.Action);
        _slots.Add(slot);
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioPacketServices.Audio.PlayUISound("window_open");
        SystemUIPacketServices.Ui.WindowOpened(this);
    }

    public override void HideWindow()
    {
        base.HideWindow();
        AudioPacketServices.Audio.PlayUISound("window_close");
        SystemUIPacketServices.Ui.WindowClosed(this);
    }
}
