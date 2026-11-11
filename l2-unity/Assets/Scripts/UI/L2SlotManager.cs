using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2SlotManager : L2PopupWindow
{

    [SerializeField] private L2Slot _draggedSlot;
    [SerializeField] private L2Slot _hoverSlot;
    private L2Slot _dragSlotData;

    private static L2SlotManager _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static L2SlotManager Instance { get { return _instance; } }



    








    /// <summary>
    /// Places the floating drag icon so <paramref name="grabOffset"/> (local to the source slot)
    /// stays under the pointer. <paramref name="panelPos"/> is UI Toolkit panel space.
    /// </summary>


    /// <summary>Abort an in-progress or primed slot drag without applying a drop.</summary>

}
