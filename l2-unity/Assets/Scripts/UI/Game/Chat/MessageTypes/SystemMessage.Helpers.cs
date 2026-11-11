using System;
using UnityEngine;
using static SMParam;

public partial class SystemMessage
{
    private string GetParameterReplacement(SMParam param, int index)
    {
        switch (param.Type)
        {
            case SMParamType.TYPE_TEXT:
            case SMParamType.TYPE_PLAYER_NAME:
                return param.GetStringValue();

            case SMParamType.TYPE_LONG_NUMBER:
            case SMParamType.TYPE_INT_NUMBER:
            case SMParamType.TYPE_CASTLE_NAME:
            case SMParamType.TYPE_NPC_NAME:
            case SMParamType.TYPE_ELEMENT_NAME:
            case SMParamType.TYPE_SYSTEM_STRING:
            case SMParamType.TYPE_INSTANCE_NAME:
            case SMParamType.TYPE_DOOR_NAME:
                return param.GetIntValue().ToString();

            case SMParamType.TYPE_ITEM_NAME:
                return GetItemName(param.GetIntValue());

            case SMParamType.TYPE_SKILL_NAME:
                return GetSkillName(param.GetIntArrayValue());

            default:
                return null;
        }
    }

    private string GetItemName(int itemId)
    {
        try
        {
            return TablePacketServices.ItemNames.GetItemName(itemId)?.DisplayName ?? "";
        }
        catch (Exception)
        {
            // Debug.LogError($"Failed to get item name for ID {itemId}: {e.Message}");
            return "Unknown Item";
        }
    }

    private string GetSkillName(int[] skillData)
    {
        try
        {
            if (skillData == null || skillData.Length < 2)
            {
                // Debug.LogError("Invalid skill data array");
                return "Unknown Skill";
            }

            var skill = TablePacketServices.SkillNames.GetName(skillData[0], skillData[1]);

            if(skill != null)
            {
                return skill.Name;
            }

            return "Unknown Skill Skill ID " + skillData[0] + " Level " + skillData[1];
        }
        catch (Exception)
        {
            // Debug.LogError($"Failed to get skill name: {e.Message}");
            return "Unknown Skill";
        }
    }

    private string ReplacePlaceholders(string value, string replacement, int index)
    {
        value = value.Replace($"$s{index}", replacement);

        // Only replace $c placeholders for text and player name types
        if (_params[index - 1].Type == SMParamType.TYPE_TEXT ||
            _params[index - 1].Type == SMParamType.TYPE_PLAYER_NAME)
        {
            value = value.Replace($"$c{index}", replacement);
        }

        return value;
    }
}
