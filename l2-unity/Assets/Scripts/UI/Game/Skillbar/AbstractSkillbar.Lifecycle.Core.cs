using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractSkillbar
{
    public AbstractSkillbar(VisualElement skillbarWindowElement, int skillbarIndex, int page, bool horizontalBar)
    {
        _skillbarIndex = skillbarIndex;
        _skillbarWindow = skillbarWindowElement;
        _page = page;
        _horizontalBar = horizontalBar;
        _slotAnchors = new List<VisualElement>();
    }

    public IEnumerator BuildWindow(VisualTreeAsset template, VisualElement container)
    {
        _windowEle = template.Instantiate()[0];
        container.Add(_windowEle);

        yield return new WaitForEndOfFrame();

        VisualElement dragArea = _windowEle.Q<VisualElement>("DragArea");
        dragArea.AddManipulator(new DragManipulator(
            dragArea,
            _skillbarWindow,
            () => SkillbarPacketServices.Skillbar?.PersistPosition()));

        for (int i = 1; i <= 12; i++)
        {
            VisualElement anchor = _windowEle.Q<VisualElement>("SlotAnchor" + i);
            _slotAnchors.Add(anchor);
        }

        VisualElement arrowInput = _windowEle.Q<VisualElement>(null, "skill-arrow-input");
        string[] arrowInputValues = new string[10];
        for (int i = 1; i <= 10; i++)
        {
            arrowInputValues[i - 1] = i.ToString();
        }

        _arrowInputManipulator = new ArrowInputManipulator(arrowInput, null, arrowInputValues, _page,
        (index, value) => SkillbarPacketServices.Skillbar.OnPageChanged(SkillbarIndex, index));

        UpdateVisuals();
        ResetShortcuts();
    }
}
