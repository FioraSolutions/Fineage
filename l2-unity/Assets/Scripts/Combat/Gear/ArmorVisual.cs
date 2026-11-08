using UnityEngine;
using static ModelTable;

/// <summary>
/// Resolved body-armor look for one item + race. No parenting / pool / dresser state.
/// </summary>
public sealed class ArmorVisual
{
    public int ItemId { get; }
    public Armor Armor { get; }
    public ItemSlot Slot { get; }
    public bool IsMissing { get; }
    public string MissingReason { get; }
    public ArmorVisualPart[] Parts { get; }

    /// <summary>Transitional cache handle for callers still on <see cref="L2ArmorPiece"/>.</summary>
    public L2ArmorPiece LegacyPiece { get; }

    ArmorVisual(
        int itemId,
        Armor armor,
        ItemSlot slot,
        bool isMissing,
        string missingReason,
        ArmorVisualPart[] parts,
        L2ArmorPiece legacyPiece)
    {
        ItemId = itemId;
        Armor = armor;
        Slot = slot;
        IsMissing = isMissing;
        MissingReason = missingReason ?? string.Empty;
        Parts = parts ?? System.Array.Empty<ArmorVisualPart>();
        LegacyPiece = legacyPiece;
    }

    public static ArmorVisual Ok(
        Armor armor,
        ItemSlot slot,
        ArmorVisualPart[] parts,
        L2ArmorPiece legacyPiece)
    {
        return new ArmorVisual(
            armor != null ? armor.Id : 0,
            armor,
            slot,
            false,
            string.Empty,
            parts,
            legacyPiece);
    }

    public static ArmorVisual Missing(int itemId, Armor armor, ItemSlot slot, string reason)
    {
        return new ArmorVisual(
            itemId,
            armor,
            slot,
            true,
            reason,
            System.Array.Empty<ArmorVisualPart>(),
            null);
    }
}

public readonly struct ArmorVisualPart
{
    public GameObject Mesh { get; }
    public Material Material { get; }
    public ItemSlot PartSlot { get; }

    public ArmorVisualPart(GameObject mesh, Material material, ItemSlot partSlot)
    {
        Mesh = mesh;
        Material = material;
        PartSlot = partSlot;
    }
}
