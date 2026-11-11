using System;
using Object = UnityEngine.Object;

public partial class CharacterCreator : MonoBehaviour
{
    void Awake()
    {
        if (_instance == null) {
            _instance = this;
        } else if(_instance != this) {
            Destroy(gameObject);
        }

        WireCharacterPacketServices();
    }

    private void Start()
    {
        WireCharacterPacketServices();
    }

    private void WireCharacterPacketServices()
    {
        CharacterPacketServices.Bind(animations: _animations, builder: _characterBuilder);
        TablePacketServices.Bind(logonGrps: _logonGrps);
    }
}
