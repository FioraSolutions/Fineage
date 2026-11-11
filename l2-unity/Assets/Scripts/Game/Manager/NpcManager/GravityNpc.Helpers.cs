using System.Collections.Generic;
using UnityEngine;

public partial class GravityNpc : MonoBehaviour, IGravity
{
    private void Delete(Dictionary<int, GravityData> dict , List<int> remove)
    {
        if(remove.Count > 0)
        {
            foreach (int id in remove)
            {
                if (dict.ContainsKey(id)) dict.Remove(id);
            }
        }
    }

    public void DeleteGravity(int id)
    {
        _remove.Add(id);
    }

    void OnDestroy()
    {
        if (ReferenceEquals(_instance, this))
            _instance = null;
    }
}
