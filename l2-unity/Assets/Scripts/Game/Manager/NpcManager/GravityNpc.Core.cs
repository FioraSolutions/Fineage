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
            _direction = Vector3.zero;
        }
        else
        {
            Destroy(this);
        }
    }

    private void FixedUpdate()
    {
        if (_dict == null || _dict.Count == 0) return;
        //Debug.Log("GravityData size " + _dict.Count);
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

            if (!data.IsDead())
            {
                // Spawn gate: wait until WorldPlacement + colliders are live before yanking down.
                if (data.ArmFrames > 0)
                {
                    data.ArmFrames--;
                    continue;
                }

                Vector3 ajustedDirection = _direction * _speed * _moveSpeedMultiplier + Vector3.down * _gravity;
                controller.Move(ajustedDirection * Time.deltaTime);
            }
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
