using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public abstract partial class L2Window : MonoBehaviour
{
    protected VisualElement _root;
    protected VisualTreeAsset _windowTemplate;

    protected VisualElement _windowEle;
    protected bool _isWindowHidden = false;
    protected MouseOverDetectionManipulator _mouseOverDetection;
    protected List<string> _listDropDown;


    protected abstract void LoadAssets();



    protected bool _isCenter = false;
    protected bool _isReg = false;


    protected float defaultWidth = 0;
    protected float defaultHeight = 0;









    private DropdownFieldUtils dropDownUtils = new DropdownFieldUtils();
    private System.Action<bool> onOpen;
    private System.Action<bool> onClose;
    private bool _IsCloseCreate = false;

    //Algorutm Dropdown disabled click MoveTo
    //_IsCloseCreate > The flag that indicates that we are entering the mode of selecting the desired element from the dropdown list.
    //OnClose(bool check) > OnClose is triggered if the list is closed; it will be triggered even if there was no value in the list.
    //Event Running: 1. OnMoveEvent 2.OnLeaveEvent 3.OnMouseOutFocus 4. OnClose 






    //special for dropfield






    protected abstract IEnumerator BuildWindow(VisualElement root);














}
