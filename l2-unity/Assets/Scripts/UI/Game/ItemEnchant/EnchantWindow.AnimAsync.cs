using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using static L2Slot;
using System.Collections;
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
}
