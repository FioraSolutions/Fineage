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
        StepGravityWithBatchRays();
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
