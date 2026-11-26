using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Regression guard for #1578: local <see cref="PlayerEntity"/> must use the body catalog path and
/// non-zero catalog-backed run rates (not only <see cref="UserEntity"/>).
/// </summary>
public sealed class PlayerLocomotionPlaybackEditModeTests
{
    [TearDown]
    public void TearDown()
    {
        LocomotionStrideCatalog.ClearForTests();
        LocomotionGait.ClearStrideCache();
        LocomotionGait.ClearMissingClipLogForTests();
    }

    [Test]
    public void PlayerEntity_resolves_run_rate_from_body_catalog()
    {
        LogAssert.ignoreFailingMessages = true;
        var go = new GameObject("locomotion-test-player");
        PlayerEntity entity = go.AddComponent<PlayerEntity>();
        entity.RaceId = CharacterRaceAnimation.MFighter;

        float unityRun = NumberUtils.ScaleToUnity(PlayerLocomotionBodyCatalog.DefaultRunSpeedL2j);
        float unityWalk = NumberUtils.ScaleToUnity(PlayerLocomotionBodyCatalog.DefaultWalkSpeedL2j);

        PlayerLocomotionPlayback.ResolvePlaybackRates(
            entity,
            animator: null,
            "run_hand",
            "walk_hand",
            unityRun,
            unityWalk,
            unityRun,
            unityWalk,
            out float runRate,
            out float walkRate);

        Assert.Greater(runRate, LocomotionGait.MinRunRate, "MFighter run_rate must be catalog-backed");
        Assert.Greater(walkRate, LocomotionGait.MinWalkRate, "MFighter walk_rate must be catalog-backed");

        AnimationClip runClip = Resources.Load<AnimationClip>(
            "Data/Animations/Fighter/MFighter/Clips/MFighter_m000_b.ao_run_Hand_MFighter");
        Assert.IsNotNull(runClip);
        AnimNotifyTable.Load();
        Assert.IsTrue(AnimNotifyTable.TryGetBySequence("Run_Hand_MFighter", out var runRows) && runRows.Count > 0);
        float official = runRows[0].FrameCount / runRows[0].Rate;
        float expected = LocomotionGait.PlaybackRateForSequence(
            unityRun, unityRun, runClip.length, official, LocomotionGait.MinRunRate, LocomotionGait.MaxRate);
        Assert.AreEqual(expected, runRate, 0.001f, "run rate is the ukx sequence clock, not the baked stride");
        Assert.Less(runRate, 1f, "a 15fps sequence imported at 24fps must not play at animator speed 1");

        Object.DestroyImmediate(go);
        LogAssert.ignoreFailingMessages = false;
    }

    [Test]
    public void UserEntity_and_PlayerEntity_yield_same_catalog_run_rate_for_same_profile()
    {
        LogAssert.ignoreFailingMessages = true;
        CharacterRaceAnimation profile = CharacterRaceAnimation.MFighter;
        float unityRun = NumberUtils.ScaleToUnity(120f);

        var playerGo = new GameObject("player-entity");
        PlayerEntity player = playerGo.AddComponent<PlayerEntity>();
        player.RaceId = profile;

        var userGo = new GameObject("user-entity");
        UserEntity user = userGo.AddComponent<UserEntity>();
        user.RaceId = profile;

        PlayerLocomotionPlayback.ResolvePlaybackRates(
            player, null, "run_1HS", "walk_1HS", unityRun, unityRun, unityRun, unityRun, out float playerRun, out _);
        PlayerLocomotionPlayback.ResolvePlaybackRates(
            user, null, "run_1HS", "walk_1HS", unityRun, unityRun, unityRun, unityRun, out float userRun, out _);

        Assert.AreEqual(playerRun, userRun, 0.0001f,
            "local PlayerEntity and UserEntity must share the catalog stride pipeline");

        Object.DestroyImmediate(playerGo);
        Object.DestroyImmediate(userGo);
        LogAssert.ignoreFailingMessages = false;
    }

    [Test]
    public void Missing_weapon_clip_returns_zero_and_does_not_use_the_hand_stride()
    {
        LogAssert.ignoreFailingMessages = true;
        var go = new GameObject("missing-weapon");
        PlayerEntity entity = go.AddComponent<PlayerEntity>();
        entity.RaceId = CharacterRaceAnimation.MFighter;

        PlayerLocomotionPlayback.ResolvePlaybackRates(
            entity,
            null,
            "run_not_a_weapon",
            "walk_hand",
            NumberUtils.ScaleToUnity(120f),
            NumberUtils.ScaleToUnity(80f),
            NumberUtils.ScaleToUnity(120f),
            NumberUtils.ScaleToUnity(80f),
            out float runRate,
            out float walkRate);

        Assert.AreEqual(0f, runRate, "missing weapon clip must not fall back to a default or hand stride");
        Assert.Greater(walkRate, LocomotionGait.MinWalkRate, "hand walk stride is baked");

        // MDwarf's file is run_1hs; the animator state is run_1HS.
        entity.RaceId = CharacterRaceAnimation.MDwarf;
        PlayerLocomotionPlayback.ResolvePlaybackRates(
            entity,
            null,
            "run_1HS",
            "walk_hand",
            NumberUtils.ScaleToUnity(120f),
            NumberUtils.ScaleToUnity(80f),
            NumberUtils.ScaleToUnity(120f),
            NumberUtils.ScaleToUnity(80f),
            out float dwarfRun,
            out _);
        Assert.Greater(dwarfRun, LocomotionGait.MinRunRate, "MDwarf run_1hs must resolve despite token case");

        Object.DestroyImmediate(go);
        LogAssert.ignoreFailingMessages = false;
    }
}