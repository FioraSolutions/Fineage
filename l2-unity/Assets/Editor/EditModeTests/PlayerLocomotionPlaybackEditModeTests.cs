using System.Text.RegularExpressions;
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
        PlayerLocomotionBodyCatalog.ClearStrideCacheForTests();
        LocomotionGait.ClearStrideCache();
        LocomotionGait.ClearMissingClipLogForTests();
    }

    [Test]
    public void PlayerEntity_resolves_run_rate_from_body_catalog()
    {
        LogAssert.Expect(LogType.Error, new Regex(@"\[PlayerLocomotionBody\] profile=MFighter unmeasurable"));
        LogAssert.Expect(LogType.Error, new Regex(@"\[PlayerLocomotionBody\] profile=MFighter unmeasurable"));
        LogAssert.Expect(LogType.Error, new Regex(@"\[PlayerLocomotionBody\] profile=MFighter using FallbackStrideMeters"));
        LogAssert.Expect(LogType.Error, new Regex(@"\[PlayerLocomotionBody\] profile=MFighter using FallbackStrideMeters"));

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
            out float runRate,
            out float walkRate);

        Assert.Greater(runRate, LocomotionGait.MinRunRate, "MFighter run_rate must be catalog-backed");
        Assert.Greater(walkRate, LocomotionGait.MinWalkRate, "MFighter walk_rate must be catalog-backed");

        Object.DestroyImmediate(go);
    }

    [Test]
    public void UserEntity_and_PlayerEntity_yield_same_catalog_run_rate_for_same_profile()
    {
        LogAssert.Expect(LogType.Error, new Regex(@"\[PlayerLocomotionBody\] profile=MFighter unmeasurable"));
        LogAssert.Expect(LogType.Error, new Regex(@"\[PlayerLocomotionBody\] profile=MFighter unmeasurable"));
        LogAssert.Expect(LogType.Error, new Regex(@"\[PlayerLocomotionBody\] profile=MFighter using FallbackStrideMeters"));
        LogAssert.Expect(LogType.Error, new Regex(@"\[PlayerLocomotionBody\] profile=MFighter using FallbackStrideMeters"));

        CharacterRaceAnimation profile = CharacterRaceAnimation.MFighter;
        float unityRun = NumberUtils.ScaleToUnity(120f);

        var playerGo = new GameObject("player-entity");
        PlayerEntity player = playerGo.AddComponent<PlayerEntity>();
        player.RaceId = profile;

        var userGo = new GameObject("user-entity");
        UserEntity user = userGo.AddComponent<UserEntity>();
        user.RaceId = profile;

        PlayerLocomotionPlayback.ResolvePlaybackRates(
            player, null, "run_1HS", "walk_1HS", unityRun, unityRun, out float playerRun, out _);
        PlayerLocomotionPlayback.ResolvePlaybackRates(
            user, null, "run_1HS", "walk_1HS", unityRun, unityRun, out float userRun, out _);

        Assert.AreEqual(playerRun, userRun, 0.0001f,
            "local PlayerEntity and UserEntity must share the catalog stride pipeline");

        Object.DestroyImmediate(playerGo);
        Object.DestroyImmediate(userGo);
    }
}