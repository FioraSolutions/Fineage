using UnityEngine;

public sealed class PlayerSpawner
{
    private readonly CameraController _camera;
    private readonly CharacterInfoWindow _characterInfo;
    private readonly CharacterBuilder _characterBuilder;

    public PlayerSpawner(
        CameraController camera,
        CharacterInfoWindow characterInfo,
        CharacterBuilder characterBuilder)
    {
        _camera = camera;
        _characterInfo = characterInfo;
        _characterBuilder = characterBuilder;
    }

    public PlayerEntity Spawn(
        EntityIdentity identity,
        PlayerStatus status,
        PlayerStats stats,
        PlayerAppearance appearance,
        IWorldSpawnContext world)
    {
        if (identity == null || world == null)
            return null;

        identity.SetPosY(world.GetGroundHeight(identity.Position));
        identity.EntityType = EntityType.Player;

        CharacterRace race = (CharacterRace)appearance.Race;
        int playerClass = identity != null ? identity.PlayerClass : 0;
        CharacterRaceAnimation raceId = CharacterRaceAnimationParser.ResolveRaceAnimation(
            race, appearance, playerClass);
        // Debug.Log("PlayerSpawner body race=" + race +
            // " sex=" + (appearance != null ? appearance.Sex : -1) +
            // " playerClass=" + playerClass +
            // " baseClass=" + (appearance != null ? appearance.BaseClass : -1) +
            // " raceId=" + raceId);

        GameObject go = _characterBuilder.BuildCharacterBaseInterlude(raceId, appearance, identity.EntityType);
        if (go == null)
            return null;

        EntitySpawnShared.SanitizeCharacterControllerStepOffset(go);
        EntitySpawnShared.Place(go, world.UsersContainer, identity.Position, identity.Heading, identity.Name);

        PlayerEntity player = go.GetComponent<PlayerEntity>();
        player.Status = status;
        player.Identity = identity;
        player.Stats = stats;
        player.Appearance = appearance;
        player.Race = race;
        player.RaceId = raceId;
        player.Running = appearance.Running;
        player.SetDead(false);

        NetworkTransformShare share = go.GetComponent<NetworkTransformShare>();
        if (share != null)
            share.enabled = true;
        EntitySpawnShared.DisableLegacyPositionSync(go);
        PlayerController controller = go.GetComponent<PlayerController>();
        if (controller == null)
        {
            // Debug.LogError("PlayerSpawner: PlayerController missing on " + go.name);
            UnityEngine.Object.Destroy(go);
            return null;
        }
        controller.enabled = true;
        controller.Initialize();
        CharacterController cc = go.GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = true;
        App.InjectGameObject(go);
        go.SetActive(true);

        PlayerAnimationController animation = go.GetComponentInChildren<PlayerAnimationController>();
        if (animation != null)
            animation.Initialize();
        EntitySpawnShared.RegisterAnimation(world, identity.Id, animation, player);

        Gear gear = go.GetComponent<Gear>();
        if (gear != null)
            gear.Initialize(player.Identity.Id, player.RaceId);

        PlayerStats statsIntr = (PlayerStats)player.Stats;
        player.Initialize();
        float run = statsIntr.RunRealSpeed > 0.001f ? statsIntr.RunRealSpeed : 120f;
        float walk = statsIntr.WalkRealSpeed > 0.001f ? statsIntr.WalkRealSpeed : 80f;
        player.UpdateRunSpeed(run);
        player.UpdateWalkSpeed(walk);
        // Direct apply — Guarantees PlayerController has speeds even if anim path threw.
        controller.UpdateRunSpeed(StatsConverter.Instance.ConvertStat(Stat.SPEED, run));
        controller.UpdateWalkSpeed(StatsConverter.Instance.ConvertStat(Stat.SPEED, walk));
        player.UpdatePAtkSpeedPlayer((int)statsIntr.BasePAtkSpeed);
        player.UpdateMAtkSpeed((int)statsIntr.MAtkSpd);

        CameraController camera = _camera != null ? _camera : CameraController.Instance;
        if (camera != null)
        {
            camera.enabled = true;
            Camera unityCam = camera.GetComponent<Camera>();
            if (unityCam != null)
                unityCam.enabled = true;
            camera.SetTarget(go);
            camera.SetHeading(identity.OrigHeading);
        }

        if (_characterInfo != null)
            _characterInfo.UpdateValues();
        PlayerStateMachine.AdoptFromPlayer(player);
        if (PlayerStateMachine.Instance != null)
            PlayerStateMachine.Instance.Player = player;

        world.RegisterPlayer(player);
        DiBootstrap.EnsureGameScope();
        return player;
    }

    public void UpdateInfo(Entity entity, UserInfoDto userInfo)
    {
        if (entity == null || userInfo == null || !(entity is PlayerEntity))
            return;

        PlayerEntity player = (PlayerEntity)entity;
        // GearFlowLog.Info("PlayerSpawner.UpdateInfo " +
            // GearFlowLog.Entity(player) + " " + GearFlowLog.Paperdoll(player));
        PlayerStats statsIntr = userInfo.PlayerInfoInterlude.Stats;
        player.UpdateRunSpeed(statsIntr.RunRealSpeed);
        player.UpdateWalkSpeed(statsIntr.WalkRealSpeed);
        player.UpdatePAtkSpeedPlayer((int)statsIntr.BasePAtkSpeed);
        player.UpdateMAtkSpeed((int)statsIntr.MAtkSpd);
        player.RefreshVisuals();
    }
}
