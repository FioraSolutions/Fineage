using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2CloudSimple : MonoBehaviour
{
    void OnDisable()
    {
        if (Instance == this)
            Instance = null;
        DestroyChunks();
    }

    void LateUpdate()
    {
        if (Instance != this)
            return;

        EnsureChunks();
        if (_domeA.tf == null)
            return;

        WorldClock clock = WorldClockPacketServices.Clock ?? WorldClock.EnsurePersistent();
        float hours = clock != null ? clock.WorldHours : 12f;
        L2CloudLut.Sample cloudColors = L2CloudLut.SampleAt(hours);
        float opacity = Mathf.Clamp01(_opacity);
        bool draw = opacity > 0.01f;
        SetActive(ref _domeA, draw);
        SetActive(ref _domeB, draw);
        if (!draw)
            return;

        Camera cam = ResolveCamera();
        Vector3 pos = _followCamera && cam != null ? cam.transform.position : Vector3.zero;
        float skyZ = ResolveSkyDistance(cam);
        Place(ref _domeA, pos, skyZ, SkyCapRadius, 15f);
        float beltBottom = BeltNativeBottomY * (skyZ / SkyBeltRadius);
        float beltLift = L2HazeRing.HorizonHeight(skyZ) - beltBottom - BeltNestIntoHaze;
        Place(ref _domeB, pos, skyZ, SkyBeltRadius, beltLift);
        ApplyTint(_domeA.mats, 0, cloudColors.sheet, opacity);
        ApplyTint(_domeA.mats, 1, cloudColors.sheet, opacity);
        ApplyTint(_domeB.mats, 0, cloudColors.sheet, opacity);
        ApplyTint(_domeB.mats, 1, cloudColors.silhouette, opacity);
    }

    void StripStaleChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child == null)
                continue;
            string n = child.name;
            if (n == "L2CloudSimple_Patch" || n == "L2CloudSimple_DomeA" || n == "L2CloudSimple_DomeB" ||
                n == "L2CloudSimple_Cap" || n == "L2CloudSimple_Ring")
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }
    }
}
