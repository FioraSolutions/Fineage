using System.Collections.Generic;
using UnityEngine.UIElements;

public partial class PrivilegesInfoContent : AbstractClanContent
{
    private void PrepareUsePowerGrade(int powerGrade , VisualElement _panelPrivilages , VisualElement _panelClanHall, VisualElement _panelCastle)
    {
        //var elements = new VisualElement[] { _panelPrivilages, _panelClanHall, _panelCastle };
        UsePowerGrade(powerGrade, _panelPrivilages, _panelClanHall, _panelCastle);
        //switch (powerGrade)
        //{
        //case -1:
        //  CheckBoxInstaller.UsePowerGradeMinus1(_createPanelCheckBox , _leftCheckBoxes , _rightCheckBoxes, elements);
        //  break;
        //case 1:
        // CheckBoxInstaller.UsePowerGrade1(_createPanelCheckBox, _leftCheckBoxes, _rightCheckBoxes, elements);
        /// break;
        // case 6:
        // CheckBoxInstaller.UsePowerGrade6(_createPanelCheckBox, _leftCheckBoxes, _rightCheckBoxes, elements);
        // break;
        //default:
        //   break;
        //}
    }

    private void UsePowerGrade(int powerGrade, VisualElement panelPrivilages, VisualElement panelClanHall, VisualElement panelCastle)
    {
        PledgePowerCheckBoxInstaller.CreateCheckboxUsePowerGrade(powerGrade,
            new ModelPowerCheckBox(_createPanelCheckBox, _leftCheckBoxes, _rightCheckBoxes, new VisualElement[]
        { panelPrivilages, panelClanHall, panelCastle }), true, true);
    }

    private void UseRank(int rank, VisualElement panelPrivilages, VisualElement panelClanHall, VisualElement panelCastle)
    {
        PledgePowerCheckBoxInstaller.CreateCheckboxUsePowerRanked(rank , 
            new ModelPowerCheckBox(_createPanelCheckBox , _leftCheckBoxes , _rightCheckBoxes , new VisualElement[]
            { panelPrivilages, panelClanHall, panelCastle }), true, false);
    }
}
