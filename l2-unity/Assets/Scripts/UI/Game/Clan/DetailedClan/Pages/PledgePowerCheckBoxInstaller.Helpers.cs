using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public partial class PledgePowerCheckBoxInstaller
{
    private static void UsePowerGradeCpAll(ModelPowerCheckBox model, bool defaultChecked, bool defaultDisabled)
    {
        var headers = CreateHeaders();
        CreateAndDisplayPanels(model,
            CheckBoxInstaller.CreateAllLeft1(model.LeftCheckBoxes, defaultChecked, defaultDisabled, headers),
            CheckBoxInstaller.CreateAllRight1(model.RightCheckBoxes, defaultChecked, defaultDisabled, headers));
    }

    private static void UsePowerGradeDefaultTrueTrue(ModelPowerCheckBox model, bool defaultChecked, bool defaultDisabled)
     => CreateDefaultElements(model, defaultChecked, defaultDisabled, ref _defaultTrueTrue);

    private static void UsePowerGradeDefaultFalseTrue(ModelPowerCheckBox model, bool defaultChecked, bool defaultDisabled)
        => CreateDefaultElements(model, defaultChecked, defaultDisabled, ref _defaultFalseTrue);

    private static void HandlePowerNotAll(int powerValue, ModelPowerCheckBox model, bool defaultChecked, bool defaultDisabled, bool isGrade)
    {
        if (powerValue == 0)
        {
            if (isGrade)
                UsePowerGradeDefaultFalseTrue(model, false, true);
            else
                UsePowerGradeCpAll(model, false, false);
            return;
        }

        if (powerValue == ClanPrivileges.CP_ALL)
        {
            if(isGrade)
                UsePowerGradeDefaultTrueTrue(model, true, true);
            else
                UsePowerGradeCpAll(model, true, isGrade);
            return;
        }

        var (left, right) = PrepareCheckBoxLists(model, false, isGrade);
        ApplyPrivileges(ByteUtils.SplitMask(powerValue), left, right, defaultChecked, defaultDisabled);
        CreateAndDisplayPanels(model, left, right);
    }

    public static void CreateCheckboxUsePowerGrade(int powerGrade, ModelPowerCheckBox model, bool defaultChecked, bool defaultDisabled)
        => HandlePowerNotAll(powerGrade, model, defaultChecked, defaultDisabled, true);

    public static void CreateCheckboxUsePowerRanked(int powerRanked, ModelPowerCheckBox model, bool defaultChecked, bool defaultDisabled)
        => HandlePowerNotAll(powerRanked, model, defaultChecked, defaultDisabled, false);
}
