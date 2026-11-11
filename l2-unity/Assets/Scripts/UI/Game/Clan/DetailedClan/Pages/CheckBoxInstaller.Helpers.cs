using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CheckBoxInstaller
{
    public static List<SettingCheckBox> InitChecBoxPrivilegesLeft(bool isChecked, bool isDisabled, SettingCheckBox header)
        => CreateCheckBoxList(header, PrivilegeLeftItems, isChecked, isDisabled);

    public static List<SettingCheckBox> InitChecBoxPrivilegesRight(bool isChecked, bool isDisabled, SettingCheckBox header)
        => CreateRegularList(header, PrivilegeRightItems, isChecked, isDisabled);

    public static List<SettingCheckBox> InitChecBoxClanHallLeft(bool isChecked, bool isDisabled, SettingCheckBox header)
        => CreateCheckBoxList(header, ClanHallLeftItems, isChecked, isDisabled);

    public static List<SettingCheckBox> InitChecBoxClanHallRight(bool isChecked, bool isDisabled, SettingCheckBox header)
        => CreateRegularList(header, ClanHallRightItems, isChecked, isDisabled);

    public static List<SettingCheckBox> InitChecBoxCastleLeft(bool isChecked, bool isDisabled, SettingCheckBox header)
        => CreateCheckBoxList(header, CastleLeftItems, isChecked, isDisabled);

    public static List<SettingCheckBox> InitChecBoxCastleRight(bool isChecked, bool isDisabled, SettingCheckBox header)
        => CreateRegularList(header, CastleRightItems, isChecked, isDisabled);

    public static List<List<SettingCheckBox>> CreateAllLeft1(List<List<SettingCheckBox>> checkBoxes, bool isChecked, bool isDisabled, SettingCheckBox[] headers)
    {
        checkBoxes.AddRange(new List<List<SettingCheckBox>>
        {
            InitChecBoxPrivilegesLeft(isChecked, isDisabled, headers[0]),
            InitChecBoxClanHallLeft(isChecked, isDisabled, headers[1]),
            InitChecBoxCastleLeft(isChecked, isDisabled, headers[2])
        });
        return checkBoxes;
    }

    public static List<List<SettingCheckBox>> CreateAllRight1(List<List<SettingCheckBox>> checkBoxes, bool isChecked, bool isDisabled, SettingCheckBox[] headers)
    {
        checkBoxes.AddRange(new List<List<SettingCheckBox>>
        {
            InitChecBoxPrivilegesRight(isChecked, isDisabled, headers[0]),
            InitChecBoxClanHallRight(isChecked, isDisabled, headers[1]),
            InitChecBoxCastleRight(isChecked, isDisabled, headers[2])
        });
        return checkBoxes;
    }
}
