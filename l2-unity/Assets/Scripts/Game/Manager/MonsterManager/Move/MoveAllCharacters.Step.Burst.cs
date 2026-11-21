using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public partial class MoveAllCharacters
{
  readonly List<int> _burstMoveKeys = new List<int>(64);

  NativeArray<float3> _planPositions;
  NativeArray<float3> _planTargets;
  NativeArray<float> _planStopDistances;
  NativeArray<float> _planSpeeds;
  NativeArray<RemoteMovePlanResult> _planResults;
  int _planCapacity;

  NativeArray<float3> _rotateMonsterPos;
  NativeArray<float3> _rotateTargetPos;
  NativeArray<quaternion> _rotateCurrent;
  NativeArray<float> _rotateThresholds;
  NativeArray<PawnRotateStepResult> _rotateResults;
  readonly List<int> _rotateBurstKeys = new List<int>(16);
  int _rotateCapacity;

  void EnsurePlanCapacity(int count)
  {
    if (_planCapacity >= count)
      return;
    DisposePlanBuffers();
    _planCapacity = math.max(64, count);
    var persistent = Allocator.Persistent;
    _planPositions = new NativeArray<float3>(_planCapacity, persistent);
    _planTargets = new NativeArray<float3>(_planCapacity, persistent);
    _planStopDistances = new NativeArray<float>(_planCapacity, persistent);
    _planSpeeds = new NativeArray<float>(_planCapacity, persistent);
    _planResults = new NativeArray<RemoteMovePlanResult>(_planCapacity, persistent);
  }

  void DisposePlanBuffers()
  {
    if (_planCapacity == 0)
      return;
    if (_planPositions.IsCreated) _planPositions.Dispose();
    if (_planTargets.IsCreated) _planTargets.Dispose();
    if (_planStopDistances.IsCreated) _planStopDistances.Dispose();
    if (_planSpeeds.IsCreated) _planSpeeds.Dispose();
    if (_planResults.IsCreated) _planResults.Dispose();
    _planCapacity = 0;
  }

  void EnsureRotateCapacity(int count)
  {
    if (_rotateCapacity >= count)
      return;
    DisposeRotateBuffers();
    _rotateCapacity = math.max(16, count);
    var persistent = Allocator.Persistent;
    _rotateMonsterPos = new NativeArray<float3>(_rotateCapacity, persistent);
    _rotateTargetPos = new NativeArray<float3>(_rotateCapacity, persistent);
    _rotateCurrent = new NativeArray<quaternion>(_rotateCapacity, persistent);
    _rotateThresholds = new NativeArray<float>(_rotateCapacity, persistent);
    _rotateResults = new NativeArray<PawnRotateStepResult>(_rotateCapacity, persistent);
  }

  void DisposeRotateBuffers()
  {
    if (_rotateCapacity == 0)
      return;
    if (_rotateMonsterPos.IsCreated) _rotateMonsterPos.Dispose();
    if (_rotateTargetPos.IsCreated) _rotateTargetPos.Dispose();
    if (_rotateCurrent.IsCreated) _rotateCurrent.Dispose();
    if (_rotateThresholds.IsCreated) _rotateThresholds.Dispose();
    if (_rotateResults.IsCreated) _rotateResults.Dispose();
    _rotateCapacity = 0;
  }

  void OnDestroyBurstBuffers()
  {
    DisposePlanBuffers();
    DisposeRotateBuffers();
  }

  bool TryQueueBurstRemoteMove(int key, Vector3 target, MovementData data)
  {
    if (!data.IsEntity())
      return false;

    Transform transform = data.GetTransform();
    if (transform == null)
      return false;

    if (data.IsOwnerDead() || !data.IsMove())
      return false;

    if (L2PawnWalkEntity.IsCharInfoPawn(data.GetEntity()))
      return false;

    Entity entity = data.GetEntity();
    if (entity is MonsterEntity || entity is NpcEntity)
      return false;

    _burstMoveKeys.Add(key);
    int slot = _burstMoveKeys.Count - 1;

    Vector3 before = transform.position;
    _planPositions[slot] = new float3(before.x, before.y, before.z);
    _planTargets[slot] = new float3(target.x, target.y, target.z);
    _planStopDistances[slot] = data.GetDistance();
    data.AdvanceMoveTimer();
    data.SyncUserGait();
    _planSpeeds[slot] = data.GetSpeed();
    return true;
  }

  void CompleteBurstRemoteMoves()
  {
    int count = _burstMoveKeys.Count;
    if (count == 0)
      return;

    using (FineagePerfProfiling.MoveBurstPlan.Auto())
    {
      var job = new RemoteMovePlanBurstJob
      {
          Positions = _planPositions,
          Targets = _planTargets,
          StopDistances = _planStopDistances,
          Speeds = _planSpeeds,
          DeltaTime = Time.deltaTime,
          Results = _planResults,
      };
      job.Schedule(count, 16).Complete();
    }

    for (int i = 0; i < count; i++)
    {
      int key = _burstMoveKeys[i];
      if (!_moveDict.TryGetValue(key, out MovementData data) || data == null)
        continue;

      RemoteMovePlanResult plan = _planResults[i];
      Transform transform = data.GetTransform();
      if (transform == null)
      {
        _removeListMove.Add(key);
        continue;
      }

      if (plan.Action == RemoteMovePlanResult.ActionFinished)
      {
        if (_moveDict.TryGetValue(key, out MovementData still) && still.GetMovementTarget() != null &&
            still.GetMovementTarget().TryGetTarget(out Vector3 dest))
          still.OnFinish(dest);
        _removeListMove.Add(key);
        data.SetLastPosition(transform.position);
        continue;
      }

      if (plan.Action == RemoteMovePlanResult.ActionNoOp)
      {
        data.SetLastPosition(transform.position);
        continue;
      }

      if (plan.Action == RemoteMovePlanResult.ActionMove)
      {
        var direction = new Vector3(plan.DirX, 0f, plan.DirZ);
        data.Move(direction, plan.EffectiveSpeed);
        data.SetLastPosition(transform.position);
      }
    }

    _burstMoveKeys.Clear();
  }

  void StepRotatesBurst()
  {
    if (_rotateDict == null || _rotateDict.Count == 0)
      return;

    _rotateBurstKeys.Clear();
    foreach (KeyValuePair<int, RotateData> kvp in _rotateDict)
    {
      RotateData data = kvp.Value;
      if (!data.IsRotate())
        continue;
      if (data.IsMonsterDead())
      {
        _removeListRotate.Add(kvp.Key);
        continue;
      }

      if (!(data.IsEntityTarget() & data.IsEntityMonster()))
      {
        _removeListRotate.Add(kvp.Key);
        continue;
      }

      Transform monster = data.GetMonsterTransform();
      Transform target = data.GetTargetTransform();
      if (monster == null || target == null)
      {
        _removeListRotate.Add(kvp.Key);
        continue;
      }

      _rotateBurstKeys.Add(kvp.Key);
    }

    int count = _rotateBurstKeys.Count;
    if (count == 0)
      return;

    EnsureRotateCapacity(count);
    for (int i = 0; i < count; i++)
    {
      int key = _rotateBurstKeys[i];
      RotateData data = _rotateDict[key];
      Transform monster = data.GetMonsterTransform();
      Transform target = data.GetTargetTransform();
      _rotateMonsterPos[i] = new float3(monster.position.x, monster.position.y, monster.position.z);
      Vector3 tp = target.position;
      _rotateTargetPos[i] = new float3(tp.x, tp.y, tp.z);
      _rotateCurrent[i] = monster.rotation;
      _rotateThresholds[i] = data.GetAngleThreshold();
    }

    using (FineagePerfProfiling.MoveBurstRotate.Auto())
    {
      var job = new PawnRotateStepBurstJob
      {
          MonsterPositions = _rotateMonsterPos,
          TargetPositions = _rotateTargetPos,
          CurrentRotations = _rotateCurrent,
          AngleThresholds = _rotateThresholds,
          SlerpFactor = Time.deltaTime * 15f,
          Results = _rotateResults,
      };
      job.Schedule(count, 8).Complete();
    }

    for (int i = 0; i < count; i++)
    {
      int key = _rotateBurstKeys[i];
      if (!_rotateDict.TryGetValue(key, out RotateData data))
        continue;

      PawnRotateStepResult result = _rotateResults[i];
      Transform monster = data.GetMonsterTransform();
      if (monster == null)
      {
        _removeListRotate.Add(key);
        continue;
      }

      if (!result.HasStep)
        continue;

      if (result.Facing)
      {
        data.SetRotate(false);
        _removeListRotate.Add(key);
        continue;
      }

      monster.rotation = result.Rotation;
    }
  }
}