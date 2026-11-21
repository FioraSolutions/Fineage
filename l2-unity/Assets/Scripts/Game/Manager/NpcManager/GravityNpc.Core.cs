using System.Collections.Generic;
using UnityEngine;

public partial class GravityNpc : MonoBehaviour, IGravity
{
    private void Awake()
    {
        if ((_instance as UnityEngine.Object) == null)
        {
            _instance = this;
            GravityNpcPacketServices.Bind(gravity: this);
            _dict = new Dictionary<int, GravityData>();
            _remove = new List<int>();
        }
        else
        {
            Destroy(this);
        }
    }

    public void StepGravity()
    {
        using (FineagePerfProfiling.GravityStep.Auto())
            StepGravityCore();
    }

    void StepGravityCore()
    {
        if (_dict == null || _dict.Count == 0) return;
        foreach (KeyValuePair<int, GravityData> kvp in _dict)
        {
            GravityData data = kvp.Value;
            CharacterController controller = data.GetControllerToTypeEntity();

            if (controller == null || !controller.enabled || !controller.gameObject.activeInHierarchy)
            {
                // Pooled/despawned entity left in dict — drop next Delete pass.
                _remove.Add(kvp.Key);
                continue;
            }

            if (data.IsDead())
                continue;

            // Spawn gate: wait until WorldPlacement + colliders are live.
            if (data.ArmFrames > 0)
            {
                data.ArmFrames--;
                continue;
            }

            // Same pipeline as spawn and move: server Y is the authority.
            // Do not CharacterController.Move downward — that lands on water
            // and distant shells the walkable mask would have rejected.
            Transform body = controller.transform;
            Vector3 feet = body.position;
            Entity entity = data.Entity;
            float refY = feet.y;
            if (entity != null && entity.Identity != null)
            {
                entity.Identity.RememberServerPosition();
                refY = entity.Identity.ServerPosition.y;
            }

            Vector3 reference = new Vector3(feet.x, refY, feet.z);
            Vector3 stood = WorldPlacement.KeepOnBrush(reference);
            if ((stood - feet).sqrMagnitude < 0.0001f)
                continue;

            WorldPlacement.ApplyPose(body.gameObject, stood, body.rotation);
            if (entity != null && entity.Identity != null)
                entity.Identity.Position = stood;
        }

        Delete(_dict, _remove);
        _remove.Clear();
    }

    public void AddGravity(int id, GravityData data)
    {
        if (!_dict.ContainsKey(id))
        {
            _dict.Add(id, data);
        }
    }
}
