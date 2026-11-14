using UnityEngine;
using System.Collections.Generic;

public static partial class CharTemplateRegistry
{
    static bool TryResolveModel(int classId, out BaseStatModel model)
    {
        if (Classes.TryGetValue(classId, out model))
            return true;

        bool mage = CharacterClassParser.IsMage(classId);
        int archetype = (int)ClassId.FIGHTER;
        switch (MapClassId.GetCharacterRace(classId))
        {
            case CharacterRace.Elf:
                archetype = mage ? (int)ClassId.ELVEN_MAGE : (int)ClassId.ELVEN_FIGHTER;
                break;
            case CharacterRace.DarkElf:
                archetype = mage ? (int)ClassId.DARK_MAGE : (int)ClassId.DARK_FIGHTER;
                break;
            case CharacterRace.Orc:
                archetype = mage ? (int)ClassId.ORC_MAGE : (int)ClassId.ORC_FIGHTER;
                break;
            case CharacterRace.Dwarf:
                archetype = (int)ClassId.DWARVEN_FIGHTER;
                break;
            default:
                archetype = mage ? (int)ClassId.MAGE : (int)ClassId.FIGHTER;
                break;
        }

        return Classes.TryGetValue(archetype, out model);
    }
}
