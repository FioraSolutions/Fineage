using System.Collections.Generic;

public partial class MapClassId
{
    private static Dictionary<int, ClassIdTemplate> dict = new Dictionary<int, ClassIdTemplate>();
    private static readonly string Fighter = "Fighter";
    private static readonly string Human = "Human";
    private static readonly string Ork = "Orc";
    private static readonly string Dwarf = "Dwarf";
    private static readonly string Dark_Elf = "Dark Elf";
    private static readonly string Elf = "Elf";



    /// <summary>
    /// ClassId → race for all Interlude trees including 3rd profession.
    /// Unknown ids return Human — never DarkElf (that silent fallback remapped Humans).
    /// Prefer server Race from UserInfo/EntitySpawn when available.
    /// </summary>



    //"Human", "Elf", "Dark Elf", "Orc", "Dwarf"



}
