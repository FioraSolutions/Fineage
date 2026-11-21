using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public sealed class PlayerSocialAnimatorEditModeTests
{
    const string PlayerBasicPath =
        "Assets/Resources/Data/Animations/Animator/_Template/Player_Basic.controller";

    [Test]
    public void Player_Basic_contains_social01_state_with_placeholder_motion()
    {
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(PlayerBasicPath);
        Assert.NotNull(controller, PlayerBasicPath);

        AnimatorState state = controller.layers[0].stateMachine.states
            .Select(s => s.state)
            .FirstOrDefault(s => s != null && s.name == InterludeSocialAnimationCatalog.SocialAnimatorStateName);

        Assert.NotNull(state, "social01 state on Player_Basic");
        Assert.NotNull(state.motion, "social01 motion");
        Assert.IsTrue(
            state.motion.name.IndexOf(InterludeSocialAnimationCatalog.PlaceholderClipNameFragment,
                System.StringComparison.OrdinalIgnoreCase) >= 0,
            "placeholder clip name contains " + InterludeSocialAnimationCatalog.PlaceholderClipNameFragment);
        Assert.IsTrue(
            state.behaviours.Any(b => b is PlayerStateSocial),
            "PlayerStateSocial behaviour on social01");
    }

    [Test]
    public void Catalog_clip_stems_resolve_for_every_race_folder()
    {
        foreach (string stem in InterludeSocialAnimationCatalog.AllClipStems())
        {
            foreach (var raceRow in InterludeSocialAnimationCatalog.AllRaces())
            {
                Assert.IsTrue(
                    InterludeSocialAnimationCatalog.TryLoadClip(raceRow.Race, stem, out _),
                    "stem=" + stem + " race=" + raceRow.Race + " gender=" +
                    InterludeSocialAnimationCatalog.GenderLabel(raceRow.Race));
            }
        }
    }

    [Test]
    public void Mapped_social_action_ids_match_clip_stems()
    {
        var ids = InterludeSocialAnimationCatalog.AllMappedSocialActionIds();
        var stems = InterludeSocialAnimationCatalog.AllClipStems();
        Assert.AreEqual(ids.Count, stems.Count);
        for (int i = 0; i < ids.Count; i++)
        {
            Assert.IsTrue(
                InterludeSocialAnimationCatalog.TryGetClipStem(ids[i], out string stem),
                "action id=" + ids[i]);
            Assert.AreEqual(stems[i], stem, "id=" + ids[i]);
        }
    }
}