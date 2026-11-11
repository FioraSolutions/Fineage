using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public sealed partial class CombatFacingService : MonoBehaviour
{
    [Obsolete("Use CombatPacketServices.EnsureFacing or [Inject] CombatFacingService instead")]
    public static CombatFacingService Ensure()
    {
        if (_instance != null)
        {
            return _instance;
        }

        if (App.GameContainer != null)
        {
            try
            {
                CombatFacingService resolved = App.GameContainer.Resolve<CombatFacingService>();
                if (resolved != null)
                    return resolved;
            }
            catch
            {
            }
        }

        GameObject go = new GameObject(nameof(CombatFacingService));
        CombatFacingService created = go.AddComponent<CombatFacingService>();
        App.InjectGameObject(go);
        return created;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        CombatPacketServices.Bind(facing: this);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public bool HasFollow(int objectId) => _entries.ContainsKey(objectId);
}
