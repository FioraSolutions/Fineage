using System.Collections.Generic;
using UnityEngine;
using static ObjectPoolManager;

public abstract partial class AbstractPoolManager : MonoBehaviour
{
    protected Dictionary<ObjectType, Dictionary<GameObject, Queue<GameObject>>> poolDictionary;
    protected Dictionary<ObjectType, GameObject> tagToPrefabMap;
    protected Dictionary<GameObject, int> createdInstancesTracker;
    protected Dictionary<ObjectType, int> objectTypePoolLimits = new Dictionary<ObjectType, int>();








}
