using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Loads <c>interlude_packet_presentations.json</c> and resolves Mobius packet ids to shared presentation pipelines.
/// </summary>
public static class InterludePacketPresentationCatalog
{
    const string RelativePath = "Data/Meta/interlude_packet_presentations.json";

    static bool _loaded;
    static readonly Dictionary<int, InterludePresentationKind> MagicBySkillId = new();
    static readonly Dictionary<int, InterludePresentationKind> SocialByActionId = new();

    public static void EnsureLoaded()
    {
        if (_loaded)
            return;
        _loaded = true;
        MagicBySkillId.Clear();
        SocialByActionId.Clear();

        string path = Path.Combine(Application.streamingAssetsPath, RelativePath);
        if (!File.Exists(path))
        {
            Debug.LogError(
                "[PacketPresentation] missing catalog StreamingAssets/" + RelativePath);
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            var root = JsonUtility.FromJson<RootDto>(json);
            if (root?.magicSkillUse != null)
            {
                foreach (MagicRowDto row in root.magicSkillUse)
                {
                    if (row == null || row.skillId <= 0)
                        continue;
                    if (!TryParseKind(row.presentation, out InterludePresentationKind kind))
                    {
                        Debug.LogError(
                            "[PacketPresentation] unknown presentation=\"" + row.presentation +
                            "\" for MagicSkillUse skillId=" + row.skillId);
                        continue;
                    }
                    MagicBySkillId[row.skillId] = kind;
                }
            }

            if (root?.socialAction != null)
            {
                foreach (SocialRowDto row in root.socialAction)
                {
                    if (row == null || row.actionId <= 0)
                        continue;
                    if (!TryParseKind(row.presentation, out InterludePresentationKind kind))
                    {
                        Debug.LogError(
                            "[PacketPresentation] unknown presentation=\"" + row.presentation +
                            "\" for SocialAction actionId=" + row.actionId);
                        continue;
                    }
                    SocialByActionId[row.actionId] = kind;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "[PacketPresentation] failed to parse " + RelativePath + ": " + ex.Message);
        }
    }

    public static bool TryResolveMagicSkillUse(int skillId, out InterludePresentationKind kind)
    {
        EnsureLoaded();
        if (MagicBySkillId.TryGetValue(skillId, out kind))
            return true;
        kind = InterludePresentationKind.None;
        return false;
    }

    public static bool TryResolveSocialAction(int actionId, out InterludePresentationKind kind)
    {
        EnsureLoaded();
        if (SocialByActionId.TryGetValue(actionId, out kind))
            return true;
        kind = InterludePresentationKind.None;
        return false;
    }

    static bool TryParseKind(string raw, out InterludePresentationKind kind)
    {
        if (string.IsNullOrEmpty(raw))
        {
            kind = InterludePresentationKind.None;
            return false;
        }

        if (Enum.TryParse(raw, ignoreCase: true, out kind))
            return kind != InterludePresentationKind.None;
        kind = InterludePresentationKind.None;
        return false;
    }

    [Serializable]
    sealed class RootDto
    {
        public MagicRowDto[] magicSkillUse;
        public SocialRowDto[] socialAction;
    }

    [Serializable]
    sealed class MagicRowDto
    {
        public int skillId;
        public string presentation;
        public string mobiusSource;
    }

    [Serializable]
    sealed class SocialRowDto
    {
        public int actionId;
        public string presentation;
        public string mobiusSource;
    }
}