using UnityEngine;
using UnityEngine.UIElements;

public partial class GearSlot : InventorySlot
{
    private VisualElement _blackElement;
    private string _blackElementName = "backOverlay";


    /// <summary>
    /// Interlude client inventory semantics: a single left click only selects the slot and shows the
    /// item description (the base handler calls <c>SelectSlot</c>, which raises
    /// <c>ToolTipSimple.EventLeftClickSlot</c>). The double click — two clicks inside
    /// <c>InventorySlot</c>'s 0.3s window — routes to <see cref="UseItem"/> → <see cref="UnequipWorn"/>,
    /// and the right click unequips immediately.
    ///
    /// <para>
    /// This used to call <c>UnequipWorn()</c> on every left click, so inspecting a worn item
    /// silently took it off instead of showing what it was.
    /// </para>
    /// </summary>









    /// <summary>
    /// Interlude paper doll keeps empty silhouettes on Inventory_Back; do not stamp the DF
    /// empty-slot chrome over them.
    /// </summary>

}
