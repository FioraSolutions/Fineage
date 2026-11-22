using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// One skill-visual resolution for every cast.
/// skillgrp <c>desc</c> names the SkillVisualEffect (for example <c>skill.wh.1012</c>).
/// When that path is empty, the skill id's row in Animations/skill.usk is the visual.
/// When both are absent the original client plays ani_char and sound only.
/// </summary>
public sealed class SkillVisualCatalog
{
    public const string SourceDesc = "skillgrp.desc";
    public const string SourceUsk = "skill.usk";

    // Unreal object names are case-insensitive. The group token in skillgrp
    // (ph, wh, oa) is not the usk outer name (phenomenon, White, at); the
    // SkillVisualEffect object name is the last path segment and is unique.
    readonly Dictionary<int, Dictionary<int, string>> _descBySkillLevel =
        new Dictionary<int, Dictionary<int, string>>();
    readonly Dictionary<string, List<UskAction>> _byClass =
        new Dictionary<string, List<UskAction>>(StringComparer.OrdinalIgnoreCase);
    readonly Dictionary<string, string> _classByObjectName =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    readonly Dictionary<int, List<UskAction>> _bySkillId = new Dictionary<int, List<UskAction>>();
    readonly Dictionary<int, HashSet<string>> _classNamesBySkillId = new Dictionary<int, HashSet<string>>();

    public int SkillCount => _descBySkillLevel.Count;

    public IEnumerable<int> SkillIds => _descBySkillLevel.Keys;

    public static bool TryParse(string skillgrpDescText, string uskText, out SkillVisualCatalog catalog, out string error)
    {
        catalog = null;
        error = null;
        if (string.IsNullOrEmpty(skillgrpDescText))
        {
            error = "skillgrp desc table is empty";
            return false;
        }

        if (string.IsNullOrEmpty(uskText))
        {
            error = "skill.usk table is empty";
            return false;
        }

        var parsed = new SkillVisualCatalog();
        if (!parsed.LoadDesc(skillgrpDescText, out error))
            return false;
        if (!parsed.LoadUsk(uskText, out error))
            return false;
        if (parsed._descBySkillLevel.Count == 0 || parsed._byClass.Count == 0)
        {
            error = "skill visual tables parsed no rows";
            return false;
        }

        catalog = parsed;
        return true;
    }

    public bool ContainsSkill(int skillId)
    {
        return _descBySkillLevel.ContainsKey(skillId);
    }

    public SkillVisualResolution Resolve(int skillId, int level, Func<string, bool> hasAsset)
    {
        return Resolve(skillId, level, hasAsset, null);
    }

    SkillVisualResolution Resolve(
        int skillId,
        int level,
        Func<string, bool> hasAsset,
        HashSet<int> visited)
    {
        var result = new SkillVisualResolution
        {
            SkillId = skillId,
            Level = level,
            Status = SkillVisualStatus.None
        };

        Dictionary<int, string> levels;
        if (!_descBySkillLevel.TryGetValue(skillId, out levels))
        {
            List<UskAction> orphan;
            HashSet<string> orphanNames;
            if (TrySingleUskSkill(skillId, out orphan, out orphanNames))
            {
                FillFromActions(result, SourceUsk, orphan[0].ClassName, orphan, hasAsset);
                return result;
            }

            if (orphanNames != null && orphanNames.Count > 1)
            {
                MarkAmbiguous(result, orphanNames);
                return result;
            }

            return result;
        }

        string desc;
        if (!levels.TryGetValue(level, out desc))
        {
            result.Status = SkillVisualStatus.Missing;
            result.Source = SourceDesc;
            result.VisualKey = "skillgrp";
            result.MissingAssets.Add("skillgrp level=" + level.ToString(CultureInfo.InvariantCulture));
            return result;
        }

        if (IsDescPath(desc))
        {
            List<UskAction> rows;
            string className;
            if (!TryMatchDesc(desc, out className, out rows))
            {
                int followId;
                if (TryParseSkillIdRef(desc, out followId) && followId != skillId)
                    return FollowSkillDesc(skillId, level, desc, followId, hasAsset, visited);

                result.Status = SkillVisualStatus.Missing;
                result.Source = SourceDesc;
                result.VisualKey = desc;
                result.MissingAssets.Add(desc);
                return result;
            }

            FillFromActions(result, SourceDesc, className, rows, hasAsset);
            return result;
        }

        List<UskAction> own;
        HashSet<string> ownNames;
        if (TrySingleUskSkill(skillId, out own, out ownNames))
        {
            FillFromActions(result, SourceUsk, own[0].ClassName, own, hasAsset);
            return result;
        }

        if (ownNames != null && ownNames.Count > 1)
        {
            MarkAmbiguous(result, ownNames);
            return result;
        }

        return result;
    }

    /// <summary>
    /// Skill-id rollup used by the catalog test. Missing wins over Ready.
    /// None only when every level has no desc path and no usk entry.
    /// </summary>
    public SkillVisualStatus Rollup(int skillId, Func<string, bool> hasAsset, List<string> missingRows)
    {
        Dictionary<int, string> levels;
        if (!_descBySkillLevel.TryGetValue(skillId, out levels) || levels.Count == 0)
        {
            SkillVisualResolution orphan = Resolve(skillId, 0, hasAsset);
            AppendMissingRows(orphan, missingRows);
            return orphan.Status;
        }

        var ordered = new List<int>(levels.Keys);
        ordered.Sort();
        bool anyReady = false;
        bool anyMissing = false;
        for (int i = 0; i < ordered.Count; i++)
        {
            SkillVisualResolution level = Resolve(skillId, ordered[i], hasAsset);
            if (level.Status == SkillVisualStatus.Ready)
                anyReady = true;
            else if (level.Status == SkillVisualStatus.Missing)
            {
                anyMissing = true;
                AppendMissingRows(level, missingRows);
            }
        }

        if (anyMissing)
            return SkillVisualStatus.Missing;
        if (anyReady)
            return SkillVisualStatus.Ready;
        return SkillVisualStatus.None;
    }

    public static void AppendMissingRows(SkillVisualResolution resolution, List<string> rows)
    {
        if (resolution == null || rows == null || resolution.Status != SkillVisualStatus.Missing)
            return;

        for (int i = 0; i < resolution.MissingAssets.Count; i++)
        {
            rows.Add(
                resolution.SkillId.ToString(CultureInfo.InvariantCulture) + "\t" +
                resolution.Level.ToString(CultureInfo.InvariantCulture) + "\t" +
                (resolution.Source ?? string.Empty) + "\t" +
                (resolution.VisualKey ?? string.Empty) + "\t" +
                resolution.MissingAssets[i]);
        }
    }

    public static string ChoosePrefabResourcePath(IList<string> resourcePaths)
    {
        string best = null;
        if (resourcePaths == null)
            return null;

        for (int i = 0; i < resourcePaths.Count; i++)
        {
            string path = resourcePaths[i];
            if (string.IsNullOrEmpty(path))
                continue;
            if (best == null ||
                path.Length < best.Length ||
                (path.Length == best.Length && string.CompareOrdinal(path, best) < 0))
            {
                best = path;
            }
        }

        return best;
    }

    static void MarkAmbiguous(SkillVisualResolution result, HashSet<string> names)
    {
        result.Status = SkillVisualStatus.Missing;
        result.Source = SourceUsk;
        result.VisualKey = "ambiguous";
        foreach (string name in names)
            result.MissingAssets.Add(name);
        result.MissingAssets.Sort(StringComparer.Ordinal);
    }

    void FillFromActions(
        SkillVisualResolution result,
        string source,
        string className,
        List<UskAction> rows,
        Func<string, bool> hasAsset)
    {
        result.Source = source;
        result.VisualKey = className;
        result.FlyingTime = rows[0].FlyingTime;
        for (int i = 0; i < rows.Count; i++)
        {
            UskAction flying = rows[i];
            if (!IsFlyingShotShape(flying) || flying.FlyingTime > 0f)
                continue;
            result.MissingAssets.Add(
                "phase:shot missing field=FlyingTime effect=" + (flying.AssetKey ?? string.Empty));
        }

        bool projectile = false;
        for (int i = 0; i < rows.Count; i++)
        {
            if (IsProjectile(rows[i], result.FlyingTime))
            {
                projectile = true;
                break;
            }
        }

        for (int i = 0; i < rows.Count; i++)
        {
            UskAction row = rows[i];
            string phaseError;
            if (!TryMapPhase(row, projectile, out CompositePartSpawnTiming timing, out SkillVisualPartKind kind, out phaseError))
            {
                result.MissingAssets.Add(phaseError + ":" + row.AssetKey);
                continue;
            }

            EffectAttachmentPoint attachment;
            bool follow;
            bool namedBone;
            string attachError;
            if (!TryMapAttach(row, kind, projectile, out attachment, out follow, out namedBone, out attachError))
            {
                result.MissingAssets.Add(attachError + ":" + row.AssetKey);
                continue;
            }

            if (string.IsNullOrEmpty(row.EffectClass))
            {
                result.MissingAssets.Add(
                    "phase:" + (row.Phase ?? string.Empty) + " missing field=EffectClass");
                continue;
            }

            if (IsFlyingShotShape(row) && row.FlyingTime <= 0f)
                continue;

            bool present = hasAsset != null && hasAsset(row.AssetKey);
            if (!present)
                result.MissingAssets.Add(row.AssetKey);

            var action = new SkillVisualAction
            {
                Phase = row.Phase,
                EffectClass = row.EffectClass,
                AssetKey = row.AssetKey,
                AttachOn = row.AttachOn,
                AttachName = row.AttachName,
                Bone = row.Bone,
                OnTarget = row.OnTarget,
                OnMultiTarget = row.OnMultiTarget,
                RelativeToCylinder = row.RelativeToCylinder,
                OffsetX = row.OffsetX,
                OffsetY = row.OffsetY,
                OffsetZ = row.OffsetZ,
                Delay = row.Delay,
                Absolute = row.Absolute,
                UseCharacterRotation = row.UseCharacterRotation,
                SizeScale = row.SizeScale,
                DrawScale = row.DrawScale,
                RotationPitch = row.RotationPitch,
                RotationYaw = row.RotationYaw,
                RotationRoll = row.RotationRoll,
                RotPerSecondPitch = row.RotPerSecondPitch,
                RotPerSecondYaw = row.RotPerSecondYaw,
                RotPerSecondRoll = row.RotPerSecondRoll,
                RotEmitter = row.RotEmitter,
                SelfRotation = row.SelfRotation,
                Directional = row.Directional,
                Speed = row.Speed,
                AccSpeed = row.AccSpeed,
                Kind = kind,
                Timing = timing,
                Attachment = attachment,
                Follow = follow,
                NamedBone = namedBone,
                AssetPresent = present
            };
            if (action.Absolute && action.Delay > 0f)
                action.Timing = CompositePartSpawnTiming.Immediate;
            result.Actions.Add(action);
        }

        result.Status = result.MissingAssets.Count > 0
            ? SkillVisualStatus.Missing
            : SkillVisualStatus.Ready;
    }

    /// <summary>
    /// True when skill.usk has a shot that is a flying part (_fl / _pr / _fo, not on the target),
    /// whether or not FlyingTime was authored. NPC skill 4573 is not player skill 6.
    /// </summary>
    public bool HasOfficialFlyingPart(int skillId)
    {
        List<UskAction> rows;
        if (!_bySkillId.TryGetValue(skillId, out rows))
            return false;
        for (int i = 0; i < rows.Count; i++)
        {
            if (IsFlyingShotShape(rows[i]))
                return true;
        }

        return false;
    }

    static bool IsFlyingShotShape(UskAction row)
    {
        if (row == null || row.OnTarget)
            return false;
        if (!string.Equals(row.Phase, "shot", StringComparison.Ordinal))
            return false;
        string key = row.AssetKey ?? string.Empty;
        return key.EndsWith("_fl", StringComparison.Ordinal) ||
               key.EndsWith("_pr", StringComparison.Ordinal) ||
               key.EndsWith("_fo", StringComparison.Ordinal);
    }

    static bool IsProjectile(UskAction row, float flyingTime)
    {
        if (flyingTime <= 0f || row.OnTarget || !string.Equals(row.Phase, "shot", StringComparison.Ordinal))
            return false;

        string key = row.AssetKey;
        return key.EndsWith("_fl", StringComparison.Ordinal) ||
               key.EndsWith("_pr", StringComparison.Ordinal) ||
               key.EndsWith("_fo", StringComparison.Ordinal);
    }

    static bool TryMapPhase(
        UskAction row,
        bool hasProjectile,
        out CompositePartSpawnTiming timing,
        out SkillVisualPartKind kind,
        out string error)
    {
        timing = CompositePartSpawnTiming.Immediate;
        kind = SkillVisualPartKind.Stationary;
        error = null;
        string phase = row.Phase;
        if (phase == "casting" || phase == "cast" || phase == "channeling" || phase == "channel")
            return true;

        if (phase == "preshot")
        {
            timing = CompositePartSpawnTiming.OnAnimationShoot;
            return true;
        }

        if (phase == "shot")
        {
            timing = CompositePartSpawnTiming.OnAnimationShoot;
            if (IsProjectile(row, row.FlyingTime))
                kind = SkillVisualPartKind.Projectile;
            else if (row.OnTarget)
            {
                kind = SkillVisualPartKind.Impact;
                timing = hasProjectile
                    ? CompositePartSpawnTiming.OnHitCollider
                    : CompositePartSpawnTiming.OnAnimationShoot;
            }

            return true;
        }

        if (phase == "explosion" || phase == "hit")
        {
            kind = SkillVisualPartKind.Impact;
            timing = hasProjectile
                ? CompositePartSpawnTiming.OnHitCollider
                : CompositePartSpawnTiming.OnHitTime;
            return true;
        }

        error = "phase:" + (phase ?? string.Empty);
        return false;
    }

    static bool TryMapAttach(
        UskAction row,
        SkillVisualPartKind kind,
        bool hasProjectile,
        out EffectAttachmentPoint attachment,
        out bool follow,
        out bool namedBone,
        out string error)
    {
        attachment = EffectAttachmentPoint.CasterRoot;
        follow = false;
        namedBone = false;
        error = null;

        if (kind == SkillVisualPartKind.Impact && hasProjectile)
        {
            attachment = EffectAttachmentPoint.WorldHitPoint;
            follow = true;
            return true;
        }

        switch (row.AttachOn)
        {
            case 0:
                attachment = row.OnTarget ? EffectAttachmentPoint.TargetCenter : EffectAttachmentPoint.CasterRoot;
                follow = false;
                return true;
            case 1:
                attachment = EffectAttachmentPoint.CasterPosition;
                follow = false;
                return true;
            case 2:
                attachment = EffectAttachmentPoint.WeaponSocket;
                follow = true;
                return true;
            case 3:
                attachment = EffectAttachmentPoint.LeftWeaponSocket;
                follow = true;
                return true;
            case 7:
            case 8:
                namedBone = true;
                follow = true;
                attachment = row.OnTarget ? EffectAttachmentPoint.TargetCenter : EffectAttachmentPoint.CasterCenter;
                if (string.IsNullOrEmpty(row.Bone))
                {
                    error = "attach_on=" + row.AttachOn.ToString(CultureInfo.InvariantCulture) + " without bone";
                    return false;
                }

                return true;
            case 9:
                attachment = row.OnTarget ? EffectAttachmentPoint.TargetCenter : EffectAttachmentPoint.CasterCenter;
                follow = true;
                return true;
            case 101:
            case 102:
                namedBone = true;
                follow = true;
                attachment = row.OnTarget ? EffectAttachmentPoint.TargetCenter : EffectAttachmentPoint.CasterCenter;
                if (string.IsNullOrEmpty(row.Bone))
                {
                    error = "attach_on=" + row.AttachOn.ToString(CultureInfo.InvariantCulture) + " without bone";
                    return false;
                }

                return true;
            default:
                error = "attach_on=" + row.AttachOn.ToString(CultureInfo.InvariantCulture);
                return false;
        }
    }


    SkillVisualResolution FollowSkillDesc(
        int skillId,
        int level,
        string desc,
        int followId,
        Func<string, bool> hasAsset,
        HashSet<int> visited)
    {
        if (visited == null)
            visited = new HashSet<int>();
        if (!visited.Add(skillId) || visited.Contains(followId))
        {
            Debug.LogError(
                "[SkillVisual] skill=" + skillId.ToString(CultureInfo.InvariantCulture) +
                " desc=" + desc +
                " followed skill=" + followId.ToString(CultureInfo.InvariantCulture) +
                " cycle");
            var cycle = new SkillVisualResolution
            {
                SkillId = skillId,
                Level = level,
                Status = SkillVisualStatus.Missing,
                Source = SourceDesc,
                VisualKey = desc
            };
            cycle.MissingAssets.Add(
                desc + " followed skill=" + followId.ToString(CultureInfo.InvariantCulture) + " cycle");
            return cycle;
        }

        SkillVisualResolution followed = Resolve(
            followId,
            ChooseFollowLevel(followId, level),
            hasAsset,
            visited);
        followed.SkillId = skillId;
        followed.Level = level;
        if (followed.Status == SkillVisualStatus.Ready ||
            followed.Status == SkillVisualStatus.Missing)
        {
            if (followed.Status == SkillVisualStatus.Missing && followed.MissingAssets.Count == 0)
            {
                followed.Source = SourceDesc;
                followed.VisualKey = desc;
                followed.MissingAssets.Add(
                    desc + " followed skill=" + followId.ToString(CultureInfo.InvariantCulture));
            }

            return followed;
        }

        Debug.LogError(
            "[SkillVisual] skill=" + skillId.ToString(CultureInfo.InvariantCulture) +
            " desc=" + desc +
            " followed skill=" + followId.ToString(CultureInfo.InvariantCulture) +
            " empty desc");
        var missing = new SkillVisualResolution
        {
            SkillId = skillId,
            Level = level,
            Status = SkillVisualStatus.Missing,
            Source = SourceDesc,
            VisualKey = desc
        };
        missing.MissingAssets.Add(
            desc + " followed skill=" + followId.ToString(CultureInfo.InvariantCulture) +
            " empty desc");
        return missing;
    }

    int ChooseFollowLevel(int followId, int requested)
    {
        Dictionary<int, string> levels;
        if (!_descBySkillLevel.TryGetValue(followId, out levels) || levels.Count == 0)
            return requested;
        if (levels.ContainsKey(requested))
            return requested;
        var ordered = new List<int>(levels.Keys);
        ordered.Sort();
        return ordered[0];
    }

    static bool TryParseSkillIdRef(string desc, out int skillId)
    {
        skillId = 0;
        if (!IsDescPath(desc))
            return false;
        return int.TryParse(
            ObjectName(desc),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out skillId) && skillId > 0;
    }

    bool TryMatchDesc(string desc, out string className, out List<UskAction> rows)
    {
        className = null;
        rows = null;
        List<UskAction> exact;
        if (_byClass.TryGetValue(desc, out exact) && exact.Count > 0)
        {
            className = exact[0].ClassName;
            rows = exact;
            return true;
        }

        string objectName = ObjectName(desc);
        string unique;
        if (string.IsNullOrEmpty(objectName) || !_classByObjectName.TryGetValue(objectName, out unique))
            return false;
        if (string.IsNullOrEmpty(unique))
            return false;
        if (!_byClass.TryGetValue(unique, out rows) || rows.Count == 0)
            return false;
        className = rows[0].ClassName;
        return true;
    }

    bool TrySingleUskSkill(int skillId, out List<UskAction> rows, out HashSet<string> names)
    {
        rows = null;
        names = null;
        if (!_classNamesBySkillId.TryGetValue(skillId, out names) || names.Count == 0)
            return false;
        if (names.Count != 1)
            return false;
        return _bySkillId.TryGetValue(skillId, out rows) && rows.Count > 0;
    }

    bool LoadDesc(string text, out string error)
    {
        error = null;
        string[] lines = text.Split('\n');
        bool header = false;
        int idCol = 0;
        int levelCol = 1;
        int descCol = 2;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].TrimEnd('\r');
            if (line.Length == 0 || line[0] == '#')
                continue;

            string[] cols = line.Split('\t');
            if (!header)
            {
                header = true;
                if (cols.Length > 0 &&
                    cols[0].IndexOf("skill_id", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    idCol = IndexOf(cols, "skill_id");
                    levelCol = IndexOf(cols, "level");
                    if (levelCol < 0)
                        levelCol = IndexOf(cols, "skill_level");
                    descCol = IndexOf(cols, "desc");
                    if (idCol < 0 || levelCol < 0 || descCol < 0)
                    {
                        error = "skillgrp desc header is missing skill_id, level, or desc";
                        return false;
                    }

                    continue;
                }
            }

            if (cols.Length <= descCol || cols.Length <= idCol || cols.Length <= levelCol)
                continue;

            int skillId;
            int level;
            if (!int.TryParse(cols[idCol].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out skillId) ||
                !int.TryParse(cols[levelCol].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out level))
            {
                continue;
            }

            Dictionary<int, string> levels;
            if (!_descBySkillLevel.TryGetValue(skillId, out levels))
            {
                levels = new Dictionary<int, string>();
                _descBySkillLevel[skillId] = levels;
            }

            levels[level] = cols[descCol].Trim();
        }

        return true;
    }

    bool LoadUsk(string text, out string error)
    {
        error = null;
        string[] lines = text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string probe = lines[i].TrimEnd('\r');
            if (probe.Length == 0 || probe[0] == '#')
                continue;
            string[] headerCols = probe.Split('\t');
            if (IndexOf(headerCols, "attach_on_raw") >= 0)
                return LoadPlacement(lines, out error);
            break;
        }

        bool header = false;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].TrimEnd('\r');
            if (line.Length == 0 || line[0] == '#')
                continue;
            string[] cols = line.Split('\t');
            if (!header)
            {
                header = true;
                if (cols.Length > 0 && cols[0].IndexOf("skill_id", StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;
            }

            if (cols.Length < 5)
                continue;

            int skillId;
            if (!int.TryParse(cols[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out skillId))
                continue;

            string effect = cols[4].Trim();
            if (effect.Length == 0)
                continue;

            var row = new UskAction
            {
                SkillId = skillId,
                ClassName = cols[1].Trim(),
                FlyingTime = ParseFloat(cols, 2),
                Phase = cols[3].Trim().ToLowerInvariant(),
                EffectClass = effect,
                AssetKey = AssetKeyOf(effect),
                AttachOn = ParseInt(cols, 5),
                AttachName = Col(cols, 6),
                Bone = Col(cols, 7),
                OnTarget = ParseBool(cols, 8),
                OnMultiTarget = ParseBool(cols, 9),
                RelativeToCylinder = ParseBool(cols, 10),
                OffsetX = ParseFloat(cols, 11),
                OffsetY = ParseFloat(cols, 12),
                OffsetZ = ParseFloat(cols, 13),
                Delay = ParseFloat(cols, 14),
                Absolute = ParseBool(cols, 15)
            };

            List<UskAction> classRows;
            if (!_byClass.TryGetValue(row.ClassName, out classRows))
            {
                classRows = new List<UskAction>();
                _byClass[row.ClassName] = classRows;
            }

            classRows.Add(row);

            string objectName = ObjectName(row.ClassName);
            string previous;
            if (!_classByObjectName.TryGetValue(objectName, out previous))
                _classByObjectName[objectName] = row.ClassName;
            else if (!string.Equals(previous, row.ClassName, StringComparison.OrdinalIgnoreCase))
                _classByObjectName[objectName] = string.Empty;

            List<UskAction> skillRows;
            if (!_bySkillId.TryGetValue(skillId, out skillRows))
            {
                skillRows = new List<UskAction>();
                _bySkillId[skillId] = skillRows;
            }

            skillRows.Add(row);

            HashSet<string> names;
            if (!_classNamesBySkillId.TryGetValue(skillId, out names))
            {
                names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _classNamesBySkillId[skillId] = names;
            }

            names.Add(row.ClassName);
        }

        return true;
    }

    bool LoadPlacement(string[] lines, out string error)
    {
        error = null;
        int skillIdCol = -1;
        int objectNameCol = -1;
        int visualKeyCol = -1;
        int flyingCol = -1;
        int phaseCol = -1;
        int effectCol = -1;
        int attachVisualCol = -1;
        int attachNameCol = -1;
        int boneCol = -1;
        int offsetXCol = -1;
        int offsetYCol = -1;
        int offsetZCol = -1;
        int delayCol = -1;
        int absoluteCol = -1;
        int multiCol = -1;
        int relCol = -1;
        int sizeScaleCol = -1;
        int onTargetCol = -1;
        int useRotCol = -1;
        int drawScaleCol = -1;
        int rotPCol = -1;
        int rotYCol = -1;
        int rotRCol = -1;
        int rpsPCol = -1;
        int rpsYCol = -1;
        int rpsRCol = -1;
        int rotEmitterCol = -1;
        int selfRotCol = -1;
        int directionalCol = -1;
        int speedCol = -1;
        int accSpeedCol = -1;
        bool header = false;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].TrimEnd('\r');
            if (line.Length == 0 || line[0] == '#')
                continue;
            string[] cols = line.Split('\t');
            if (!header)
            {
                header = true;
                if (!RequireColumn(cols, "skill_id", out skillIdCol, ref error) ||
                    !RequireColumn(cols, "object_name", out objectNameCol, ref error) ||
                    !RequireColumn(cols, "visual_key", out visualKeyCol, ref error) ||
                    !RequireColumn(cols, "flying_time", out flyingCol, ref error) ||
                    !RequireColumn(cols, "phase", out phaseCol, ref error) ||
                    !RequireColumn(cols, "effect_class", out effectCol, ref error) ||
                    !RequireColumn(cols, "attach_on_visual", out attachVisualCol, ref error) ||
                    !RequireColumn(cols, "attach_on_visual_name", out attachNameCol, ref error) ||
                    !RequireColumn(cols, "attach_bone_name", out boneCol, ref error) ||
                    !RequireColumn(cols, "offset_x", out offsetXCol, ref error) ||
                    !RequireColumn(cols, "offset_y", out offsetYCol, ref error) ||
                    !RequireColumn(cols, "offset_z", out offsetZCol, ref error) ||
                    !RequireColumn(cols, "spawn_delay", out delayCol, ref error) ||
                    !RequireColumn(cols, "b_absolute", out absoluteCol, ref error) ||
                    !RequireColumn(cols, "b_on_multi_target", out multiCol, ref error) ||
                    !RequireColumn(cols, "b_relative_to_cylinder", out relCol, ref error) ||
                    !RequireColumn(cols, "b_size_scale", out sizeScaleCol, ref error) ||
                    !RequireColumn(cols, "b_spawn_on_target", out onTargetCol, ref error) ||
                    !RequireColumn(cols, "b_use_character_rotation", out useRotCol, ref error) ||
                    !RequireColumn(cols, "draw_scale", out drawScaleCol, ref error) ||
                    !RequireColumn(cols, "rotation_pitch", out rotPCol, ref error) ||
                    !RequireColumn(cols, "rotation_yaw", out rotYCol, ref error) ||
                    !RequireColumn(cols, "rotation_roll", out rotRCol, ref error) ||
                    !RequireColumn(cols, "rot_per_second_pitch", out rpsPCol, ref error) ||
                    !RequireColumn(cols, "rot_per_second_yaw", out rpsYCol, ref error) ||
                    !RequireColumn(cols, "rot_per_second_roll", out rpsRCol, ref error) ||
                    !RequireColumn(cols, "b_rot_emitter", out rotEmitterCol, ref error) ||
                    !RequireColumn(cols, "b_self_rotation", out selfRotCol, ref error) ||
                    !RequireColumn(cols, "b_directional", out directionalCol, ref error) ||
                    !RequireColumn(cols, "speed", out speedCol, ref error) ||
                    !RequireColumn(cols, "acc_speed", out accSpeedCol, ref error))
                {
                    return false;
                }

                continue;
            }

            int skillId;
            if (!int.TryParse(Col(cols, skillIdCol), NumberStyles.Integer, CultureInfo.InvariantCulture, out skillId))
                continue;

            string visualKey = Col(cols, visualKeyCol);
            if (visualKey.Length == 0)
                continue;

            string effect = Col(cols, effectCol);
            string objectName = Col(cols, objectNameCol);
            var row = new UskAction
            {
                SkillId = skillId,
                ClassName = visualKey,
                FlyingTime = ParseFloat(cols, flyingCol),
                Phase = Col(cols, phaseCol).ToLowerInvariant(),
                EffectClass = effect,
                AssetKey = AssetKeyOf(effect),
                AttachOn = ParseInt(cols, attachVisualCol),
                AttachName = Col(cols, attachNameCol),
                Bone = Col(cols, boneCol),
                OnTarget = ParseBool(cols, onTargetCol),
                OnMultiTarget = ParseBool(cols, multiCol),
                RelativeToCylinder = ParseBool(cols, relCol),
                OffsetX = ParseFloat(cols, offsetXCol),
                OffsetY = ParseFloat(cols, offsetYCol),
                OffsetZ = ParseFloat(cols, offsetZCol),
                Delay = ParseFloat(cols, delayCol),
                Absolute = ParseBool(cols, absoluteCol),
                UseCharacterRotation = ParseBool(cols, useRotCol),
                SizeScale = ParseBool(cols, sizeScaleCol),
                DrawScale = ParseFloat(cols, drawScaleCol),
                RotationPitch = ParseFloat(cols, rotPCol),
                RotationYaw = ParseFloat(cols, rotYCol),
                RotationRoll = ParseFloat(cols, rotRCol),
                RotPerSecondPitch = ParseFloat(cols, rpsPCol),
                RotPerSecondYaw = ParseFloat(cols, rpsYCol),
                RotPerSecondRoll = ParseFloat(cols, rpsRCol),
                RotEmitter = ParseBool(cols, rotEmitterCol),
                SelfRotation = ParseBool(cols, selfRotCol),
                Directional = ParseBool(cols, directionalCol),
                Speed = ParseFloat(cols, speedCol),
                AccSpeed = ParseFloat(cols, accSpeedCol)
            };

            List<UskAction> classRows;
            if (!_byClass.TryGetValue(row.ClassName, out classRows))
            {
                classRows = new List<UskAction>();
                _byClass[row.ClassName] = classRows;
            }

            classRows.Add(row);

            string previous;
            if (!_classByObjectName.TryGetValue(objectName, out previous))
                _classByObjectName[objectName] = row.ClassName;
            else if (!string.Equals(previous, row.ClassName, StringComparison.OrdinalIgnoreCase))
                _classByObjectName[objectName] = string.Empty;

            // Variant object names (1012_sec) stay resolvable by visual_key / object
            // name but must not make the skill-id usk fallback ambiguous.
            if (objectName != skillId.ToString(CultureInfo.InvariantCulture))
                continue;

            List<UskAction> skillRows;
            if (!_bySkillId.TryGetValue(skillId, out skillRows))
            {
                skillRows = new List<UskAction>();
                _bySkillId[skillId] = skillRows;
            }

            skillRows.Add(row);

            HashSet<string> names;
            if (!_classNamesBySkillId.TryGetValue(skillId, out names))
            {
                names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _classNamesBySkillId[skillId] = names;
            }

            names.Add(row.ClassName);
        }

        return true;
    }

    static bool RequireColumn(string[] header, string name, out int index, ref string error)
    {
        index = IndexOf(header, name);
        if (index >= 0)
            return true;
        error = "skill-effect-placement header is missing " + name;
        return false;
    }

    static bool IsDescPath(string desc)
    {
        return !string.IsNullOrEmpty(desc) &&
               desc.StartsWith("skill.", StringComparison.OrdinalIgnoreCase);
    }

    static string ObjectName(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        int slash = path.LastIndexOf('.');
        return slash >= 0 && slash + 1 < path.Length ? path.Substring(slash + 1) : path;
    }

    static string AssetKeyOf(string effectClass)
    {
        return ObjectName(effectClass).ToLowerInvariant();
    }

    static int IndexOf(string[] cols, string name)
    {
        for (int i = 0; i < cols.Length; i++)
        {
            if (string.Equals(cols[i].Trim(), name, StringComparison.OrdinalIgnoreCase))
                return i;
        }

        return -1;
    }

    static string Col(string[] cols, int index)
    {
        return index >= 0 && index < cols.Length ? cols[index].Trim() : string.Empty;
    }

    static int ParseInt(string[] cols, int index)
    {
        int value;
        if (index < cols.Length &&
            int.TryParse(cols[index].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
        {
            return value;
        }

        return 0;
    }

    static float ParseFloat(string[] cols, int index)
    {
        float value;
        if (index < cols.Length &&
            float.TryParse(cols[index].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            return value;
        }

        return 0f;
    }

    static bool ParseBool(string[] cols, int index)
    {
        if (index >= cols.Length)
            return false;
        string text = cols[index].Trim();
        return text == "1" || string.Equals(text, "true", StringComparison.OrdinalIgnoreCase);
    }

    sealed class UskAction
    {
        public int SkillId;
        public string ClassName;
        public float FlyingTime;
        public string Phase;
        public string EffectClass;
        public string AssetKey;
        public int AttachOn;
        public string AttachName;
        public string Bone;
        public bool OnTarget;
        public bool OnMultiTarget;
        public bool RelativeToCylinder;
        public float OffsetX;
        public float OffsetY;
        public float OffsetZ;
        public float Delay;
        public bool Absolute;
        public bool UseCharacterRotation;
        public bool SizeScale;
        public float DrawScale;
        public float RotationPitch;
        public float RotationYaw;
        public float RotationRoll;
        public float RotPerSecondPitch;
        public float RotPerSecondYaw;
        public float RotPerSecondRoll;
        public bool RotEmitter;
        public bool SelfRotation;
        public bool Directional;
        public float Speed;
        public float AccSpeed;
    }
}

public enum SkillVisualStatus
{
    None = 0,
    Ready = 1,
    Missing = 2
}

public enum SkillVisualPartKind
{
    Stationary = 0,
    Projectile = 1,
    Impact = 2
}

public sealed class SkillVisualResolution
{
    public int SkillId;
    public int Level;
    public SkillVisualStatus Status;
    public string Source;
    public string VisualKey;
    public float FlyingTime;
    public readonly List<SkillVisualAction> Actions = new List<SkillVisualAction>();
    public readonly List<string> MissingAssets = new List<string>();
}

public sealed class SkillVisualAction
{
    public string Phase;
    public string EffectClass;
    public string AssetKey;
    public int AttachOn;
    public string AttachName;
    public string Bone;
    public bool OnTarget;
    public bool OnMultiTarget;
    public bool RelativeToCylinder;
    public float OffsetX;
    public float OffsetY;
    public float OffsetZ;
    public float Delay;
    public bool Absolute;
    public bool UseCharacterRotation;
    public bool SizeScale;
    public float DrawScale;
    public float RotationPitch;
    public float RotationYaw;
    public float RotationRoll;
    public float RotPerSecondPitch;
    public float RotPerSecondYaw;
    public float RotPerSecondRoll;
    public bool RotEmitter;
    public bool SelfRotation;
    public bool Directional;
    public float Speed;
    public float AccSpeed;
    public SkillVisualPartKind Kind;
    public CompositePartSpawnTiming Timing;
    public EffectAttachmentPoint Attachment;
    public bool Follow;
    public bool NamedBone;
    public bool AssetPresent;
}
