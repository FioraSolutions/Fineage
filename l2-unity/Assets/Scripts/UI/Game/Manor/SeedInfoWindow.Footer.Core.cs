using System;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class SeedInfoWindow
{
    private void UnRegisterAllButtons()
    {
        if(_buttonSeeds != null) _buttonSeeds.UnregisterCallback<ClickEvent>(OnClickButtonSeed);
        if (_buttonHarvst != null) _buttonSeeds.UnregisterCallback<ClickEvent>(OnClickButtonSeed);
        if (_buttonAll1 != null) _buttonSeeds.UnregisterCallback<ClickEvent>(OnClickButtonSeed);
        if (_buttonAll2 != null) _buttonSeeds.UnregisterCallback<ClickEvent>(OnClickButtonSeed);
    }

    private void AddContent0Footer(ICreator creatorWindow , VisualElement footerContainer)
    {
        if (footerContainer != null)
        {
            if(_buttonSeeds == null)
            {
                _buttonSeeds = (Button)ToolTipsUtils.CloneOne(_buttonTemplate);
                _buttonSeeds.Q<Label>("ButtonLabel").text = "Buy seeds";
            }


            UnRegisterAllButtons();

            _buttonSeeds.RegisterCallback<ClickEvent>(OnClickButtonSeed);

            creatorWindow.InsertFooterIntoContent(_buttonSeeds, footerContainer);
        }
    }

    private void AddContent1Footer(ICreator creatorWindow, VisualElement footerContainer)
    {
        if (footerContainer != null)
        {
            if(_buttonHarvst == null)
            {
                _buttonHarvst = (Button)ToolTipsUtils.CloneOne(_buttonTemplate);
                _buttonHarvst.Q<Label>("ButtonLabel").text = "Sale of harvest";
            }
            UnRegisterAllButtons();

            _buttonHarvst.RegisterCallback<ClickEvent>(OnClickButtonHarvest);

            creatorWindow.InsertFooterIntoContent(_buttonHarvst, footerContainer);
        }
    }
}
