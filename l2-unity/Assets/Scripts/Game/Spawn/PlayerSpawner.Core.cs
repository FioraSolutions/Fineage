using UnityEngine;

public sealed partial class PlayerSpawner
{
    public PlayerSpawner(
        CameraController camera,
        CharacterInfoWindow characterInfo,
        CharacterBuilder characterBuilder)
    {
        _camera = camera;
        _characterInfo = characterInfo;
        _characterBuilder = characterBuilder;
    }
}
