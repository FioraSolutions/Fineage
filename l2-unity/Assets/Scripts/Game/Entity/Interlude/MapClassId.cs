
using System.Collections.Generic;
using UnityEngine;


public class MapClassId
{
    private static Dictionary<int, ClassIdTemplate> dict = new Dictionary<int, ClassIdTemplate>();
    private static readonly string Fighter = "Fighter";
    private static readonly string Human = "Human";
    private static readonly string Ork = "Orc";
    private static readonly string Dwarf = "Dwarf";
    private static readonly string Dark_Elf = "Dark Elf";
    private static readonly string Elf = "Elf";


    public static void Init()
    {
        if(dict.Count == 0)
        {
            //Human
            AddClassId((int)ClassId.FIGHTER, false, InterludeRace.HUMAN, null);
            //AddClassId((int)ClassId.WARRIOR, false, InterludeRace.HUMAN, GetClassId((int)ClassId.FIGHTER));
            AddClassId((int)ClassId.MAGE, true, InterludeRace.HUMAN, null);

            //Elven
            AddClassId((int)ClassId.ELVEN_FIGHTER, false, InterludeRace.ELF, null);
            AddClassId((int)ClassId.ELVEN_MAGE, true, InterludeRace.ELF, null);

            //Dark Elven
            AddClassId((int)ClassId.DARK_FIGHTER, false, InterludeRace.DARK_ELF, null);
            AddClassId((int)ClassId.DARK_MAGE, true, InterludeRace.DARK_ELF, null);

            //Ork
            AddClassId((int)ClassId.ORC_FIGHTER, false, InterludeRace.ORC, null);
            AddClassId((int)ClassId.ORC_MAGE, true, InterludeRace.ORC, null);


            //DWARVEN
            AddClassId((int)ClassId.DWARVEN_FIGHTER, false, InterludeRace.DWARF, null);
        }
    }

    public static CharacterRace GetRace(int race)
    {
        if(race == 0)
        {
            return CharacterRace.Human;
        }else if (race == 1)
        {
            return CharacterRace.Elf;
        }
        else if (race == 2)
        {
            return CharacterRace.DarkElf;
        }
        else if (race == 3)
        {
            return CharacterRace.Orc;
        }
        else if (race == 4)
        {
            return CharacterRace.Dwarf;
        }

        // Debug.Log("Critical error: Not Found Race!!!");
        return 0;
    }
    /// <summary>
    /// ClassId → race for all Interlude trees including 3rd profession.
    /// Unknown ids return Human — never DarkElf (that silent fallback remapped Humans).
    /// Prefer server Race from UserInfo/EntitySpawn when available.
    /// </summary>
    public static CharacterRace GetCharacterRace(int serverClassId)
    {
        // Human: 0–17, 3rd 88–98
        if ((serverClassId >= (int)ClassId.FIGHTER && serverClassId <= (int)ClassId.PROPHET) ||
            (serverClassId >= (int)ClassId.DUELIST && serverClassId <= (int)ClassId.HIEROPHANT))
            return CharacterRace.Human;
        // Elf: 18–30, 3rd 99–105
        if ((serverClassId >= (int)ClassId.ELVEN_FIGHTER && serverClassId <= (int)ClassId.ELDER) ||
            (serverClassId >= (int)ClassId.EVA_TEMPLAR && serverClassId <= (int)ClassId.EVA_SAINT))
            return CharacterRace.Elf;
        // Dark Elf: 31–43, 3rd 106–112
        if ((serverClassId >= (int)ClassId.DARK_FIGHTER && serverClassId <= (int)ClassId.SHILLIEN_ELDER) ||
            (serverClassId >= (int)ClassId.SHILLIEN_TEMPLAR && serverClassId <= (int)ClassId.SHILLIEN_SAINT))
            return CharacterRace.DarkElf;
        // Orc: 44–52, 3rd 113–116
        if ((serverClassId >= (int)ClassId.ORC_FIGHTER && serverClassId <= (int)ClassId.WARCRYER) ||
            (serverClassId >= (int)ClassId.TITAN && serverClassId <= (int)ClassId.DOOMCRYER))
            return CharacterRace.Orc;
        // Dwarf: 53–57, 3rd 117–118
        if ((serverClassId >= (int)ClassId.DWARVEN_FIGHTER && serverClassId <= (int)ClassId.WARSMITH) ||
            (serverClassId >= (int)ClassId.FORTUNE_SEEKER && serverClassId <= (int)ClassId.MAESTRO))
            return CharacterRace.Dwarf;

        return CharacterRace.Human;
    }

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
    //"Human", "Elf", "Dark Elf", "Orc", "Dwarf"
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
