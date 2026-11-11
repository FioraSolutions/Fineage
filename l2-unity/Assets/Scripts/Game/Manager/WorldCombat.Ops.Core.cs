using System;

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static StatusUpdate;
using static UnityEngine.EventSystems.EventTrigger;

public partial class WorldCombat : MonoBehaviour
{
    private void ApplyDamage(Transform target, int damage, bool criticalHit) {
        Entity entity = target.GetComponent<Entity>();
        if (entity != null) {
            // Apply damage to target
            entity.ApplyDamage(damage, criticalHit);
        }
    }

    private void ParticleImpact(Vector3 hitPoint, Vector3 impactDirection)
    {
        // Use the actual impact point instead of calculating a new position
        GameObject go = (GameObject)Instantiate(
            _impactParticle,
            hitPoint,  // Use the exact impact point from collision detection
            Quaternion.identity);

        // Use the provided impact direction to orient the particle
        go.transform.rotation = Quaternion.LookRotation(impactDirection);
    }
}
