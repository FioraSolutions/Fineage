using System;

public partial class MoveAllCharacters : MonoBehaviour, IMoveAllCharacters
{
    void Update()
    {
        CrowdAnimTest.EnsureTimeCap();
        if (_moveDict == null)
            return;

        StepMoves();

        if (_rotateDict == null || _rotateDict.Count == 0) return;

        foreach (KeyValuePair<int, RotateData> kvp in _rotateDict)
        {
            RotateData data = kvp.Value;
            bool isRotate = data.IsRotate();

            if (isRotate)
            {
                if (data.IsMonsterDead())
                {
                    _removeListRotate.Add(kvp.Key);
                }
                else if (data.IsEntityTarget() & data.IsEntityMonster())
                {
                    Transform _targetRotation = data.GetTargetTransform();
                    Transform transform = data.GetMonsterTransform();
                    float angleThreshold = data.GetAngleThreshold();

                    if (!VectorUtils.IsFacingAttacker(transform, _targetRotation.position, angleThreshold))
                    {

                        TurnTowardsAttacker(_targetRotation.position, transform);
                    }
                    else
                    {
                        data.SetRotate(false);
                        _removeListRotate.Add(kvp.Key);
                    }
                }
                else
                {
                    _removeListRotate.Add(kvp.Key);
                }

            }
        }

        RemoveFinishKeyRotate();
    }
}
