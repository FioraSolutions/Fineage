public enum ObjectType
{
    Weapon,
    Armor,
    Face,
    Arrow,
    /// <summary>Whole city NPC prefab.</summary>
    Npc,
    /// <summary>Whole monster prefab (leave-range + post-death corpse cleanup).</summary>
    Monster,
    /// <summary>Remote player body (CharInfo UserEntity).</summary>
    User
}
