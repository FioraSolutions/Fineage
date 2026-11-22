#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public class L2CameraBuilder {

    [MenuItem("Shnok/8. [Camera] (T3D) Build cameras")]
    static void ImportBrushTexturesT3D() {
        string title = "Select T3D file";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            BuildCamerasFromT3d(fileToProcess);
        }
    }

    public static GameObject BuildCamerasFromT3d(string t3dAbsolutePath) {
        List<L2InterpolationPoint> interpolationPoints = L2T3DInfoParser.ParseCameraInfo(t3dAbsolutePath);
        GameObject container = new GameObject("Cameras");

        foreach (L2InterpolationPoint point in interpolationPoints) {
            GameObject go = new GameObject(point.name);
            go.transform.position = VectorUtils.ConvertPosToUnity(point.position);
            go.transform.eulerAngles = VectorUtils.ConvertRotToUnity(point.eulerAngles);
            Camera cam = go.AddComponent<Camera>();
            cam.usePhysicalProperties = true;
            cam.fieldOfView = 60;
            cam.allowHDR = false;
            cam.allowMSAA = false;

            go.transform.parent = container.transform;
        }

        SortChildrenByNameTag(container);
        return container;
    }

    /// <summary>
    /// Builds cameras from UE2 <c>Class=Camera</c> actors (Lobby.unr viewports).
    /// UE2 FovAngle is horizontal; the baked vertical value uses the 4:3 reference
    /// aspect and LoginLobbyPresentation re-derives it for the live aspect at
    /// runtime. Actors without a stored FovAngle inherit the PlayerController.uc
    /// class default of 50 (official UE2 semantics, logged per camera).
    /// </summary>
    public static GameObject BuildLobbyCamerasFromT3d(string t3dAbsolutePath) {
        List<L2T3DCameraInfo> cameraInfos = L2T3DInfoParser.ParseLobbyCameras(t3dAbsolutePath);
        GameObject container = new GameObject("LobbyCameras");

        foreach (L2T3DCameraInfo info in cameraInfos) {
            float hfov = info.fovAngleDeg;
            if (float.IsNaN(hfov)) {
                hfov = L2Ue2Fov.OfficialHfovDeg;
                Debug.Log($"[L2CameraBuilder] {info.name}: no FovAngle, inheriting PlayerController default {hfov}");
            }

            string goName = string.IsNullOrEmpty(info.tag) ? info.name : $"{info.name} ({info.tag})";
            GameObject go = new GameObject(goName);
            go.transform.position = VectorUtils.ConvertPosToUnity(info.position);
            go.transform.eulerAngles = VectorUtils.ConvertRotToUnity(info.eulerAngles);
            Camera cam = go.AddComponent<Camera>();
            cam.usePhysicalProperties = false;
            cam.fieldOfView = L2Ue2Fov.VerticalForHorizontal(hfov, 4f / 3f);
            cam.allowHDR = false;
            cam.allowMSAA = false;

            go.transform.parent = container.transform;
        }

        SortChildrenByNameTag(container);
        return container;
    }

    public static void SortChildrenByNameTag(GameObject obj) {
        List<Transform> children = new List<Transform>();
        for (int i = obj.transform.childCount - 1; i >= 0; i--) {
            Transform child = obj.transform.GetChild(i);

            children.Add(child);
            child.transform.SetParent(null, false);
        }

        children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });

        foreach (Transform child in children) {
            child.transform.SetParent(obj.transform, false);
        }
    }

}
#endif