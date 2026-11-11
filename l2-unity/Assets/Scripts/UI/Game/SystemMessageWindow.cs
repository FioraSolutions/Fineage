using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

using UnityEngine.UIElements;
using VContainer;

public partial class SystemMessageWindow : L2PopupWindow
{
    private static SystemMessageWindow _instance;
    protected VisualTreeAsset _windowTemplateDropdown;
    private const string _dropDownTemplateName = "SystemMessageComboBox";
    private const string _defaultNameCancel = "CancelButton";
    private const string _defaultNameOk = "OkButton";
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SystemMessageWindow Instance { get { return _instance; } }
    private UnityEngine.UIElements.Label _textLabel;
    private Button _cancelButton;
    private Button _okButton;

    public event Action OnButtonOk;
    public event Action OnButtonClosed;

    /// <summary>Drop prior OK/Cancel listeners before wiring a new yes/no prompt.</summary>





















    

}
