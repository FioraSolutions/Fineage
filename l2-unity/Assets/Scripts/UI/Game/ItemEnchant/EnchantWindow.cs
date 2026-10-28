using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static EnchantResultDto;
using static L2Slot;
using VContainer;


public class EnchantWindow : L2PopupWindow
{

    [Inject] GameClient _gameClient;

    private GameClient Game => ItemEnchantPacketServices.Game;
    private static EnchantWindow _instance;
    private VisualTreeAsset _inventorySlotTemplate;
    private VisualTreeAsset _inventorySlotChoiceTemplate;
    private VisualElement _slot1;
    private VisualElement _slot2;
    private VisualElement _progressBar;
    private Label _textLabel;
    private const string _defaultTextLabel = "Please place the item to be enchanted in the empty slot below.";
    private VisualElement _progressBarBg;
    private VisualElement _animElement;
    private List<Texture2D> _loadingAnimWaitPng;
    private List<Texture2D> _loadingAnimSuccessPng;
    private List<Texture2D> _loadingAnimFailPng;
    private bool _isStartAnim = false;
    private bool _isRun = false;
    private float _moveDuration = 1.5f;
    private float offsetLeft2 = 76;//default offset element2
    private float offsetLeft1 = 0;//default offset element2
   // private EtcItemgrp _useScroll;
    private EnchantSlot _enchant1;
    private EnchantSlot _enchant2;
    private ItemInstance _selectItem;
    private bool _zoneWire;
    private bool _closingFromServer;
    private bool _awaitingRoll;
    private int _scrollObjectId;
    private bool _frameCloseWired;

    public bool AcceptsTarget
    {
        get { return _zoneWire && !_isWindowHidden; }
    }


    public static EnchantWindow Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _loadingAnimWaitPng = new List<Texture2D>();
            _loadingAnimSuccessPng = new List<Texture2D>();
            _loadingAnimFailPng = new List<Texture2D>();
            CacheTexture();
            WireItemEnchantPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireItemEnchantPacketServices();
    }

    private void WireItemEnchantPacketServices()
    {
        ItemEnchantPacketServices.Bind(game: _gameClient, enchant: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ItemEnchant/EnchantWindow");
        _inventorySlotTemplate = LoadAsset("Data/UI/_Elements/Template/SlotEnchant");
        _inventorySlotChoiceTemplate = LoadAsset("Data/UI/_Elements/Template/SlotEnchantChoice");
    }

    private void CacheTexture()
    {
        List<string> listWait = EnchantListTextures.GetNameTexturesLoading();
        List<string> listSuccess = EnchantListTextures.GetNameTexturesSuccess();
        List<string> listFail = EnchantListTextures.GetNameTexturesFail();

        AddCache(listWait, ref _loadingAnimWaitPng);
        AddCache(listSuccess, ref _loadingAnimSuccessPng);
        AddCache(listFail, ref _loadingAnimFailPng);

    }

    private void AddCache(List<string> list , ref List<Texture2D> animList)
    {
        foreach (string href in list)
        {
            animList.Add(IconManager.Instance.LoadTextureOtherSources(href));
        }
    }
    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");

        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        var buttonStart = GetElementById("StartButton");
        Button closeButton = (Button)GetElementById("CancelButton");
        _progressBar = GetElementById("ProgressGauge");
        _textLabel = (Label)GetElementById("textLabel");
        _progressBarBg = GetElementById("ProgressBg");
        _animElement = GetElementById("animelement");
        var footer = GetElementById("footer");

        RegisterClickWindowEvent(_windowEle, dragArea);
        buttonStart.RegisterCallback<ClickEvent>((evt) => OnClick(evt));
        closeButton.RegisterCallback<ClickEvent>((evt) => OnCancel(evt));
        WireFrameClose();

        _slot1 = GetElementById("slot1");
        _slot2 = GetElementById("slot2");
        //_itemBox = GetElementById("itemBox");
        InitSlotElement1(_slot1);
        InitSlotElement2(_slot2);
        OnCenterScreen(root);

    }


    private void OnDestroy()
    {
        _instance = null;
    }

    private EnchantSlot ChangeSlotBoxType(VisualElement slotBox , EnchantSlot enchantSlot)
    {
        VisualElement slotElement = CretaVisualElement();
        enchantSlot = CreateEnchantSlot(1, slotElement, SlotType.Enchant);
        enchantSlot.AssignEmpty();
        slotBox.Add(slotElement);
        return enchantSlot;
    }

    private EnchantSlot ChangeSlotBoxTypeChoice(VisualElement slotBox, EnchantSlot enchantSlot)
    {
        VisualElement slotElement = CretaVisualElementChoice();
        enchantSlot = CreateEnchantSlot(1, slotElement, SlotType.Enchant);
        enchantSlot.AssignEmpty();
        slotBox.Add(slotElement);
        return enchantSlot;
    }

    private void InitSlotElement1(VisualElement slotBox)
    {
        VisualElement slotElement = CretaVisualElement();
        _enchant1 = CreateEnchantSlot(0, slotElement, SlotType.Enchant);
        _enchant1.AssignEmpty();
        slotBox.Add(slotElement);
    }

    private void InitSlotElement2(VisualElement slotBox)
    {
        VisualElement slotElement = CretaVisualElementChoice();
        _enchant2 = CreateEnchantSlot(1, slotElement, SlotType.Enchant);
        _enchant2.AssignEmpty();
        slotBox.Add(slotElement);
    }


    public void AddEnchantItem(int objectId)
    {
        if (_zoneWire && (objectId <= 0 || objectId == _scrollObjectId))
            return;

        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory == null)
            return;
        ItemInstance itemInstance = inventory.GetItem(objectId);
        if (itemInstance == null)
        {
            Dictionary<int, ItemInstance> worn = inventory.GetPlayerEquipInventory();
            if (worn != null)
                worn.TryGetValue(objectId, out itemInstance);
        }

        if (itemInstance != null)
        {
            _selectItem = itemInstance;
            _slot2.Clear();
            _enchant2 = ChangeSlotBoxType(_slot2, _enchant2);
            _enchant2.AssignItem(itemInstance);
            if (_textLabel != null)
                _textLabel.text = ZoneV2Enchant.WithLevel(itemInstance.GetName(), itemInstance.EnchantLevel);
        }
        else
        {
            // Debug.LogWarning("EnchantWindows error not add dropItems");
        }
    }

    private VisualElement CretaVisualElement()
    {
        return _inventorySlotTemplate.Instantiate()[0];
    }

    private VisualElement CretaVisualElementChoice()
    {
        return _inventorySlotChoiceTemplate.Instantiate()[0];
    }

    private EnchantSlot CreateEnchantSlot(int i, VisualElement slotElement, SlotType slotType)
    {
        return new EnchantSlot(i, slotElement, null, slotType, false);
    }
   
    private void OnClick(ClickEvent evt)
    {
        if (_awaitingRoll)
            return;
        if (_selectItem == null || (_zoneWire && _selectItem.ObjectId == _scrollObjectId))
        {
            if (_textLabel != null)
                _textLabel.text = "Select an item to enchant.";
            return;
        }

        if (_zoneWire)
            _awaitingRoll = true;
        RequestEnchantItem(_selectItem.ObjectId);
    }

    private void OnCancel(ClickEvent evt)
    {
        if (_zoneWire)
        {
            HideWindow();
            return;
        }

        RequestEnchantItem(-1);
    }

    public void BeginZoneSelect(ItemInstance scroll)
    {
        _zoneWire = true;
        _scrollObjectId = scroll != null ? scroll.ObjectId : 0;
        _selectItem = null;
        if (_enchant1 == null || scroll == null)
            return;

        ShowWindow(scroll);
        if (_textLabel != null)
            _textLabel.text = "Select an item to enchant.";
        SetWindowTitle("Enchant");
    }

    public void ApplyZoneResult(ZoneV2EnchantResult result)
    {
        ZoneV2Enchant.View view = ZoneV2Enchant.ViewOf(result);
        if (view.RemoveItem || view.ShowEnchant || view.CloseWindow)
            CloseFromServer();
    }

    public void AbortZone()
    {
        if (!_zoneWire)
            return;
        CloseFromServer();
    }

    void CloseFromServer()
    {
        _closingFromServer = true;
        HideWindow();
        _closingFromServer = false;
    }

    public override void HideWindow()
    {
        if (_isWindowHidden)
            return;

        bool cancel = _zoneWire && !_closingFromServer;
        _zoneWire = false;
        _awaitingRoll = false;
        _scrollObjectId = 0;
        _selectItem = null;
        base.HideWindow();
        if (!cancel)
            return;

        GameClient game = Game;
        if (game != null)
            game.Send(new RequestEnchantItemCommand(-1));
    }

    void WireFrameClose()
    {
        if (_frameCloseWired || _windowEle == null)
            return;
        VisualElement frameClose = GetElementByClass("btn-close-frame");
        if (frameClose == null)
            return;
        frameClose.RegisterCallback<MouseUpEvent>(evt => OnCancel(null));
        _frameCloseWired = true;
    }

    void SetWindowTitle(string title)
    {
        if (_windowEle == null)
            return;
        Label label = _windowEle.Q<Label>("windows-name-label");
        if (label != null)
            label.text = title;
    }


    private void RequestEnchantItem(int objectId)
    {
        //if (_selectItem != null)
        //{
            Game.Send(new RequestEnchantItemCommand(objectId));
        //}
    }

    private async void  DecreaseProgressBar()
    {
        float endValue = 245;
        int index = 0;

        while (index <= endValue)
        {
            index = index + 7;
            ProgressBarUtils.UpdatePb(_progressBarBg, _progressBar, endValue, index, endValue);
            float delayPerIteration = _moveDuration / endValue * 1000;
            await Task.Delay((int)delayPerIteration);
        }
        SetPb0(_progressBar);
        _isRun = false ;
    }





    private void SetPb0(VisualElement pbGauge)
    {
        pbGauge.style.width = Convert.ToSingle(0);
    }
    


    private IEnumerator MoveElementsToCenter(VisualElement element1, VisualElement element2 , bool isSuccess)
    {
        float fadeStartTime = _moveDuration / 2;

        element2.style.opacity = 1;
        element1.style.opacity = 1;

        element1.style.left = offsetLeft1;
        element2.style.left = offsetLeft2;

        Vector2 startPos1 = new Vector2(offsetLeft1, element1.resolvedStyle.top);
        Vector2 startPos2 = new Vector2(offsetLeft2, element2.resolvedStyle.top);

        Vector2 intersectionPoint = (startPos1 + startPos2) / 2;

        float elapsedTime = 0f;

        while (elapsedTime < _moveDuration)
        {
            float t = elapsedTime / _moveDuration;

            float left1 = Mathf.Lerp(startPos1.x, intersectionPoint.x, t);
            element1.style.left = left1;

            float left2 = Mathf.Lerp(startPos2.x, intersectionPoint.x, t);
            float newLeft = left2 - offsetLeft2;
            element2.style.left = newLeft;


            float animTime = _moveDuration - 0.4f;
            //PlayAnim(_loadingAnimWaitPng, _animElement, animTime);
            WorkerAnim(_animElement, fadeStartTime, isSuccess);
            StartFinalEvent(fadeStartTime, elapsedTime, element1);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetOpacity0(element1);
        _isRun = false;
        _isStartAnim = false;
    }

    public void ShowWindow(ItemInstance item)
    {
        base.ShowWindow();

        ResetStyle(_slot1, _slot2 , _textLabel);
        ResetProgressBar(_progressBar);
        _enchant1.AssignItem(item);
        ClearSlot2(_slot2);
        _selectItem = null;
        _textLabel.text = _defaultTextLabel;
    }

    public void EnchantResult(EnumResult result)
    {
        switch (result)
        {
            case EnumResult.Success:
                UpdateS1();
                StartEnchantAnim(true);
                break;
            case EnumResult.Cancelled:
               HideWindow();
                break;
            case EnumResult.UnSuccess:
                StartEnchantAnim(false);
                break;
            case EnumResult.Unk_Result_1:
                StartEnchantAnim(false);
                break;
            case EnumResult.Unk_Result_4:
                StartEnchantAnim(false);
                break;
        }
    }


    private void StartEnchantAnim(bool isSuccess)
    {
        if (!_isRun)
        {
            _isRun = true;
            DecreaseProgressBar();
            StartCoroutine(MoveElementsToCenter(_slot1, _slot2 , isSuccess));
        }
    }

    private void UpdateS1()
    {
        var itemInstance = PlayerInventory.Instance.GetItem(_selectItem.ObjectId);
        if (itemInstance != null && itemInstance.EnchantLevel > 0)
        {
            //StorageVariable.getInstance().AddS1Items(new VariableItem(itemInstance.Count.ToString(), itemInstance.ObjectId));
            //StorageVariable.getInstance().AddS2Items(new VariableItem(itemInstance.ItemData.ItemName.Name, itemInstance.ObjectId));
        }
    }

    private void ResetStyle(VisualElement _slot1 , VisualElement _slot2 , Label text)
    {
        _slot2.style.opacity = 1;
        _slot1.style.opacity = 1;
        _slot1.style.left = offsetLeft1;
        _slot2.style.left = 0;
        text.style.marginTop = 0;

    }

    private void ClearSlot2(VisualElement _slot2)
    {
        _slot2.Clear();
        _enchant2 = ChangeSlotBoxTypeChoice(_slot2, _enchant2);
    }

    private void ResetProgressBar(VisualElement progressBar)
    {
        progressBar.style.width = Convert.ToSingle(245);
    }

    private void StartFinalEvent(float fadeStartTime, float elapsedTime, VisualElement element1)
    {
        if (elapsedTime >= fadeStartTime)
        {
            AlphaChanged(fadeStartTime, elapsedTime, element1);
            //PlayAnim(_loadingAnimPng, _animElement , fadeStartTime);
        }
            
    }

    public ItemInstance GetItemByPosition(int position)
    {
        if(position == 0)
        {
           return  _enchant1.ItemInstance;
        }else if (position == 1)
        {
            return _enchant2.ItemInstance;
        }

        return null;
    }
    private async void WorkerAnim(VisualElement animElement, float fadeStartTime, bool isSuccess)
    {
        if (!_isStartAnim)
        {
            animElement.style.display = DisplayStyle.Flex;
            //offset 32x32 icon up 10px
            animElement.style.marginTop = -14;
            _isStartAnim = true;

            if (await PlayAnim(_loadingAnimWaitPng, animElement, fadeStartTime))
            {
                if (isSuccess)
                {
                    _textLabel.style.marginTop = 5;
                    var itemInstance =  PlayerInventory.Instance.GetItem(_selectItem.ObjectId);
                    SetTextLabel(itemInstance);
                    await PlayAnim(_loadingAnimSuccessPng, animElement, 0.7f);
                }
                else
                {
                    SetTextLabelFail();
                    SetIconGradeCrystal(_selectItem.GetItemGrade());
                    await PlayAnim(_loadingAnimFailPng, animElement, 0.7f);
                }
            }

            animElement.style.display = DisplayStyle.None;
            animElement.style.marginTop = 0;
        }
  
    }

    private void SetIconGradeCrystal(ItemGrade itemGrade)
    {
        if(itemGrade == ItemGrade.d)
        {
            _enchant2.AssignItem(new ItemInstance(-1, 1458, ItemLocation.Inventory, 2, 1, ItemCategory.Item, false, ItemSlot.none, 0, 9999));
        }
        else if(itemGrade == ItemGrade.c)
        {
            _enchant2.AssignItem(new ItemInstance(-1, 1459, ItemLocation.Inventory, 2, 1, ItemCategory.Item, false, ItemSlot.none, 0, 9999));
        }
    }

    private void SetTextLabel(ItemInstance itemInstance)
    {
        if (itemInstance != null && itemInstance.EnchantLevel != 0)
        {
            _textLabel.text = "Success! The Item is now " + itemInstance.EnchantLevel;
        }
        else
        {
            _textLabel.text = "Success! The Item is now +3";
        }
    }

    private void SetTextLabelFail()
    {
        _textLabel.text = "Failed! You have obtained " + 0 + " of Crystal (C-Grade)";
    }

    private async Task<bool> PlayAnim(List<Texture2D> listTextures, VisualElement animElement , float fadeStartTime)
    {

       foreach (Texture2D texture in listTextures)
       {
          animElement.style.backgroundImage = new StyleBackground(texture);
          float delayPerIteration = fadeStartTime / listTextures.Count * 1000;
          await Task.Delay((int)delayPerIteration);
       }

        return true;
    }


    

    private void AlphaChanged(float fadeStartTime , float elapsedTime , VisualElement element1)
    {
        if (elapsedTime >= fadeStartTime)
        {
            float fadeT = (elapsedTime - fadeStartTime) / (_moveDuration - fadeStartTime);
            float alpha = Mathf.Lerp(1f, 0f, fadeT);
            element1.style.opacity = alpha; 
        }
    }

    private void SetOpacity0(VisualElement element1)
    {
        element1.style.opacity = 0;
    }
}

