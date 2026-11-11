using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
    private TemplateContainer SwitchToSkill()
    {
        _content.Clear();
        var weapon = _windowTemplateSkill.CloneTree();
        _content.Add(weapon);
        _contentInside = weapon.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return weapon;
    }

    private TemplateContainer SwitchToAccessories()
    {
        _content.Clear();
        var _accessories = _windowTemplateAcccesories.CloneTree();
        _content.Add(_accessories);
        _contentInside = _accessories.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return _accessories;
    }

    private TemplateContainer SwitchToRequiredAccessories()
    {
        _content.Clear();
        var _accessories = _windowTemplateRequiredAcccesories.CloneTree();
        _content.Add(_accessories);
        _contentInside = _accessories.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return _accessories;
    }

    private TemplateContainer SwitchToArmor()
    {
        _content.Clear();
        var _accessories = _windowTemplateArmor.CloneTree();
        _content.Add(_accessories);
        _contentInside = _accessories.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return _accessories;
    }
}
