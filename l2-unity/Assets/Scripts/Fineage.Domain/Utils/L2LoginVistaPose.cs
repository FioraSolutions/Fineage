using System;
using UnityEngine;

/// <summary>
/// Login vista numbers taken from the Lobby.unr property dump. The file is
/// written by tools/interlude-extract/build_login_vista.py. Nothing here is a
/// tuned constant: a missing file or a missing Actions link is an error.
/// </summary>
[Serializable]
public sealed class L2LoginVistaVec
{
    public float x, y, z;
}

[Serializable]
public sealed class L2LoginVistaRot
{
    public float pitch, yaw, roll;
}

[Serializable]
public sealed class L2LoginVistaFog
{
    public int r, g, b;
    public float startUu, endUu;
}

[Serializable]
public sealed class L2LoginVistaSun
{
    public string name;
    public float pitch, yaw, roll, brightness;
    public int hue, saturation;
}

[Serializable]
public sealed class L2LoginVistaAmbient
{
    public float r, g, b, brightness;
}

[Serializable]
public sealed class L2LoginVistaTerrain
{
    public L2LoginVistaVec location;
    public L2LoginVistaVec scale;
    public int sectors;
    public float drawScaleIgnored;
}

[Serializable]
public sealed class L2LoginVistaFile
{
    public string cameraActor;
    public string cameraReason;
    public bool actionsObjectArray;
    public string zoneActor;
    public float horizontalFov;
    public string fovSource;
    public L2LoginVistaVec location;
    public L2LoginVistaRot rotationUu;
    public L2LoginVistaFog fog;
    public L2LoginVistaSun sun;
    public L2LoginVistaAmbient ambient;
    public L2LoginVistaTerrain terrain;
}

public static class L2LoginVistaPose
{
    public const string ResourcePath = "Data/Maps/l2_lobby/Meta/LoginVista";

    public static L2LoginVistaFile Parse(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("[LoginVista] LoginVista.json is empty");
            return null;
        }

        L2LoginVistaFile file = JsonUtility.FromJson<L2LoginVistaFile>(json);
        if (file == null || file.location == null || file.rotationUu == null || file.fog == null)
        {
            Debug.LogError("[LoginVista] LoginVista.json is missing camera location, rotation, or fog");
            return null;
        }

        if (file.horizontalFov <= 0f)
        {
            Debug.LogError("[LoginVista] horizontalFov " + file.horizontalFov + " is not a FOV");
            return null;
        }

        return file;
    }

    public static Vector3 CameraLocationUu(L2LoginVistaFile file)
    {
        return new Vector3(file.location.x, file.location.y, file.location.z);
    }

    public static Vector3 CameraRotationUu(L2LoginVistaFile file)
    {
        return new Vector3(file.rotationUu.pitch, file.rotationUu.yaw, file.rotationUu.roll);
    }

    public static L2Ue2ActorXform.Pose CameraPose(L2LoginVistaFile file)
    {
        return L2Ue2ActorXform.Evaluate(
            CameraLocationUu(file),
            CameraRotationUu(file),
            Vector3.zero,
            1f,
            Vector3.one,
            false,
            0f,
            Quaternion.identity,
            Vector3.one,
            file.cameraActor ?? "login-camera");
    }

    public static float VerticalFov(L2LoginVistaFile file, float aspect)
    {
        return L2Ue2ActorXform.VerticalFovDegrees(file.horizontalFov, aspect);
    }

    public static float FogStartMeters(L2LoginVistaFile file)
    {
        return file.fog.startUu * VectorUtils.L2UuToUnity;
    }

    public static float FogEndMeters(L2LoginVistaFile file)
    {
        return file.fog.endUu * VectorUtils.L2UuToUnity;
    }

    public static Color FogColor(L2LoginVistaFile file)
    {
        return new Color(file.fog.r / 255f, file.fog.g / 255f, file.fog.b / 255f, 1f);
    }
}
