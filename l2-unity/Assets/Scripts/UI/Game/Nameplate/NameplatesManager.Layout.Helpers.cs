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

        _entitiesInRange = Physics.OverlapSphere(
            _playerTransform.position,
            _nameplateViewDistance,
            _entityMask);

        _entryStore.Discover(_entitiesInRange, _defaultNameColor);

        // Remote users (CharInfo/UserEntity) move via L2PawnWalk with their
        // CharacterController disabled, so OverlapSphere cannot find them. Upsert
        // every registered user directly from World so their nameplates stay visible
        // even when not hovered. Distance cull still happens in RefreshVisibility.
        World world = WorldApplyPacketServices.GameWorld;
        if (world != null)
        {
            world.ForEachUser(user => _entryStore.UpsertEntry(user, _defaultNameColor));
            // NPCs/monsters whose click_area is outside OverlapSphere (or missing) still need plates (#930).
            world.ForEachNpc(npc => _entryStore.UpsertEntry(npc, _defaultNameColor));
        }

        _entryStore.EnsureHoverAndTarget(_entityMask, _defaultNameColor);

        if (DomainPacketServices.LocalPlayer != null)
        {
            _entryStore.UpsertEntry(DomainPacketServices.LocalPlayer, _defaultNameColor);
        }

        _entryStore.RefreshVisibility(_playerTransform, _nameplateViewDistance, _pixelSnap);
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
