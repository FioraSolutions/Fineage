using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class PledgePowerCheckBoxInstaller
{
    private static SettingCheckBox[] CreateHeaders() => new[]
    {
        new SettingCheckBox("System Privileges", false, false),
        new SettingCheckBox("Clan Hall Privileges", false, false),
        new SettingCheckBox("Castle/Fortress", false, false)
    };

    private static void CreateAndDisplayPanels(ModelPowerCheckBox model, List<List<SettingCheckBox>> left, List<List<SettingCheckBox>> right)
    {
        var elements = new CheckBoxRootElements(model.Element, left, right);
        model.CreatePanelCheckBox.CreateTwoPanels(elements);
    }

    private static (List<List<SettingCheckBox>> left, List<List<SettingCheckBox>> right) PrepareCheckBoxLists(ModelPowerCheckBox model, bool defaultChecked, bool defaultDisabled)
    {
        var headers = CreateHeaders();
        return (
            CheckBoxInstaller.CreateAllLeft1(model.LeftCheckBoxes, defaultChecked, defaultDisabled, headers),
            CheckBoxInstaller.CreateAllRight1(model.RightCheckBoxes, defaultChecked, defaultDisabled, headers)
        );
    }

    private static bool TrySetPrivilege(List<List<SettingCheckBox>> checkBoxLists, int privilege, bool defaultChecked, bool defaultDisabled)
    {
        foreach (var checkBoxList in checkBoxLists)
        {
            var matchingCheckBox = checkBoxList.FirstOrDefault(cb => cb.GetData() == privilege);
            if (matchingCheckBox != null)
            {
                var header = matchingCheckBox.GetMyHeaderSetting();
                header?.SetChecked(defaultChecked);
                header?.SetDisabled(defaultDisabled);
                //matchingCheckBox.SetChecked(true);
                matchingCheckBox.SetChecked(defaultChecked);
                matchingCheckBox.SetDisabled(defaultDisabled);
                return true;
            }
        }
        return false;
    }

    private static void ApplyPrivileges(int[] splitData, List<List<SettingCheckBox>> left, List<List<SettingCheckBox>> right, bool defaultChecked, bool defaultDisabled)
    {
        foreach (int privilege in splitData)
        {
            if (!TrySetPrivilege(left, privilege, defaultChecked, defaultDisabled))
                TrySetPrivilege(right, privilege, defaultChecked, defaultDisabled);
        }
    }

    private static void CreateDefaultElements(ModelPowerCheckBox model, bool defaultChecked, bool defaultDisabled, ref CheckBoxRootElements elements)
    {
        if (elements == null)
        {
            // Debug.Log($"Create default elements (Checked: {defaultChecked}, Disabled: {defaultDisabled})");
            model.LeftCheckBoxes = new List<List<SettingCheckBox>>();
            model.RightCheckBoxes = new List<List<SettingCheckBox>>();
            var headers = CreateHeaders();

            elements = new CheckBoxRootElements(
                model.Element,
                CheckBoxInstaller.CreateAllLeft1(model.LeftCheckBoxes, defaultChecked, defaultDisabled, headers),
                CheckBoxInstaller.CreateAllRight1(model.RightCheckBoxes, defaultChecked, defaultDisabled, headers)
            );
        }

        elements.RefreshElements(model.Element);
        model.LeftCheckBoxes = elements.GetLeftAll();
        model.RightCheckBoxes = elements.GetRightAll();
        model.CreatePanelCheckBox.CreateTwoPanels(elements);
    }
}
