using UnityEngine;

public partial class ProjectileData
{
    public ProjectileData()
    {

        id = 0;
        prefab = null;
        transform = null;
        startPosition = Vector3.zero;
        targetTransform = null;
        speed = 8f;
        lifetime = 5f;
        flytime = 5f;
        //speedCurve = AnimationCurve.Linear(0, 1, 1, 1);
        speedCurve = CreateDefaultSpeedCurve();
        startTime = 0f;
        distance = 0f;
        useGravity = false;
        gravityScale = 1f;
        isActive = true;
        impactType = ProjectileImpactType.ArrowStick;
        attackerEntityId = 0;
    }

    public ProjectileData(GameObject prefabGo , Transform transform1 , Vector3 startPos , Transform endPos)
    {
        id = 0;
        prefab = prefabGo;
        transform = transform1; 
        startPosition = startPos;
        targetTransform = endPos;
        speed = 8f;
        lifetime = 5f;
        flytime = 5f;
        //speedCurve = AnimationCurve.Linear(0, 1, 1, 1);
        speedCurve = CreateDefaultSpeedCurve();
        startTime = 0f;
        distance = 0f;
        useGravity = false;
        gravityScale = 1f;
        isActive = true;
        impactType = ProjectileImpactType.ArrowStick;
        attackerEntityId = 0;
    }
}
