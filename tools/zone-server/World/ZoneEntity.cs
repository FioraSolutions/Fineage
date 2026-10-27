namespace Fineage.ZoneServer.World;

public sealed class ZoneEntity
{
    public required int ObjectId { get; init; }
    public required ZoneEntityKind Kind { get; init; }
    public required string Name { get; init; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    /// <summary>Interlude NPC template id (NPCs only).</summary>
    public int NpcTemplateId { get; init; }
    public int ClassId { get; init; }
}
