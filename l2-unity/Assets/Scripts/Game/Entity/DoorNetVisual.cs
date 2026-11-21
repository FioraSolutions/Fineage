using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Binds a live door entity to a static mesh and toggles collision on open/close.</summary>
public sealed class DoorNetVisual : MonoBehaviour
{
    [SerializeField] int _doorId;
    [SerializeField] int _objectId;
    [SerializeField] bool _open;

    readonly List<Collider> _colliders = new List<Collider>();
    Transform _meshRoot;

    public int DoorId => _doorId;
    public int ObjectId => _objectId;
    public bool IsOpen => _open;

    public void Init(int objectId)
    {
        _objectId = objectId;
    }

    public void BindByDoorId(int doorId)
    {
        if (doorId <= 0)
        {
            FineageLog.Error(FineageLogCategory.Net, $"DoorNetVisual: objectId={_objectId} has invalid doorId");
            return;
        }

        _doorId = doorId;
        if (!DoorCatalog.TryGetName(doorId, out string doorName))
        {
            FineageLog.Error(FineageLogCategory.Net, $"DoorNetVisual: missing doors.json row for doorId={doorId}");
            return;
        }

        Transform found = FindStaticDoorRoot(doorName);
        if (found == null)
        {
            FineageLog.Error(
                FineageLogCategory.Net,
                $"DoorNetVisual: scene mesh not found for door '{doorName}' (id={doorId})");
            return;
        }

        _meshRoot = found;
        _colliders.Clear();
        found.GetComponentsInChildren(true, _colliders);
        ApplyOpenState(_open, animate: false);
    }

    public void ApplyStatus(bool open, int curHp, int maxHp, bool attackable)
    {
        if (_doorId > 0 && _meshRoot == null)
            BindByDoorId(_doorId);

        if (open != _open)
            ApplyOpenState(open, animate: true);
        else
            _open = open;

        var entity = GetComponent<MonsterEntity>();
        if (entity != null && entity.Status != null && maxHp > 0)
        {
            entity.Stats.MaxHp = maxHp;
            entity.Status.SetHp(Mathf.Clamp(curHp, 0, maxHp));
        }

        NetNpcSession.RememberNpc(_objectId, "", attackable);
    }

    void ApplyOpenState(bool open, bool animate)
    {
        _open = open;
        for (int i = 0; i < _colliders.Count; i++)
        {
            Collider col = _colliders[i];
            if (col != null)
                col.enabled = !open;
        }

        if (_meshRoot == null)
            return;

        Animator anim = _meshRoot.GetComponentInChildren<Animator>(true);
        if (anim != null)
        {
            anim.SetBool("open", open);
            if (animate)
                anim.SetTrigger(open ? "Open" : "Close");
        }
    }

    static Transform FindStaticDoorRoot(string doorName)
    {
        if (string.IsNullOrEmpty(doorName))
            return null;

        Transform[] all = UnityEngine.Object.FindObjectsByType<Transform>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            Transform t = all[i];
            if (t == null)
                continue;
            string n = t.name;
            if (string.Equals(n, doorName, StringComparison.OrdinalIgnoreCase))
                return t;
            if (n.IndexOf(doorName, StringComparison.OrdinalIgnoreCase) >= 0)
                return t;
        }

        return null;
    }
}