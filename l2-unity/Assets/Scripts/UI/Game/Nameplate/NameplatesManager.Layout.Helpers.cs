using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public partial class NameplatesManager : MonoBehaviour
{
    private Camera ResolveCamera()
    {
        return _camera != null ? _camera : Camera.main;
    }

    private void Update()
    {
        using (FineagePerfProfiling.NameplatesUpdate.Auto())
        {
            UpdateNameplatesCore();
        }
    }

    void UpdateNameplatesCore()
    {
        if (_playerTransform == null)
        {
            if (DomainPacketServices.LocalPlayer != null && DomainPacketServices.LocalPlayer.transform != null)
            {
                _playerTransform = DomainPacketServices.LocalPlayer.transform;
            }
            else
            {
                return;
            }
        }

        if (_entryStore == null || _pixelSnap == null)
            return;

        Vector3 playerPos = _playerTransform.position;
        _overlapRangeSq = _nameplateViewDistance * _nameplateViewDistance;

        int hitCount = Physics.OverlapSphereNonAlloc(
            playerPos,
            _nameplateViewDistance,
            OverlapBuffer,
            _entityMask);

        if (hitCount >= OverlapBufferSize && !_overlapBufferOverflowLogged)
        {
            _overlapBufferOverflowLogged = true;
            FineageLog.Error(
                FineageLogCategory.World,
                $"NameplatesManager OverlapSphereNonAlloc hit cap {OverlapBufferSize}; "
                + "raise OverlapBufferSize or reduce view distance.");
        }

        _entitiesInRange = OverlapBuffer;
        _entryStore.Discover(OverlapBuffer, hitCount, _defaultNameColor);

        // Remote users (CharInfo/UserEntity) move via L2PawnWalk with their
        // CharacterController disabled, so OverlapSphere cannot find them. Upsert
        // users in view range only (distance cull still runs in RefreshVisibility).
        World world = WorldApplyPacketServices.GameWorld;
        if (world != null)
        {
            world.ForEachUser(_upsertUserInRange);
            // NPCs/monsters whose click_area is outside OverlapSphere (or missing) still need plates (#930).
            world.ForEachNpc(_upsertNpcInRange);
        }

        _entryStore.EnsureHoverAndTarget(_entityMask, _defaultNameColor);

        if (DomainPacketServices.LocalPlayer != null)
        {
            _entryStore.UpsertEntry(DomainPacketServices.LocalPlayer, _defaultNameColor);
        }

        _entryStore.RefreshVisibility(_playerTransform, _nameplateViewDistance, _pixelSnap);
    }

    void UpsertUserInRange(Entity user)
    {
        if (user == null || user.transform == null)
            return;
        if ((_playerTransform.position - user.transform.position).sqrMagnitude > _overlapRangeSq)
            return;
        _entryStore.UpsertEntry(user, _defaultNameColor);
    }

    void UpsertNpcInRange(Entity npc)
    {
        if (npc == null || npc.transform == null)
            return;
        if ((_playerTransform.position - npc.transform.position).sqrMagnitude > _overlapRangeSq)
            return;
        _entryStore.UpsertEntry(npc, _defaultNameColor);
    }

    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        Camera targetCam = ResolveCamera();
        if (cam == null || targetCam == null || cam != targetCam)
        {
            return;
        }

        EnsureResources();
        if (!_textDrawer.IsReady)
        {
            return;
        }

        _entryStore.BuildPaintList(_paintList, _drawTitles);
        if (_paintList.Count == 0)
        {
            return;
        }

        _textDrawer.Draw(cam, _paintList, _pixelSnap);
        _castBarDrawer?.Draw(cam, _paintList);
        _bubbleDrawer.Draw(cam, _paintList);
    }
}
