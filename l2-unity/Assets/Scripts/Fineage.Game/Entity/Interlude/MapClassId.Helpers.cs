using System.Collections.Generic;

public partial class MapClassId
{
    public static bool IsMage(int serverClassId)
    {
        return CharacterClassParser.IsMage(serverClassId);
    }

    public static void AddClassId(int pId, bool mage , InterludeRace race, ClassIdTemplate pParent)
    {
        dict.Add(pId, new ClassIdTemplate(pId, mage, race, pParent));
    }

    public static ClassIdTemplate GetClassId(int pId)
    {
        return (dict.ContainsKey(pId)) ? dict[pId] : null;
    }

    public static ClassIdTemplate GetClassIdByName(string className ,string raceName )
    {
        if (raceName.Equals(Human))
        {
            if (className.Equals(Fighter))
            {
                return  GetClassId((int) ClassId.FIGHTER);
            }
            else
            {
                return GetClassId((int)ClassId.MAGE);
            }

        }else if (raceName.Equals(Ork))
        {
            if (className.Equals(Fighter))
            {
                return GetClassId((int)ClassId.ORC_FIGHTER);
            }
            else
            {
                return GetClassId((int)ClassId.ORC_MAGE);
            }
        }
        else if (raceName.Equals(Dwarf))
        {
            if (className.Equals(Fighter))
            {
                return GetClassId((int)ClassId.DWARVEN_FIGHTER);
            }
            else
            {
                return GetClassId((int)ClassId.DWARVEN_FIGHTER);
            }
        }
        else if (raceName.Equals(Dark_Elf))
        {
            if (className.Equals(Fighter))
            {
                return GetClassId((int)ClassId.DARK_FIGHTER);
            }
            else
            {
                return GetClassId((int)ClassId.DARK_MAGE);
            }
        }
        else if (raceName.Equals(Elf))
        {
            if (className.Equals(Fighter))
            {
                return GetClassId((int)ClassId.ELVEN_FIGHTER);
            }
            else
            {
                return GetClassId((int)ClassId.ELVEN_MAGE);
            }

        }

        return null; 
    }
}
