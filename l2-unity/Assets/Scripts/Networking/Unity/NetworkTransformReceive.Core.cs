using System;
using System.Collections;
using UnityEngine;
using VContainer;

public partial class NetworkTransformReceive : MonoBehaviour
{
    void Start() {
        NetworkingPacketServices.Bind(world: _world);
        World world = GameWorld;
        if(world != null && world.OfflineMode) {
            this.enabled = false;
            return;
        }

        _lastPos = transform.position;
        _newRotation = transform.eulerAngles.y;
        _serverPosition = transform.position;
        _positionSyncPaused = false;
    }

    void FixedUpdate() {
        if(_positionSyncProtection && !_positionSyncPaused) {
            //Debug.Log("SYNC POSITIOBBBBBBBBBBBBB !");
            UpdatePosition();
        }
        UpdateRotation();
    }

    public void SetNewPosition(Vector3 pos) {
        /* adjust y to ground height */
        World world = GameWorld;
        if (world != null)
            pos.y = world.GetGroundHeight(pos);

        _serverPosition = pos;

        /* reset states */
        _lastPos = transform.position;
        _posLerpValue = 0;
    }

    public void UpdatePosition() {
        /* Check if client transform position is synced with server's */
        _positionDelta = VectorUtils.Distance2D(transform.position, _serverPosition);
        Geodata geodata = GeodataPacketServices.Geodata;
        if (geodata == null)
            return;
        if(_positionDelta > geodata.NodeSize * _positionSyncNodesThreshold && _positionSynced) {
            _lastDesyncTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            _positionSynced = false;
           // Debug.Log("position 0");
        }

        if(!_positionSynced) {
            //Debug.Log("position 1");
            _lastDesyncDuration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastDesyncTime;
            if(_lastDesyncDuration > _maximumAllowedDesyncTimeMs) {
                if(_positionDelta <= geodata.NodeSize / 10f) {
                    //true - use Gravity sync
                    _positionSynced = true;
                   // Debug.Log("position 2");
                    return;
                }

                transform.position = Vector3.Lerp(_lastPos, _serverPosition, _posLerpValue);
                _posLerpValue += (1 / _lerpDuration) * Time.deltaTime;
               // Debug.Log("position 3");
            }

        }   
    }

    private void UpdateRotation() {
        ///transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.up * _newRotation), Time.deltaTime * 7.5f);
    }
}
