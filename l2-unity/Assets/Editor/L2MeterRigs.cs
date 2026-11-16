#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// One migratable skeleton unit: a folder that owns its own Models/, Clips/ and the
/// anim prefab chain. Discovered from disk rather than hard-coded, because the
/// Animations tree is not uniform:
///
///   Fighter/MFighter            Models/anim/MFighter_anim.fbx        (pilot shape)
///   Elf/MElf                    Models/anim/MElf_anim_user.fbx       (anim named *_user)
///   Shaman/MShaman              Models/anim only — clips/prefabs live in Orc/MShaman
///   Fighter/FFighter            no anim FBX at all — skeleton comes from the armors
///
/// A rig is `&lt;Race&gt;/&lt;Actor&gt;` when it has Models/anim or an *_anim_player prefab,
/// otherwise the leaf directory that holds the models (`.../Model`, `monster_m00`).
/// Weapons are deliberately excluded: they are shared, carry no skeleton, and were
/// migrated separately.
/// </summary>
public sealed class MeterRig
{
    /// <summary>`Assets/.../Animations/&lt;id&gt;`, the key used for baselines.</summary>
    public string Id;
    /// <summary>Absolute asset folder that owns the Models/ folder.</summary>
    public string Root;
    public string Models;
    /// <summary>Anim FBX reimports as the skeleton the clips bind to; may be null.</summary>
    public string AnimFbx;
    /// <summary>`Models/anim/&lt;Actor&gt;_anim.prefab`, the flattened skeleton; may be null.</summary>
    public string AnimPrefab;
    /// <summary>`&lt;Root&gt;/&lt;Actor&gt;_anim_player.prefab` etc.</summary>
    public readonly List<string> AnimPrefabs = new List<string>();
    public readonly List<string> ClipDirs = new List<string>();
    /// <summary>Folders searched for gear prefabs that hold the actor's meshes.</summary>
    public readonly List<string> GearDirs = new List<string>();
    /// <summary>Prefab name prefixes that belong to this actor (armor/face/hair/parts).</summary>
    public readonly List<string> NamePrefixes = new List<string>();
    /// <summary>Actor prefabs (Player_/User_/Pawn_) that must be captured by the harness.</summary>
    public readonly List<string> ActorPrefabs = new List<string>();

    /// <summary>
    /// Pilot artefacts were committed before rigs were discovered, so they keep their
    /// original file names; every other rig derives its own from the id.
    /// </summary>
    static string ArtifactName(string id, string suffix)
    {
        string stem = id == "Fighter/MFighter"
            ? "MFighter"
            : id.Replace('/', '_');
        return "Assets/MeterContract/" + stem + suffix;
    }

    public string BaselinePath => ArtifactName(Id, ".baseline.json");
    public string ConvertedClipsPath => ArtifactName(Id, ".clips-converted.json");

    public override string ToString() => Id;
}

/// <summary>
/// Batch argv parsing shared by the migration and the harness.
///
/// Unity passes `-executeMethod` and its value as *separate* argv entries, so a batch
/// invocation looks like: `... -executeMethod L2MeterMigration.RunCli rigs Elf -logFile x`.
/// Anything after the method name and before the next `-flag` is ours.
/// </summary>
public static class BatchArgs
{
    /// <summary>Positional arguments that follow the given `-executeMethod` value.</summary>
    public static string[] After(string method)
    {
        string[] argv = Environment.GetCommandLineArgs();
        int at = Array.FindIndex(argv, a =>
            a == method || a.EndsWith("." + method, StringComparison.Ordinal));
        if (at < 0)
            return Array.Empty<string>();
        var args = new List<string>();
        for (int i = at + 1; i < argv.Length; i++)
        {
            // A flag ends the positional list; the next entry would be that flag's value
            // (`-logFile /tmp/x`), which must not be mistaken for a rig filter.
            if (argv[i].StartsWith("-", StringComparison.Ordinal))
                break;
            args.Add(argv[i]);
        }
        return args.ToArray();
    }

    /// <summary>First positional argument after the method, or null.</summary>
    public static string First(string method)
    {
        string[] args = After(method);
        return args.Length > 0 ? args[0] : null;
    }
}

public static class L2MeterRigs
{
    public const string Animations = "Assets/Resources/Data/Animations";
    /// <summary>Folders that hold shared geometry rather than one skeleton unit.</summary>
    static readonly string[] Excluded =
    {
        "LineageWeapons", "LineageEffect", "LineageMonsters2",
    };

    public static List<MeterRig> Discover()
    {
        var rigs = new List<MeterRig>();
        foreach (string race in Directory.GetDirectories(Animations))
        {
            string raceName = Path.GetFileName(race);
            if (Excluded.Contains(raceName) || raceName.StartsWith("."))
                continue;
            if (!raceName.Equals("LineageNPCs") && !raceName.Equals("LineageNPCs4") &&
                !raceName.Equals("LineageMonsters"))
            {
                // <Race>/<Actor>: the playable races and the Magic actor pair.
                foreach (string actor in Directory.GetDirectories(race))
                {
                    MeterRig rig = FromActorDirectory(race, actor);
                    if (rig != null)
                        rigs.Add(rig);
                }
                continue;
            }

            // NPC/monster trees use one directory per model, with the clips loose beside
            // the FBX instead of in Clips/. Treat every directory holding an .fbx as a rig.
            foreach (string unit in Directory.GetDirectories(race, "*", SearchOption.AllDirectories))
            {
                if (unit.EndsWith("Model", StringComparison.Ordinal) ||
                    Directory.GetFiles(unit, "*.fbx").Length > 0)
                {
                    MeterRig rig = FromLooseDirectory(race, unit);
                    if (rig != null && rigs.All(r => r.Id != rig.Id))
                        rigs.Add(rig);
                }
            }
        }
        return rigs.OrderBy(r => r.Id, StringComparer.Ordinal).ToList();
    }

    static MeterRig FromActorDirectory(string race, string actorDir)
    {
        string actors = Path.GetFileName(actorDir);
        string models = actorDir + "/Models";
        bool hasModels = Directory.Exists(models);
        bool hasAnim = hasModels && Directory.Exists(models + "/anim");
        bool hasPlayer = Directory.GetFiles(actorDir, "*_anim_player.prefab").Length > 0;
        if (!hasModels || (!hasAnim && !hasPlayer))
            return null;

        var rig = new MeterRig
        {
            Id = Path.GetFileName(race) + "/" + actors,
            Root = actorDir,
            Models = models,
        };
        if (hasAnim)
        {
            string animDir = models + "/anim";
            rig.AnimFbx = FirstFbx(animDir, actors + "_anim.fbx")
                          ?? FirstFbx(animDir, actors + "_anim_user.fbx")
                          ?? Directory.GetFiles(animDir, "*.fbx").OrderBy(p => p).FirstOrDefault();
            // The flattened skeleton is not always `<Actor>_anim.prefab`: Elf/MElf ships
            // `Models/anim/MElf_anim_user.prefab`, and missing it left the whole rig on the
            // legacy 100x carrier while every other rig reported scale 1.
            rig.AnimPrefab = FirstFile(animDir, actors + "_anim.prefab")
                             ?? FirstFile(animDir, actors + "_anim_user.prefab");
        }

        foreach (string prefab in new[]
                 {
                     actorDir + "/" + actors + "_anim_player.prefab",
                     actorDir + "/" + actors + "_anim_user.prefab",
                     actorDir + "/Player_" + actors + ".prefab",
                     actorDir + "/User_" + actors + ".prefab",
                     actorDir + "/Pawn_" + actors + ".prefab",
                 })
        {
            if (!File.Exists(prefab))
                continue;
            if (prefab.Contains("_anim_"))
                rig.AnimPrefabs.Add(prefab);
            else
                rig.ActorPrefabs.Add(prefab);
        }

        // Some rigs keep the flattened skeleton under Models/anim instead of the actor
        // folder (measured: Dwarf/MDwarf ships `Models/anim/MDwarf_anim_player.prefab` and
        // no `MDwarf_anim.prefab`). Without this scan that prefab was never a migration
        // target and kept its legacy `.ao` at scale 100.
        if (hasAnim)
        {
            foreach (string prefab in Directory.GetFiles(models + "/anim", "*_anim_*.prefab"))
            {
                string normalised = prefab.Replace('\\', '/');
                if (rig.AnimPrefabs.All(p => p != normalised))
                    rig.AnimPrefabs.Add(normalised);
            }
        }

        // Clips are authoritative where they live; Shaman keeps them under Orc/ because
        // the orc and shaman actors share one import. Search both, skip the model folder
        // so a clip is never converted twice.
        foreach (string dir in CandidateClipDirs(race, actors))
            if (Directory.Exists(dir) && rig.ClipDirs.All(d => d != dir))
                rig.ClipDirs.Add(dir);

        foreach (string dir in new[] { actorDir, race })
            if (Directory.Exists(dir))
                rig.GearDirs.Add(dir);

        // Human face and hair meshes were shared with the Fighter actor after the race
        // split. Their FBX live in the actor Models folder, but their prefabs stayed in
        // Human/<Actor>/Faces and Hair. Include those directories without sharing clips.
        string human = Path.Combine(Animations, "Human", actors);
        if (Directory.Exists(human))
        {
            if (rig.GearDirs.All(d => d != human))
                rig.GearDirs.Add(human);
            foreach (string dir in new[] { "Faces", "Hair" })
            {
                string parts = Path.Combine(human, dir);
                if (Directory.Exists(parts) && rig.GearDirs.All(d => d != parts))
                    rig.GearDirs.Add(parts);
            }
            foreach (string path in new[] { "Player_", "User_", "Pawn_" }
                         .Select(prefix => Path.Combine(human, prefix + actors + ".prefab")))
                if (File.Exists(path))
                    rig.ActorPrefabs.Add(path);
        }

        rig.NamePrefixes.Add(actors);
        return rig;
    }

    static IEnumerable<string> CandidateClipDirs(string race, string actors)
    {
        string raceName = Path.GetFileName(race);
        yield return Path.Combine(race, actors, "Clips");
        yield return Path.Combine(race, "Clips");
        // Shaman actors live in Orc/ although their FBX live in Shaman/.
        if (actors.EndsWith("Shaman", StringComparison.Ordinal))
        {
            yield return Path.Combine(Animations, "Orc", actors, "Clips");
            yield return Path.Combine(Animations, "Orc", "Clips");
        }
    }

    static MeterRig FromLooseDirectory(string race, string unit)
    {
        string[] fbx = Directory.GetFiles(unit, "*.fbx");
        if (fbx.Length == 0)
            return null;
        string id = unit.Substring(Animations.Length + 1);
        var rig = new MeterRig
        {
            Id = id,
            Root = unit,
            Models = unit,
        };
        if (fbx.Length == 1)
            rig.AnimFbx = fbx[0];
        foreach (string prefab in Directory.GetFiles(unit, "*.anim_player.prefab")
                     .Concat(Directory.GetFiles(unit, "*_anim_player.prefab")))
            if (rig.AnimPrefabs.All(p => p != prefab))
                rig.AnimPrefabs.Add(prefab);

        string basename = Path.GetFileName(fbx.OrderBy(p => p.Length).First());
        rig.NamePrefixes.Add(Path.GetFileNameWithoutExtension(basename));
        rig.GearDirs.Add(unit);
        // Loose town NPCs nest their FBX in Model/ but save the actual character prefab
        // beside that folder. Include the parent, otherwise Verify reports prefabs=0.
        DirectoryInfo parent = Directory.GetParent(unit);
        string assetRoot = Path.GetFullPath(Animations);
        if (parent != null && parent.FullName.StartsWith(assetRoot, StringComparison.Ordinal))
        {
            string relative = Path.GetRelativePath(Path.GetFullPath("."), parent.FullName);
            rig.GearDirs.Add(relative.Replace('\\', '/'));
        }
        return rig;
    }

    static string FirstFbx(string dir, string name)
    {
        string path = dir + "/" + name;
        return File.Exists(path) ? path : null;
    }

    static string FirstFile(string dir, string name)
    {
        string path = dir + "/" + name;
        return File.Exists(path) ? path : null;
    }

    /// <summary>Every `*.fbx` under the rig's model folder, including `Models/anim`.</summary>
    public static IEnumerable<string> ModelFbx(MeterRig rig)
    {
        if (!Directory.Exists(rig.Models))
            yield break;
        foreach (string path in Directory.GetFiles(rig.Models, "*.fbx", SearchOption.AllDirectories))
            yield return path.Replace('\\', '/');
    }

    /// <summary>
    /// Folders of model FBX that no rig owns.
    ///
    /// The tree is not uniform: `Orc/Models` and `Shaman/Models` hold duplicate face meshes
    /// that the per-actor rigs do not cover, and `LineageMonsters2` / the effect static
    /// meshes are not skeleton rigs. They still carry the legacy import contract, so the
    /// migration configures them by folder, not by rig.
    /// </summary>
    public static IEnumerable<string> UnclaimedModelFolders(List<MeterRig> rigs)
    {
        var claimed = new HashSet<string>(
            rigs.Select(r => r.Models.TrimEnd('/')), StringComparer.Ordinal);
        foreach (string folder in Directory.GetDirectories(Animations, "*", SearchOption.AllDirectories))
        {
            string normalised = folder.Replace('\\', '/').TrimEnd('/');
            if (normalised.EndsWith("/anim", StringComparison.Ordinal))
                continue;
            if (claimed.Contains(normalised))
                continue;
            if (Directory.GetFiles(folder, "*.fbx").Length == 0)
                continue;
            // Only the trees the migration actually owns.
            string relative = normalised.Substring(Animations.Length + 1);
            string top = relative.Split('/')[0];
            if (top == "LineageWeapons")
                continue;
            yield return normalised;
        }
    }

    /// <summary>Every `*.anim` under the rig's clip folders and loose next to its models.</summary>
    public static IEnumerable<string> Clips(MeterRig rig)
    {
        foreach (string dir in rig.ClipDirs.Concat(new[] { rig.Models }))
        {
            if (!Directory.Exists(dir))
                continue;
            foreach (string path in Directory.GetFiles(dir, "*.anim", SearchOption.AllDirectories))
                yield return path.Replace('\\', '/');
        }
    }

    /// <summary>Actor/gear prefabs that the harness snapshots for this rig.</summary>
    public static IEnumerable<string> Prefabs(MeterRig rig)
    {
        foreach (string path in rig.ActorPrefabs)
            yield return path;
        // The flattened skeleton itself is a prefab the migration edits, so it has to be
        // in the baseline: capturing it later turns every sync into a "missing prefab".
        if (!string.IsNullOrEmpty(rig.AnimPrefab))
            yield return rig.AnimPrefab;
        foreach (string path in GearedPrefabs(rig))
            yield return path;
    }

    /// <summary>
    /// Gear (armor/face/hair) prefabs of this actor: nested FBX instances that carry the
    /// stale `m_LocalScale = 100` override the migration has to clear, plus the prefabs that
    /// hold their own copy of the skeleton.
    ///
    /// Subfolders are included: every race keeps its parts in `<Actor>/Hair` and
    /// `<Actor>/Faces`, and a top-level-only scan skipped them (measured: 223 renderers
    /// still drawing 100x large after the migration reported success).
    /// </summary>
    public static IEnumerable<string> GearedPrefabs(MeterRig rig)
    {
        foreach (string dir in rig.GearDirs.Distinct())
        {
            if (!Directory.Exists(dir))
                continue;
            foreach (string path in Directory.GetFiles(dir, "*.prefab", SearchOption.AllDirectories))
            {
                string name = Path.GetFileName(path);
                if (name.Contains("_anim_"))
                    continue;
                if (!rig.NamePrefixes.Any(p => name.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                    continue;
                yield return path.Replace('\\', '/');
            }
        }
    }
}
#endif
