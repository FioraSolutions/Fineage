using UnityEngine;

public sealed partial class ItemDropWeaponAligner
{
    public ItemDropWeaponAligner(ItemDropGrpCatalog grp)
    {
        _grp = grp;
    }

    public bool AlignBladeAlong(Transform root, Vector3 worldDir)
    {
        if (worldDir.sqrMagnitude < 1e-12f)
            return false;
        if (!TryGetBladeMarkers(root, out Transform bladeBase, out Transform bladeTip))
            return false;

        Vector3 blade = bladeTip.position - bladeBase.position;
        if (blade.sqrMagnitude < 1e-12f)
            return false;

        root.rotation = Quaternion.FromToRotation(blade.normalized, worldDir.normalized) * root.rotation;
        return true;
    }

    public bool AlignBladeTipDown(Transform root, Vector3 throwDir)
    {
        Vector3 desired = Vector3.down;
        Vector3 flat = new Vector3(throwDir.x, 0f, throwDir.z);
        if (flat.sqrMagnitude > 0.0001f)
            desired = (Vector3.down + flat.normalized * 0.45f).normalized;
        return AlignBladeAlong(root, desired);
    }

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
        float mul = target / current;
        bool skip = current >= target * 0.55f;
        if (!skip)
        {
            mul = Mathf.Clamp(mul, 0.25f, 64f);
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
