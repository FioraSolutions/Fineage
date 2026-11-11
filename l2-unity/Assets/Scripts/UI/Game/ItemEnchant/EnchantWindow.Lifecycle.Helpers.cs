using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public partial class EnchantWindow : L2PopupWindow
{
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
            animList.Add(IconPacketServices.Icons.LoadTextureOtherSources(href));
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
}
