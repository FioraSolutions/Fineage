using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class SurfaceDetector : MonoBehaviour 
{
    [SerializeField] private ObjectData _surfaceObject;

    [Inject] World _world;

    private World GameWorld
    {
        get { return _world != null ? _world : AudioPacketServices.GameWorld; }
    }

    private void Awake()
    {
        App.InjectGameObject(gameObject);
        WireAudioPacketServices();
    }

    private void Start()
    {
        WireAudioPacketServices();
    }

    private void WireAudioPacketServices()
    {
        AudioPacketServices.Bind(gameWorld: _world);
    }

    public string GetSurfaceTag() {
        World world = GameWorld;
        if (world == null)
            return null;

        if(Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out var hit, 2f, world.GroundMask)) {
            _surfaceObject = new ObjectData(hit.collider.gameObject);
        } else {
            return null;
        }

        return _surfaceObject.ObjectTag;
    }
}
