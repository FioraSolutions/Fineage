using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public partial class EnchantWindow
{
    private async void  DecreaseProgressBar()
    {
        float endValue = 245;
        int index = 0;

        while (index <= endValue)
        {
            index = index + 7;
            float remaining = 1f - (index / endValue);
            L2ResourceBarView.SetFillFixed(_progressBar, _progressBarBg, remaining, endValue);
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

    /// <summary>Legacy Interlude wire result (not Net <see cref="ApplyZoneResult"/>).</summary>
    public void HandleEnchantResult(EnchantResult result)
    {
        switch (result)
        {
            case EnchantResult.Success:
                UpdateS1();
                StartEnchantAnim(true);
                break;
            case EnchantResult.Cancelled:
               HideWindow();
                break;
            case EnchantResult.UnSuccess:
                StartEnchantAnim(false);
                break;
            case EnchantResult.Unk_Result_1:
                StartEnchantAnim(false);
                break;
            case EnchantResult.Unk_Result_4:
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
        var itemInstance = InventoryPacketServices.Inventory.GetItem(_selectItem.ObjectId);
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
                    var itemInstance =  InventoryPacketServices.Inventory.GetItem(_selectItem.ObjectId);
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
