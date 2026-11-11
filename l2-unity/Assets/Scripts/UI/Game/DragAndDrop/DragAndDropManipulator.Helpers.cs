using UnityEngine;
using UnityEngine.UIElements;

public partial class DragAndDropManipulator : PointerManipulator
{
    private void SetActiveToManager(VisualElement active)
    {
        if(active != null) manager.SetActive(active.name);
    }

    private void PointerMoveHandler(PointerMoveEvent evt)
    {
        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            //UnityEngine.Cursor.visible = false;
            // Vector3 pointerDelta = evt.position - pointerStartPosition;
            Vector3 pointerDelta = evt.position;
            var position = new Vector2(
                Mathf.Clamp(targetStartPosition.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
                Mathf.Clamp(targetStartPosition.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height));
            DragIconPacketServices.Icon.NewPosition(position);
            DragIconPacketServices.Icon.BringToFront1();

        }
    }

    private void PointerUpHandler(PointerUpEvent evt)
    {
        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            target.ReleasePointer(evt.pointerId);
            VisualElement ve = (VisualElement)evt.currentTarget;
            DragIconPacketServices.Icon.ResetPosition();
        }
    }

    private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
    {
        if (enabled)
        {
            //Debug.Log("PointerCaptureOutHandler DRAGGGGANDDROPPPP");
            //VisualElement slotsContainer = root.Q<VisualElement>("AllPanel");

           // UQueryBuilder<VisualElement> allSlots =
            //    slotsContainer.Query<VisualElement>(className: "imgbox0");

           // UQueryBuilder<VisualElement> overlappingSlots =
            //    allSlots.Where(OverlapsTarget);

            //VisualElement closestOverlappingSlot =
             //   FindClosestSlot(overlappingSlots);

           // Vector3 closestPos = Vector3.zero;
           //// if (closestOverlappingSlot != null)
           // {
            //    closestPos = RootSpaceOfSlot(closestOverlappingSlot);
           //     closestPos = new Vector2(closestPos.x - 5, closestPos.y - 5);
          //  }
           // target.transform.position =
           //     closestOverlappingSlot != null ?
            //    closestPos :
            //    targetStartPosition;

            enabled = false;
        }
    }

    private bool OverlapsTarget(VisualElement slot)
    {
        return target.worldBound.Overlaps(slot.worldBound);
    }
}
