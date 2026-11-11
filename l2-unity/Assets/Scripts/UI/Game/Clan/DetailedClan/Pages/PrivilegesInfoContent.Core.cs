using System.Collections.Generic;
using UnityEngine.UIElements;

public partial class PrivilegesInfoContent : AbstractClanContent
{
    public PrivilegesInfoContent(DataProviderClanInfo dataProvider , ICreatorPanelCheckBox createPanelCheckBox)
    {
        _dataProvider = dataProvider;
        _createPanelCheckBox = createPanelCheckBox;

        // Initialize lists once
        _leftCheckBoxes = new List<List<SettingCheckBox>>();
        _rightCheckBoxes = new List<List<SettingCheckBox>>();

    }

    public void PreShow(object packet, VisualElement detailedInfoElement)
    {
        content = LoadContent(content, detailedInfoElement);
        ClearContent(content);
        _createPanelCheckBox.DestroyTempElements();

        Show(packet, detailedInfoElement);
    }

    private void Show(object packet, VisualElement detailedInfoElement)
    {


        VisualElement page = ToolTipsUtils.CloneOne(template);


        Button cancelButton = page.Q<Button>("CancelButtonBox2");
        Button applyButton = page.Q<Button>("ApplyButtonBox2"); 

        SubscribeCloseButton(cancelButton, detailedInfoElement);


        var panelPrivilages = page.Q<VisualElement>(_panelSystemPrivilegesName);
        var panelClanHall = page.Q<VisualElement>(_panelSystemClanHallName);
        var panelCastle = page.Q<VisualElement>(_panelSystemCastleName);

        if (packet is PledgeReceivePowerInfoDto privilegesInfo)
        {
            HideElement(applyButton);
            PrepareUsePowerGrade(privilegesInfo.PowerGradeByRank, panelPrivilages, panelClanHall, panelCastle);
        }

        if (packet is ManagePledgePowerDto managePledgePower)
        {
            SubscribeApplyButton(applyButton, detailedInfoElement, managePledgePower.Rank);
            ShowElement(applyButton);

            UseRank(managePledgePower.PrivilegesByRank, panelPrivilages, panelClanHall, panelCastle);
        }


        // Clear and reuse lists
        _leftCheckBoxes.Clear();
        _rightCheckBoxes.Clear();

        if (content != null & page != null)
        {
            content.Add(page);
        }
    }
}
