using UnityEngine;

public partial class MagicCastData
{
    private static float ClipPlaySeconds(
        int index,
        int filler,
        int clipCount,
        float native,
        float fillerInitial,
        float headSlack,
        float scale,
        bool rescale,
        float pad,
        float room)
    {
        if (index >= clipCount || native <= 0f)
            return 0f;

        float play;
        if (index == filler)
        {
            if (rescale)
                play = 0f;
            else if (index == 0)
                play = fillerInitial + headSlack;
            else
                play = fillerInitial;
        }
        else if (index == 0 && clipCount > 1)
        {
            float baseSeconds = native / scale + headSlack;
            bool rewriteFirstClip = pad >= ClientFirstClipFactorSeconds;
            play = rewriteFirstClip
                ? baseSeconds * (1f - ClientFirstClipFactorSeconds / room)
                : baseSeconds;
        }
        else
        {
            play = native / scale;
        }

        return play > 0f ? play : 0f;
    }

    /// <summary>Unity clip length / wall play seconds. Filler may be &lt; ×1 (InitSkillProcess stretch).</summary>
    private static float SpeedFor(float unityLength, float playSeconds)
    {
        if (unityLength <= 0f)
            return 1f;
        if (playSeconds <= 0.0001f)
            return unityLength / 0.0001f;
        return unityLength / playSeconds;
    }

    /// <summary>skillgrp cast_style, stored by the client at pawn+52C.</summary>
    private static float PadSeconds(int castStyle)
    {
        switch (castStyle)
        {
            case 2:
            case 5:
            case 8:
            case 10:
                return 0.4f;
            case 14:
                return 2f;
            case 3:
            case 12:
                return 0f;
            default:
                return 0.15f;
        }
    }

    private static float ClientNativeSeconds(float unityLength)
    {
        if (unityLength <= 0f)
            return 0f;
        return (unityLength * UnityClipFramesPerSecond + 1f) / ClientFramesPerSecond;
    }

    private static float ClientNotifySeconds(float unityEventTime)
    {
        if (unityEventTime <= 0f)
            return 0f;
        return unityEventTime * UnityClipFramesPerSecond / ClientFramesPerSecond;
    }

    private static float GetDurationSafe(float[] clipsDurations, int index)
    {
        if (clipsDurations == null || index < 0 || index >= clipsDurations.Length)
            return 0f;
        return Mathf.Max(0f, clipsDurations[index]);
    }
