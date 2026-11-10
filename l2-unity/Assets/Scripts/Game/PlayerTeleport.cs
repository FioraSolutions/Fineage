using System.Collections;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    private Vector3 _teleportPosition;
    private bool _isTeleporting;
    Coroutine _resnap;

    void Update()
    {
        if (!_isTeleporting)
            return;

        _isTeleporting = false;
        ApplyTeleportPose(_teleportPosition);
        if (_resnap != null)
            StopCoroutine(_resnap);
        _resnap = StartCoroutine(ResnapAfterTerrainLoads());
    }

    public void TeleportTo(Vector3 teleportPosition)
    {
        _teleportPosition = WorldPlacement.Resolve(
            WorldPlacement.PlacementKind.PlayerTeleport,
            teleportPosition);
        _isTeleporting = true;
        // Death/rebirth leave pitch/roll on the root; village restart must stand upright.
        Vector3 eulers = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, eulers.y, 0f);
    }

    /// <summary>World position after ground snap (set when <see cref="TeleportTo"/> runs).</summary>
    public Vector3 LastTeleportPosition => _teleportPosition;

    void ApplyTeleportPose(Vector3 position)
    {
        Vector3 eulers = transform.eulerAngles;
        Quaternion upright = Quaternion.Euler(0f, eulers.y, 0f);
        WorldPlacement.ApplyPose(gameObject, position, upright);
    }

    /// <summary>
    /// Village/gate teleports often land before the destination tile colliders are active; retry snap so
    /// the pawn does not stay on the raw server Y in the air.
    /// </summary>
    IEnumerator ResnapAfterTerrainLoads()
    {
        float[] at = { 0.15f, 0.5f, 1.25f };
        float elapsed = 0f;
        for (int i = 0; i < at.Length; i++)
        {
            yield return new WaitForSeconds(at[i] - elapsed);
            elapsed = at[i];
            if (this == null)
                yield break;

            Vector3 from = transform.position;
            Vector3 snapped = WorldPlacement.Resolve(WorldPlacement.PlacementKind.PlayerTeleport, from);
            if ((snapped - from).sqrMagnitude < 0.0001f)
                yield break;

            ApplyTeleportPose(snapped);
            _teleportPosition = snapped;
            yield break;
        }
    }
}
