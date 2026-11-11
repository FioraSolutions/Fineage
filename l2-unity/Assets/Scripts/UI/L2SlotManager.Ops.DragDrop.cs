using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2SlotManager
{






    #region SlotVerification



    #endregion

    #region DragActions



    private enum PendingItemOp
    {
        None,
        Destroy,
        Drop
    }

    private PendingItemOp _pendingOp = PendingItemOp.None;
    private Vector3 _pendingDropWorldPos;
    private bool _hasPendingDropPos;


    /// <summary>Mouse ray → world hit (same idea as ClickManager move pick).</summary>

    private InventorySlot _slotTemp = null;








}
