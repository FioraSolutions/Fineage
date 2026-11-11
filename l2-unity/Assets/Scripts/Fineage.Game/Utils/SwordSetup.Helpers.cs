using UnityEngine;

public partial class SwordSetup : MonoBehaviour
{
    private Transform CreateOrGetPoint(Transform parent, string pointName)
    {
        Transform existing = parent.Find(pointName);
        if (existing == null)
        {
            existing = transform.Find(pointName);
        }

        if (existing != null)
        {
            existing.SetParent(parent, false);
            return existing;
        }

        GameObject newPoint = new GameObject(pointName);
        newPoint.transform.SetParent(parent, false);
        return newPoint.transform;
    }
}
