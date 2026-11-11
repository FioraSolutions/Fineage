using System;
using UnityEngine;

public sealed partial class UserSpawner
{
    public UserSpawner(
        CharacterBuilder characterBuilder,
        Func<UserEntity, UserBowArrowEvents> createBowArrowEvents,
        AppearFadeService appearFade,
        L2PawnYaw pawnYaw)
    {
        _characterBuilder = characterBuilder;
        _createBowArrowEvents = createBowArrowEvents;
        _appearFade = appearFade;
        _pawnYaw = pawnYaw;
    }

    public UserEntity Spawn(CharInfoDto info, IWorldSpawnContext world)
    {
        if (info == null || info.Identity == null || world == null)
            return null;

        EntityIdentity identity = info.Identity;
        PlayerAppearance appearance = info.Appearance;
        identity.SetPosY(world.GetGroundHeight(identity.Position));
        identity.EntityType = EntityType.User;

        CharacterRace race = (CharacterRace)appearance.Race;
        int playerClass = identity != null ? identity.PlayerClass : 0;
        int baseBefore = appearance != null ? appearance.BaseClass : -1;
        CharacterRaceAnimation raceId = CharacterRaceAnimationResolve.ResolveRaceAnimation(
            race, appearance, playerClass);
        // Debug.Log("[CHARINFO_CLASS] SPAWN nick=" + identity.Name +
            // " id=" + identity.Id +
            // " race=" + race +
            // " sex=" + (appearance != null ? appearance.Sex : -1) +
            // " classFirst=" + info.PacketClassFirst +
            // " classSecond=" + info.PacketClassSecond +
            // " playerClass=" + playerClass +
            // " baseBefore=" + baseBefore +
            // " baseAfter=" + (appearance != null ? appearance.BaseClass : -1) +
            // " raceId=" + raceId);

        CharacterBuilder builder = _characterBuilder != null ? _characterBuilder : CharacterPacketServices.Builder;
        if (builder == null)
            return null;

        GameObject go = builder.BuildCharacterBase(raceId, appearance, EntityType.User);
        if (go == null)
            return null;

        EntitySpawnShared.Place(go, world.UsersContainer, identity.Position, identity.Heading, identity.Name);

        UserEntity user = go.GetComponent<UserEntity>();
        if (user == null)
        {
            IPoolManager pool = ObjectPoolPacketServices.Pool;
            if (pool == null || !pool.ReturnToPool(ObjectType.User, go))
                UnityEngine.Object.Destroy(go);
            return null;
        }

        ResetRuntime(user);
        user.Status = info.Status;
        user.Identity = identity;
        user.Stats = info.Stats;
        user.Appearance = appearance;
        user.Race = race;
        user.RaceId = raceId;
        user.Running = appearance.Running;
        user.SetDead(info.AlikeDead);
        // Debug.Log("[EntityAction:User] nick=" + identity.Name +
            // " spawn hp=" + user.Status.GetHp() +
            // " maxHp=" + user.Stats.MaxHp +
            // " deadFlag=" + user.GetDead() +
            // " isDead=" + user.IsDead() +
            // " alikeDead=" + info.AlikeDead +
            // " cp=" + (user.Status is PlayerStatus ps ? ps.Cp : 0));

        PlayerController playerController = go.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.enabled = false;

        NetworkTransformShare share = go.GetComponent<NetworkTransformShare>();
        if (share != null)
            share.enabled = false;

        EntitySpawnShared.SanitizeCharacterControllerStepOffset(go);
        EntitySpawnShared.DisableLegacyPositionSync(go);
        App.InjectGameObject(go);
        go.SetActive(true);
        EntitySpawnShared.ReapplyGroundAfterActivate(go, identity);

        UserGear gear = go.GetComponent<UserGear>();
        if (gear != null)
            gear.Initialize(identity.Id, raceId);

        NetworkAnimationController animation = go.GetComponent<NetworkAnimationController>();
        if (animation != null)
            animation.Initialize();

        user.Initialize();
        // Fade swaps in material instances. Snapshot must include armor/weapons equipped above,
        // otherwise a pooled body comes back with those pieces still at alpha 0.
        PooledInstance pooled = go.GetComponent<PooledInstance>();
        if (pooled != null)
            pooled.RecaptureSharedMaterials();

        if (animation != null)
            EntitySpawnShared.RegisterAnimation(world, identity.Id, animation, user);

        if (_createBowArrowEvents != null)
            user.BindCharInfoAnimEvents(_createBowArrowEvents(user));

        user.UpdateRunSpeed(info.Stats.RunRealSpeed);
        user.UpdateWalkSpeed(info.Stats.WalkRealSpeed);
        user.UpdatePAtkSpeed((int)info.Stats.PAtkSpd);
        user.UpdateMAtkSpeed((int)info.Stats.MAtkSpd);
        user.LogSpeed("spawn packet RunReal=" + info.Stats.RunRealSpeed +
            " WalkReal=" + info.Stats.WalkRealSpeed +
            " BaseRun=" + info.Stats.BaseRunSpeed +
            " BaseWalk=" + info.Stats.BaseWalkingSpeed);
        // CharInfoSpeedLog.LogPacket(user, "OnCharInfo spawn");

        world.RegisterUser(user);
        if (info.AlikeDead || user.IsDead())
            EntityActionVisual.PlayDeath(user, true);
        else if (_appearFade != null)
            _appearFade.Begin(user);
        CrowdAnimTest.Apply(user);
        return user;
    }
}
