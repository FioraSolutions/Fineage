using UnityEngine;

public partial class DeadMonster : MonoBehaviour
{
    public Color GetOriginalColor(Renderer[] monsterRenderer)
    {
        if (monsterRenderer[0] != null)
        {
            Material material = monsterRenderer[0].material;
            if (material != null)
            {
                return material.color;
            }
        }
        return new Color();
    }

    public void OnDeadAntiGravity(int monster_id , bool useAntiGravity , World world)
    {

        _useAntiGravity = useAntiGravity;
        _world = world;
        _monster_id = monster_id;
    }
}
