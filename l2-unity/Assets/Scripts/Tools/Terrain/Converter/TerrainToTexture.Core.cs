#if (UNITY_EDITOR) 
using System.IO;
using UnityEngine;

public partial class TerrainToTexture : MonoBehaviour
{
    void Start() {
        TerrainData terrainData = terrain.terrainData;
        Vector3 size = terrainData.size;


        // Camera previewCam = CreateCamera(width, length);

        GameObject cameraObject = new GameObject("Camera");
        cameraObject.AddComponent<Camera>();

        for(int x = 0; x < subdivisions; x++) {
            for(int y = 0; y < subdivisions; y++) {
                float width = size.x / subdivisions;
                float length = size.z / subdivisions;


                Camera camera = MoveCamera(cameraObject, x, width, y, length);

                int resHeight = textureSize;
                int resWidth = Mathf.RoundToInt(resHeight * camera.aspect);
                RenderTexture renderTexture = new RenderTexture(resWidth, resHeight, 24);
                camera.targetTexture = renderTexture;

                camera.Render();

                //Texture2D texture = new Texture2D(resWidth, resHeight, TextureFormat.DXT5Crunched, false);
                Texture2D texture = new Texture2D(resWidth, resHeight);
                RenderTexture.active = renderTexture;
                texture.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
                texture.Apply();

                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes("Assets/Data/Maps/17_25/TerrainData/Textures/17_25_" + x + "_" + y + ".png", bytes);
            }
        }



        /*Camera mainCamera = Camera.main;
        if(mainCamera != null && mainCamera.gameObject != previewCam.gameObject) {
            mainCamera.gameObject.SetActive(false);
            mainCamera.enabled = false;
        }

        previewCam.tag = "MainCamera";*/

        //cameraObject.AddComponent<UniversalAdditionalCameraData>();


    }
}
#endif