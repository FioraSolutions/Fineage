
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

using UnityEngine.UIElements;



public partial class ActiveSkillsHide : AbstractSkills
{
    private VisualElement _activeSkillPanel;
    private VisualTreeAsset _templateBoxPanel;
    private VisualTreeAsset _templatePanel8x1;
    private List<VisualElement> _boxPanelsPhysical;
    private List<VisualElement> _boxPanelsMagic;

    private const string _rowNamePhysical = "RowPhysical";
    private const string _rowNameMagical = "RowMagical";

    private const string _allContentPhysical = "PhysicalContent";
    private const string _allContentMagical = "MagicContent";
    private List<SkillInstance> _tempList;
    private VisualTreeAsset _templateSlotSkill;
    private int[] _arrDfActiveSelect;

    private VisualElement _rowPhysical;
    private VisualElement _rowMagical;

}
