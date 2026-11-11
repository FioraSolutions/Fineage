using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractClanContent : AbstractButtonEvents
{
    protected DataProviderClanInfo _dataProvider;
    public VisualTreeAsset template;

    public const string USS_STYLE_YELLOW = "button-label-yellow";
    public const string USS_STYLE_DISABLED = "button-label-disabled";

    protected VisualElement content;
    public Action<int> OnClickHide;
    public Action<int> OnClickApply;





}
