using System;
using UnityEngine;

public static partial class L2MaterialPropertyCopier
{
    public static void SampleMeshSpawnLocVelSizeZ(
        Material mat,
        uint stateBeforeVelocity,
        out float locationZ,
        out float velocityZ,
        out float sizeZ)
    {
        uint state = stateBeforeVelocity;
        Vector4 velRange = mat != null && mat.HasProperty(StartVelocityZRangeUUId)
            ? mat.GetVector(StartVelocityZRangeUUId)
            : new Vector4(-3.6f, 3.6f, 0f, 0f);
        Vector4 locRange = mat != null && mat.HasProperty(StartLocationZRangeUUId)
            ? mat.GetVector(StartLocationZRangeUUId)
            : new Vector4(-3.6f, 3.6f, 0f, 0f);
        Vector4 sizeZRange = mat != null && mat.HasProperty(StartSizeZRangeId)
            ? mat.GetVector(StartSizeZRangeId)
            : new Vector4(-0.0408f, 0.0408f, 0f, 0f);
        float sizeXY = mat != null && mat.HasProperty(StartSizeXYId)
            ? mat.GetFloat(StartSizeXYId)
            : 0.132f;
        Vector4 colorMul = mat != null && mat.HasProperty(ColorMultiplierId)
            ? mat.GetVector(ColorMultiplierId)
            : new Vector4(1f, 0.774f, 0.6f, 0f);
        Vector4 lifetimeRange = mat != null && mat.HasProperty(LifetimeRangeId)
            ? mat.GetVector(LifetimeRangeId)
            : new Vector4(1f, 1f, 0f, 0f);
        Vector4 delayRange = mat != null && mat.HasProperty(InitialDelayRangeId)
            ? mat.GetVector(InitialDelayRangeId)
            : Vector4.zero;

        velocityZ = AppRandFRangeVectorZ(0f, 0f, 0f, 0f, velRange.x, velRange.y, ref state);
        locationZ = AppRandFRangeVectorZ(0f, 0f, 0f, 0f, locRange.x, locRange.y, ref state);

        AppRandFRange(0f, 1f, ref state);
        for (int i = 0; i < 6; i++)
        {
            AppRandFrand(ref state);
        }

        AppRandFRangeVectorZ(
            colorMul.x, colorMul.x,
            colorMul.y, colorMul.y,
            colorMul.z, colorMul.z,
            ref state);
        AppRandFRange(lifetimeRange.x, lifetimeRange.y, ref state);
        AppRandFRange(delayRange.x, delayRange.y, ref state);
        AppRandFRange(1f, 1f, ref state);

        sizeZ = AppRandFRangeVectorZ(
            sizeXY, sizeXY,
            sizeXY, sizeXY,
            sizeZRange.x, sizeZRange.y,
            ref state);
    }

    public static void SampleMeshSpawnLocVelSize(
        Material mat,
        uint stateBeforeVelocity,
        out Vector3 velocityUe,
        out Vector3 locationUe,
        out Vector3 colorMulRgb,
        out float lifetimeSeconds,
        out float initialDelaySeconds,
        out Vector3 sizeUe)
    {
        uint state = stateBeforeVelocity;
        Vector2 velX = ReadMinMax(mat, StartVelocityRangeXUcId, 10f, 10f);
        Vector2 velY = ReadMinMax(mat, StartVelocityRangeYUcId, -20f, 20f);
        Vector2 velZ = ReadMinMax(mat, StartVelocityRangeZUcId, -20f, 20f);
        Vector2 locX = ReadMinMax(mat, StartLocationRangeXUcId, 0f, 0f);
        Vector2 locY = ReadMinMax(mat, StartLocationRangeYUcId, 0f, 0f);
        Vector2 locZ = ReadMinMax(mat, StartLocationRangeZUcId, 0f, 0f);
        Vector4 colorMul = mat != null && mat.HasProperty(ColorMultiplierId)
            ? mat.GetVector(ColorMultiplierId)
            : new Vector4(0.6f, 0.6f, 0.6f, 0f);
        Vector2 life = ReadMinMax(mat, LifetimeRangeId, 1f, 1.5f);
        Vector2 delay = ReadMinMax(mat, InitialDelayRangeId, 0f, 0f);
        Vector2 sizeX = ReadMinMax(mat, StartSizeRangeXUcId, 0.015f, 0.015f);
        Vector2 sizeY = ReadMinMax(mat, StartSizeRangeYUcId, 0.1f, 0.1f);
        Vector2 sizeZ = ReadMinMax(mat, StartSizeRangeZUcId, 0.1f, 0.1f);

        velocityUe = AppRandFRangeVector(velX, velY, velZ, ref state);
        locationUe = AppRandFRangeVector(locX, locY, locZ, ref state);

        AppRandFRange(0f, 1f, ref state);
        for (int i = 0; i < 6; i++)
        {
            AppRandFrand(ref state);
        }

        colorMulRgb = AppRandFRangeVector(
            new Vector2(colorMul.x, colorMul.x),
            new Vector2(colorMul.y, colorMul.y),
            new Vector2(colorMul.z, colorMul.z),
            ref state);
        lifetimeSeconds = AppRandFRange(life.x, life.y, ref state);
        initialDelaySeconds = AppRandFRange(delay.x, delay.y, ref state);
        AppRandFRange(1f, 1f, ref state);
        sizeUe = AppRandFRangeVector(sizeX, sizeY, sizeZ, ref state);
    }

    private static Vector2 ReadMinMax(Material mat, int propertyId, float defaultMin, float defaultMax)
    {
        if (mat != null && mat.HasProperty(propertyId))
        {
            Vector4 v = mat.GetVector(propertyId);
            return new Vector2(v.x, v.y);
        }

        return new Vector2(defaultMin, defaultMax);
    }

    private static uint AppRandStep(ref uint state)
    {
        state = unchecked(state * AppRandMultiplier + AppRandIncrement);
        return (state >> 16) & 0x7fffu;
    }
}
