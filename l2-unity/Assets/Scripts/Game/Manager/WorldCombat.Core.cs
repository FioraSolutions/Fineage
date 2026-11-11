using System;

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static StatusUpdate;
using static UnityEngine.EventSystems.EventTrigger;

public partial class WorldCombat : MonoBehaviour
{
    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    void OnDestroy() {
        _instance = null;
    }

    public void InflictAttack(Transform target, int damage, bool criticalHit) {
        ApplyDamage(target, damage, criticalHit);
    }

    public void InflictAttack(Vector3 hitPoint, Vector3 impactDirection) {
        //ApplyDamage(target, damage, criticalHit);

        // Instantiate hit particle
        ParticleImpact(hitPoint, impactDirection);
    }
}
