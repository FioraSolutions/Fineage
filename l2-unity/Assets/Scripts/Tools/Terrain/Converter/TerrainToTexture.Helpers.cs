#if (UNITY_EDITOR) 
using System.IO;
using UnityEngine;

public partial class TerrainToTexture : MonoBehaviour
{
    Camera MoveCamera(GameObject cameraObject, int x, float width, int y, float length) {

        cameraObject.transform.position = new Vector3(width / 2 + x * width, height, length / 2 + length * y);
        cameraObject.transform.rotation = Quaternion.Euler(90, 0, 0);

        Camera camera = cameraObject.GetComponent<Camera>();
        camera.orthographic = true;
        camera.aspect = width / length;
        camera.orthographicSize = length / 2f;

        return camera;
    }

    void Update() {

    }
}
#endif