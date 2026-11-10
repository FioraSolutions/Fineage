
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;


public partial class ToolTipSimple : L2PopupWindow, IToolTips
{
    private ShowToolTip _showToolTip;

    private static ToolTipSimple _instance;
    private VisualElement _content;
    private VisualTreeAsset _windowTemplateWeapon;
    private VisualTreeAsset _windowTemplateSkill;
    private VisualTreeAsset _windowTemplateSimple;
    private VisualTreeAsset _windowTemplateString;
    private VisualTreeAsset _windowTemplateAcccesories;
    private VisualTreeAsset _windowTemplateRequiredAcccesories;
    private VisualTreeAsset _windowTemplateArmor;
    private VisualTreeAsset _setsElements;
    private VisualTreeAsset _setsEffects;
    private VisualElement _contentInside;
    private float _lastHeightContent = 0;
    private float _heightContent = 0;
    private float _widtchContent = 0;
    private VisualElement _selectShow;
    private TooltipDataProvider _dataProvider;
    private CalcNewPosition _calcPosition;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static ToolTipSimple Instance { get { return _instance; } }



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _showToolTip = new ShowToolTip(this);
            _dataProvider = new TooltipDataProvider();
            _calcPosition = new CalcNewPosition();
            WireToolTipSimplePacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireToolTipSimplePacketServices();
    }

    private void WireToolTipSimplePacketServices()
    {
        ToolTipSimplePacketServices.Bind(window: this);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipObject");
        _windowTemplateString = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipString");
        _windowTemplateSimple = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipSimple");
        _windowTemplateWeapon = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipWeapon");
        _windowTemplateAcccesories = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipAccessories");
        _windowTemplateRequiredAcccesories = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipRequiredAccessories");
        _windowTemplateArmor = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipArmor");
        _windowTemplateSkill = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipSkill");
        _setsElements = LoadAsset("Data/UI/_Elements/Game/ToolTips/Elements/SetsElements");
        _setsEffects = LoadAsset("Data/UI/_Elements/Game/ToolTips/Elements/SetsEffectNameElements");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);



        yield return new WaitForEndOfFrame();


        _content = GetElementByClass("control-box");
        _windowEle.style.display = DisplayStyle.None;

    }

   


    /// <summary>
    /// Interlude client: a left click on a slot selects it <b>and</b> shows the item description.
    /// The hover tooltip only sets the content up; without this the click looked like it did
    /// nothing but frame the slot, and gear slots went further and unequipped the item.
    /// </summary>
    public void EventLeftClickSlot(VisualElement ve)
    {
        if (ve == null || _windowEle == null)
            return;

        string[] ids = GetUniquePosition(ve);
        if (ids == null || ids.Length < 2)
            return;

        var template = SwitchToolTip(ids, true);

        if(template != null)
        {
            int position = Int32.Parse(ids[0]);
            int type = Int32.Parse(ids[1]);

            object data = GetDataClickLeft(type, position);

            if(data != null)
            {
                AddData(data, template);
                _selectShow = ve;
                _windowEle.style.display = DisplayStyle.Flex;
                _showToolTip.Show(ve, GetForceBelow(ids));
                return;
            }
        }

        // Nothing to describe (empty bag / gear slot): the click must not leave a stale tooltip.
        // Only the panel is hidden here — Hide() also drops the buff countdown subscriptions of
        // whatever tooltip the pointer was on, and a click on an empty cell must not do that.
        _windowEle.style.display = DisplayStyle.None;
    }


    private void AddData(object data , TemplateContainer template)
    {
        if(data.GetType() == typeof(Product))
        {
            Product product = (Product)data;
            UseProduct(product, template);
        }else if (data.GetType() == typeof(ItemInstance))
        {
            ItemInstance item = (ItemInstance)data;
            UseItem(item, template);
        }else if (data.GetType() == typeof(SkillInstance))
        {
            SkillInstance item = (SkillInstance)data;
            UseSkill( item, template);
        }
  
    }

    private void UseProduct(Product product, TemplateContainer template)
    {
        switch (product.GetTypeItem())
        {
            case EnumType2.TYPE2_WEAPON:
                _dataProvider.AddDataWeapon(template, product);
                break;
            case EnumType2.TYPE2_ACCESSORY:
                _dataProvider.AddDataAccessories(template, product);
                break;
            case EnumType2.TYPE2_OTHER:
                _dataProvider.AddDataOther(template, product);
                break;
            case EnumType2.TYPE2_SHIELD_ARMOR:
                _dataProvider.AddDataArmor(template, product, _setsElements , _setsEffects);
                break;
        }
    }

    private void UseItem(ItemInstance item, TemplateContainer template)
    {
        switch (item.Category)
        {
            case ItemCategory.Weapon:
                _dataProvider.AddDataWeapon(template, item);
                break;
            case ItemCategory.Jewel:
                _dataProvider.AddDataAccessories(template, item);
                break;
            case ItemCategory.Item:
                _dataProvider.AddDataOther(template, item);
                break;
            case ItemCategory.RequiredItem:
                _dataProvider.AddRequiredData(template, item);
                break;
            case ItemCategory.ShieldArmor:
                _dataProvider.AddDataArmor(template, item, _setsElements, _setsEffects);
                break;
        }
    }

    private void UseSkill(SkillInstance item, TemplateContainer template)
    {
        _dataProvider.AddDataSkill(template, item);
    }

    public void ManualHide()
    {
        StartCoroutine(AllHide());
    }
    public void RegisterCallback(SlotType type , List<VisualElement> list)
    {

        list.ForEach(item =>
        {
            if (item != null)
            {
                item.RegisterCallback<MouseOverEvent>(evt =>
                {
                    VisualElement ve = (VisualElement)evt.currentTarget;
                    if (ve != null)
                    {
                        StartCoroutine(StartNewPosition(ve));
                        //Debug.Log("TooTipAction StartNewPosition disabled 33" + _windowEle.worldBound.height);
                       /// Debug.Log("TooTipAction StartNewPosition disabled 33 " + _content.worldBound.height);
                    }
                }, TrickleDown.TrickleDown);

                item.RegisterCallback<MouseOutEvent>(evt =>
                {
                    VisualElement ve = (VisualElement)evt.currentTarget;
                    if (ve != null)
                    {
                        StartCoroutine(EndNewPosition(ve));
                    }

                    evt.StopPropagation();

                }, TrickleDown.TrickleDown);

            }
        });

    }

    public void RegisterCallback(SlotType type, VisualElement item , int position)
    {
         RegisterPrice(type , item, position);   
    }

    private void RegisterPrice(SlotType type , VisualElement item , int position)
    {
        if (item != null)
        {
            SetIdElement(item, position, (int)type);

            item.RegisterCallback<MouseOverEvent>(evt =>
            {
                VisualElement ve = (VisualElement)evt.currentTarget;
                if (ve != null)
                {
                    StartCoroutine(StartNewPosition(ve));
                }
            }, TrickleDown.TrickleDown);

            item.RegisterCallback<MouseOutEvent>(evt =>
            {
                VisualElement ve = (VisualElement)evt.currentTarget;
                if (ve != null)
                {
                    StartCoroutine(EndNewPosition(ve));
                }

                evt.StopPropagation();

            }, TrickleDown.TrickleDown);

        }
    }

    private void SetIdElement(VisualElement item, int position , int type)
    {
        item.name = position + "_"+ type.ToString();
    }

    private IEnumerator EndNewPosition(VisualElement ve)
    {
        _windowEle.style.display = DisplayStyle.None;
        _showToolTip.Hide(ve);
        yield return new WaitForEndOfFrame();
    }
    public IEnumerator AllHide()
    {
        _windowEle.style.display = DisplayStyle.None;
        yield return new WaitForEndOfFrame();
    }
    private IEnumerator StartNewPosition(VisualElement ve)
    {
        string[] ids = GetUniquePosition(ve);
        TemplateContainer template = SwitchToolTip(ids , false);
        if(template != null)
        {
            _windowEle.style.display = DisplayStyle.Flex;
            _contentInside.MarkDirtyRepaint();

            AddData(ids, template);
            bool isBelow = GetForceBelow(ids);
            _selectShow = ve;
            //esle position layout != 0 <--- (EndNewPosition add new Vector2(0,0))
            //This means the layout did not return to the base, most likely this is an error. Restarting the transition to a new position
            //if (_windowEle.worldBound.height != 0)
            if (_contentInside.worldBound.height != 0)
            {
                _showToolTip.Show(_selectShow , isBelow);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
