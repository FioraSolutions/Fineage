using UnityEngine;

public sealed partial class ItemDropWeaponAligner
{
    public void PlantStuckInGround(ItemEntity item, Vector3 landPos)
    {
        if (item == null)
            return;

        item.transform.position = landPos;

        if (TryGetBladeMarkers(item.transform, out Transform bladeBase, out Transform bladeTip))
        {
            float bladeLen = Vector3.Distance(bladeBase.position, bladeTip.position);
            float bury = Mathf.Clamp(bladeLen * 0.22f, 0.04f, 0.22f);
            float dy = (landPos.y - bury) - bladeTip.position.y;
            item.transform.position += Vector3.up * dy;
            return;
        }

        Renderer renderer = item.GetComponentInChildren<Renderer>();
        if (renderer == null)
            return;

        Bounds bounds = renderer.bounds;
        float height = Mathf.Max(bounds.size.y, 0.01f);
        float fallbackBury = Mathf.Clamp(height * 0.22f, height * 0.08f, height * 0.35f);
        float fallbackDy = landPos.y - bounds.min.y - fallbackBury;
        item.transform.position += Vector3.up * fallbackDy;
    }

    public void ScaleWeapon(GameObject visual, int itemId)
    {
        if (visual == null)
            return;

        float current = 0f;
        string currentSrc = "none";
        if (TryGetBladeMarkers(visual.transform, out Transform bladeBase, out Transform bladeTip))
        {
            current = Vector3.Distance(bladeBase.position, bladeTip.position);
            currentSrc = $"blade:{bladeBase.name}->{bladeTip.name}";
        }
        if (current < 0.0001f)
        {
            current = LongestMeshAxis(visual);
            currentSrc = "meshAxis";
        }
        if (current < 0.0001f)
        {
            // Debug.LogWarning($"[DropItemScale] SCALE skip itemId={itemId} current=0 src={currentSrc}");
            return;
        }

        float equipped = TryEquippedWeaponWorldLength(itemId, out string equippedSrc);
        float defaultLen = DefaultWeaponWorldLength(itemId);
        float target = equipped > 0.05f ? equipped * 0.8f : defaultLen;
        target /= 3f;
        target *= 1.3f;
        Vector3 scaleBefore = visual.transform.localScale;

        // `current` is already in meters (mesh bounds x lossyScale), so the only correct
        // thing is to move it to `target` — the drop is a fixed fraction of the weapon's
        // real size, independent of the FBX unit contract it came from.
        //
        // There used to be a `skip = current >= target * 0.55f` guard here. It only ever
        // fired because weapon meshes were tiny (legacy contract: a staff measured
        // 0.0094 u), so the guard read "already big enough" only after a 100x error.
        // Once weapons moved to the meter contract the same guard started firing on
        // every weapon and left drops at full real size (0.94 m staff instead of 0.17 m).
        float mul = target / current;
        if (mul > 0.0001f && !Mathf.Approximately(mul, 1f))
        {
            // The old lower bound of 0.25 assumed the mesh always had to grow. Meter
            // meshes have to shrink by the full ratio, so only the upper bound stays.
            mul = Mathf.Clamp(mul, 0.0001f, 64f);
            visual.transform.localScale *= mul;
        }

        // Debug.Log(
            // $"[DropItemScale] SCALE itemId={itemId} prefab='{visual.name}' " +
            // $"current={current:F3}({currentSrc}) equipped={equipped:F3}({equippedSrc}) " +
            // $"default={defaultLen:F3} target={target:F3} skip={skip} mul={mul:F2} " +
            // $"scale {scaleBefore} → {visual.transform.localScale} " +
            // $"worldAxis={LongestMeshAxis(visual):F3}");
    }

    public static void ScaleProp(GameObject visual, float targetMeters)
    {
        if (visual == null || targetMeters < 0.01f)
            return;

        float current = LongestMeshAxis(visual);
        if (current < 0.0001f)
            return;
        if (current >= targetMeters * 0.55f)
            return;

        visual.transform.localScale *= targetMeters / current;
    }
}
